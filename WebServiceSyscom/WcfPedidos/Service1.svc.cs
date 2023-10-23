using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos.Model;
using System.Globalization;
using System.Configuration;
using Newtonsoft.Json;

namespace WcfPedidos
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        string NomProyecto = "Pedidos" + "-" + ConfigurationManager.AppSettings["NitEmpresa"];
        //variables para ordenar lista
        //numero de pagina a consultar
        public int NumPagina = 1;
        //numero de registro total encontrado
        public int ResTotal = 0;
        //registros por pagina
        public int ResPorPagina = 10;
        //numero de registro inicial por pagina
        public int inicio = 0;
        //numero de final de registro por pagina
        public int fin = 10;

        #region generarToken        
        /// <summary>
        /// Generta El token.
        /// </summary>
        /// <param name="usuario">Modelo de datos de usuario.</param>
        /// <returns></returns>
        public RespToken generarToken(Usuarios usuario)
        {
            RespToken respuesta = new RespToken();
            respuesta.Registro = new Log();

            if (usuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Datos no valido" };
            }
            else if (string.IsNullOrWhiteSpace(usuario.Usuario) || string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                GenerarToken token = new GenerarToken();
                respuesta.Token = token.nuevoToken(NomProyecto, usuario.Usuario, usuario.Contraseña, out string[] mensajeNuevo);
                respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
            }
            return respuesta;

        }
        #endregion

        #region ObetenrProducto 
        public RespProducto ConProducto(ObtProducto obtProducto)
        {
            RespProducto respuesta = new RespProducto();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            if (token.VerificarToken(obtProducto.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                Producto p = new Producto();
                respuesta = p.GetProducto(obtProducto, compania, out string[] nuevomensaje);
                respuesta.Registro = new Log { Codigo = nuevomensaje[0], Descripcion = nuevomensaje[1] };
            }
            else
            {
                respuesta.Registro = new Log { Codigo = nuevoMensaje[0], Descripcion = nuevoMensaje[1] };
            }
            return respuesta;

        }
        #endregion

        #region Obtener Clientes
        public RespClientes GetClientes(ObtInfoClientes consulta)
        {
            RespClientes respuesta = new RespClientes();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            if (token.VerificarToken(consulta.Token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                string cliente = "";
                ConexionBD con = new ConexionBD();
                ConexionSQLite conSQLite = new ConexionSQLite("");
                string connectionString = conSQLite.obtenerConexionSyscom().ConnectionString;
                con.setConnection(conSQLite.obtenerConexionSyscom());
                List<ClienteResponse> clientes = new List<ClienteResponse>();
                //variables para saber que datos traer
                if (consulta.Pagina.NumResgitroPagina > 0)
                {
                    ResPorPagina = consulta.Pagina.NumResgitroPagina;
                    fin = ResPorPagina;
                }
                if (consulta.Pagina.Pagina > 0)
                {
                    NumPagina = consulta.Pagina.Pagina;
                    fin = ResPorPagina * NumPagina;
                    inicio = (fin - ResPorPagina) + 1;
                }

                if (!(consulta.NitClinte == null || String.IsNullOrWhiteSpace(consulta.NitClinte)))
                {
                    cliente = consulta.NitClinte;
                    inicio = 1;
                    fin = 1;
                    NumPagina = 1;
                }

                //acceder al procedimiento almacenado
                DataSet TablaClientes = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@NitCliente", cliente));
                parametros.Add(new SqlParameter("@Inicio", inicio));
                parametros.Add(new SqlParameter("@Fin", fin));

                if (con.ejecutarQuery("WSPedido_consObtCliente", parametros, out TablaClientes, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    ResTotal = Convert.ToInt32(TablaClientes.Tables[0].Rows[0]["TotalFilas"]);
                    if (ResTotal > 0)
                    {


                        if (NumPagina <= (int)Math.Ceiling((double)ResTotal / ResPorPagina))
                        {
                            //se insertan los datos solicitados 
                            IEnumerable<DataRow> data = TablaClientes.Tables[1].AsEnumerable();
                            IEnumerable<DataRow> dataFil = data.GroupBy(g => g.Field<string>("IdTercero")).Select(g => g.First());

                            dataFil.ToList().ForEach(i => clientes.Add(new ClienteResponse
                            {

                                pmIdTercero = i.Field<string>("IdTercero"),
                                pmRazonSocial = i.Field<string>("RazonSocial"),
                                pmDireccion = i.Field<string>("Direccion"),
                                pmIdLocal = i.Field<string>("IdLocal"),
                                pmLocalidad = i.Field<string>("Localidad"),
                                pmTelefono = i.Field<string>("Telefono"),
                                pmIdRegimen = i.Field<string>("IdRegimen"),
                                pmRegimen = i.Field<string>("Regimen"),
                                pmIdGrupo = i.Field<string>("IdGrupo"),
                                pmIdSector = i.Field<string>("IdSector"),
                                pmSectorEco = i.Field<string>("SectorEco"),
                                pmIdPlazo = i.Field<string>("IdPlazo"),
                                pmPlazo = i.Field<string>("Plazo"),
                                pmCdCms = i.Field<string>("CdCms"),
                                pmTarCms = i.Field<decimal?>("TarCms"),
                                pmIdVend = i.Field<string>("IdVend"),
                                pmVendedor = i.Field<string>("Vendedor"),
                                pmDiasEntrega = i.Field<string>("CdDiaEnt"),
                                pmNumLista = i.Field<string>("NumLista"),
                                pmCodVend = i.Field<string>("Codigo"),
                                pmIdSZona = i.Field<string>("IdSZona"),
                                pmSubzona = i.Field<string>("SubZona"),
                                pmIdZona = i.Field<string>("IdZona"),
                                pmZona = i.Field<string>("Zona"),
                                pmIdRuta = i.Field<string>("IdRuta"),
                                pmRuta = i.Field<string>("Ruta"),
                                pmInactivo = (i.Field<bool>("Inactivo") ? "1" : "0")
                            }));


                            clientes.ForEach(c =>
                            {
                                if (data.Where(r => r.Field<string>("IdTercero") == c.pmIdTercero && r.Field<string>("IdAgencia") != null).Count() > 0)
                                {
                                    c.pmAgencias = new List<Agencia>();
                                    data.Where(ca => ca.Field<string>("IdTercero") == c.pmIdTercero && ca.Field<string>("IdAgencia") != null).ToList().ForEach(i => c.pmAgencias.Add(new Agencia
                                    {

                                        pmIdAgencia = i.Field<string>("IdAgencia"),
                                        pmNomAgencia = i.Field<string>("Agencia"),
                                        pmDirAgncia = i.Field<string>("DirAgncia"),
                                        pmAgIdLocal = i.Field<string>("AgIdLocal"),
                                        pmAgLocalidad = i.Field<string>("AgLocalidad"),
                                        pmTelAgncia = i.Field<string>("TelAgncia"),
                                        pmNomCont = i.Field<string>("NomCont"),
                                        pmemlCont = i.Field<string>("emlCont"),
                                        pmAgIdVend = i.Field<string>("AgIdVend"),
                                        pmAgVendedor = i.Field<string>("AgVendedor"),
                                        pmAgCdCms = i.Field<string>("AgCdCms"),
                                        pmAgTarCms = i.Field<decimal?>("AgTarCms"),
                                        pmAgCdDct = i.Field<string>("AgCdDct"),
                                        pmAgTarDct = i.Field<decimal?>("AgTarDct"),
                                        pmDiasEntrega = i.Field<string>("CodDiaEnt"),

                                        pmAgIdSZona = i.Field<string>("IdSZonaA"),
                                        pmAgSubzona = i.Field<string>("SubzonaA"),
                                        pmAgIdZona = i.Field<string>("IdZonaA"),
                                        pmAgZona = i.Field<string>("ZonaA"),
                                        pmAgIdRuta = i.Field<string>("IdRutaA"),
                                        pmAgRuta = i.Field<string>("RutaA")
                                    }));
                                }
                            });
                            respuesta.Clientes = clientes;
                            respuesta.Registro = new Log { Codigo = "008", Descripcion = "Se ejecutó correctamente la consulta." };


                        }
                        else
                        {
                            respuesta.Registro = new Log { Codigo = "009", Descripcion = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)ResTotal / ResPorPagina) };

                        }

                    }
                    else
                    {
                        respuesta.Registro = new Log { Codigo = "010", Descripcion = "No se encuentran Clientes disponibles" };

                    }
                }
                else
                {
                    respuesta.Registro = new Log { Codigo = nuevoMennsaje[0], Descripcion = nuevoMennsaje[1] };
                }
                respuesta.paginas = new OrganizadorPagina { PaginaActual = NumPagina, NumeroDePaginas = (int)Math.Ceiling((double)ResTotal / ResPorPagina), RegistroPorPagina = ResPorPagina, RegistroTotal = ResTotal };
            }
            else
            {
                respuesta.Registro = new Log { Codigo = nuevoMensaje[0], Descripcion = nuevoMensaje[1] };
            }


            return respuesta;
        }
        #endregion

        #region Generar pedido
        public ResGenerarPedido resGenerarPedido(GenPedido consulta)
        {
            //variables para mostrar los errores 
            string Codigo = "";
            string Mensaje = "";
            ResGenerarPedido respuesta = new ResGenerarPedido();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            if (token.VerificarToken(consulta.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                //variables para hacer el pedido
                int DiasEntrega = 0;
                DateTime FechaPedido = DateTime.Now;
                string FormaDePago = "";
                string NitContacto = "";
                string NombreContacto = "";
                string TelefonoContacto = "";
                string EmailContacto = "";
                string CargoContacto = "";
                string DiasDePlazo = "";
                string IdVendedor = "";
                //validar la informacion para la cvompañia de registro del pedido
                string Cia = "";
                string Bodega = "";
                string TarifaVendedor = "";
                decimal ValorTarifaVendedor = 0;
                string agencia = "0";
                string Localidad = "";
                string Ruta = "0";
                string Plazo = "0";

                string pmFormaPago = "";
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmFormaPago))
                {
                    pmFormaPago = consulta.DatosDelPedido.pmFormaPago;
                }
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdVendedor))
                {
                    IdVendedor = consulta.DatosDelPedido.pmIdVendedor;
                }
                else
                {
                    IdVendedor = usuario;
                }
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdBodega))
                {
                    Bodega = consulta.DatosDelPedido.pmIdBodega;
                }
                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmTarifaComision))
                {
                    TarifaVendedor = consulta.DatosDelPedido.pmTarifaComision;
                }
                if (!string.IsNullOrEmpty(consulta.Cliente.CdAgencia))
                {
                    agencia = consulta.Cliente.CdAgencia;
                }

                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdCompania))
                {
                    Cia = consulta.DatosDelPedido.pmIdCompania;
                }

                if (!string.IsNullOrEmpty(consulta.Cliente.Municipio))
                {
                    Localidad = consulta.Cliente.Municipio;
                }
                if (!string.IsNullOrEmpty(consulta.Cliente.Ruta))
                {
                    Ruta = consulta.Cliente.Ruta;
                }
                if (!string.IsNullOrEmpty(consulta.Cliente.Plazo))
                {
                    Plazo = consulta.Cliente.Plazo;
                }

                string productosingresados = "";
                string tanqueingresado = "";
                string TarifaDescuento = "";

                //organizamos los datos de producto 
                foreach (ProductosPed productos in consulta.Productos)
                {
                    if (productosingresados == "")
                    {
                        productosingresados += productos.pmIdProducto;
                        if (!string.IsNullOrEmpty(productos.pmIdTanque))
                        {
                            tanqueingresado = productos.pmIdTanque;
                        }
                        if (!string.IsNullOrEmpty(productos.pmIdTarDcto))
                        {
                            TarifaDescuento = productos.pmIdTarDcto;
                        }

                    }
                    else
                    {
                        productosingresados += "," + productos.pmIdProducto;
                        if (!string.IsNullOrEmpty(productos.pmIdTanque))
                        {
                            tanqueingresado += "," + productos.pmIdTanque;
                        }
                        else
                        {
                            tanqueingresado += ",";
                        }

                        if (!string.IsNullOrEmpty(productos.pmIdTarDcto))
                        {
                            TarifaDescuento += "," + productos.pmIdTarDcto;
                        }
                        else
                        {
                            TarifaDescuento += ",";
                        }
                    }

                }


                //realizamos el consulta de los permisos
                ConexionBD ClassConexion = new ConexionBD();
                ConexionSQLite conSqlite = new ConexionSQLite("");
                string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                DataSet TablaPermisos = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Usuario", usuario));
                parametros.Add(new SqlParameter("@Cia", compania));
                parametros.Add(new SqlParameter("@Cliente", consulta.Cliente.Documento));
                parametros.Add(new SqlParameter("@FormaPago", pmFormaPago));
                parametros.Add(new SqlParameter("@vendedor", IdVendedor));
                parametros.Add(new SqlParameter("@Bodega", Bodega));
                parametros.Add(new SqlParameter("@TarifaVendedor", TarifaVendedor));
                parametros.Add(new SqlParameter("@Agencia", agencia));
                parametros.Add(new SqlParameter("@Productos", productosingresados));
                parametros.Add(new SqlParameter("@Tanques", tanqueingresado));
                parametros.Add(new SqlParameter("@CiIngresado", Cia));
                parametros.Add(new SqlParameter("@TarifaDescuentos", TarifaDescuento));
                parametros.Add(new SqlParameter("@localidad", Localidad));
                parametros.Add(new SqlParameter("@Ruta", Ruta));
                parametros.Add(new SqlParameter("@Plazo", Plazo));


                if (ClassConexion.ejecutarQuery("WSPedido_ConsPerVenta", parametros, out TablaPermisos, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    if (TablaPermisos.Tables[0].Rows.Count > 0)
                    {
                        DataRow Permisos = TablaPermisos.Tables[0].Rows[0];

                        //comprobar que tenga habilidato el permiso de vendedor
                        if (Permisos.Field<bool>("Esvendedor"))
                        {
                            //Que tenga un nivel 3 en adelante para hacer pedido
                            if (Permisos.Field<int>("Nivel") >= 3)
                            {
                                //Comprobar que tenga habilitado el permiso de hacer pedido
                                if (Permisos.Field<string>("Permiso") == "FRMDPED")
                                {
                                    //comprobar que el cliente no se encuentr bloqueado y tenga permiso para desbloquear                                 
                                    if (Permisos.Field<string>("Cliente") == "activo")
                                    {
                                        //Validar que el cliente se encuentre en la compañia
                                        if (Permisos.Field<string>("HabCompania") == "permitido")
                                        {
                                            //validamos si cumple con la localidad del pedido 
                                            if ((Permisos.Field<string>("Localidad") != "denegado"))
                                            {
                                                //verificamos la ruta
                                                if (Permisos.Field<string>("ruta") != "denegado")
                                                {
                                                    //verificamos el estado del pedido si tiene permisdo o no
                                                    if ((Permisos.Field<string>("PermisoEST") != "denegado") | (string.IsNullOrEmpty(consulta.DatosDelPedido.pmEstadoPedido)) | (consulta.DatosDelPedido.pmEstadoPedido == "0001"))
                                                    {
                                                        if (string.IsNullOrEmpty(consulta.DatosDelPedido.pmEstadoPedido))
                                                        {
                                                            consulta.DatosDelPedido.pmEstadoPedido = "0001";
                                                        }
                                                       
                                                        //verificamos que el plazo ingresado sea el adecuado
                                                        if (Permisos.Field<string>("Plazo") != "denegado")
                                                        {
                                                            //validar si esta en mora 
                                                            if (Permisos.Field<string>("PermisoMor") == "MOR" | Permisos.Field<string>("EstadoMora") == "AlDia")
                                                            {
                                                                //validamos la agencia
                                                                bool permisoAgencia = false;

                                                                if (!string.IsNullOrEmpty(consulta.Cliente.CdAgencia))
                                                                {
                                                                    if (Permisos.Field<string>("ExisteAgencia") != "denegado")
                                                                    {
                                                                        permisoAgencia = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        Codigo = "044";
                                                                        Mensaje = "La agencia agregada no pertenece al cliente";
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    permisoAgencia = true;
                                                                    if (Permisos.Field<string>("AgenciaAsiganada") != "denegado")
                                                                    {
                                                                        agencia = Permisos.Field<string>("AgenciaAsiganada");
                                                                    }
                                                                }

                                                                if (permisoAgencia)
                                                                {
                                                                    //validamos que la tarifa ingresada exita
                                                                    if (Permisos.Field<string>("ExisteTarifa") != "dene" | string.IsNullOrEmpty(consulta.DatosDelPedido.pmTarifaComision))
                                                                    {
                                                                        bool permisoTarifa = false;

                                                                        if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmTarifaComision))
                                                                        {
                                                                            if (Permisos.Field<string>("ExisteTarifa") == "dene")
                                                                            {
                                                                                Codigo = "046";
                                                                                Mensaje = "El codigo de pmTarifaComision no existe en syscom";
                                                                            }
                                                                            else
                                                                            {
                                                                                //validamos para que sea igual 
                                                                                if ((Permisos.Field<string>("PermisoCms") == "CMS") | (Permisos.Field<string>("ExisteTarifa") == Permisos.Field<string>("TarifaVendedor")))
                                                                                {
                                                                                    TarifaVendedor = Permisos.Field<string>("ExisteTarifa");
                                                                                    ValorTarifaVendedor = Permisos.Field<decimal>("ValorTarifaIngre");
                                                                                    permisoTarifa = true;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Codigo = "047";
                                                                                    Mensaje = "El usuario no tiene permiso CMS para cambiar la tarifa";
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            TarifaVendedor = Permisos.Field<string>("TarifaVendedor");
                                                                            ValorTarifaVendedor = Permisos.Field<decimal>("ValorTarifaExis");
                                                                            permisoTarifa = true;
                                                                        }

                                                                        if (permisoTarifa)
                                                                        {
                                                                            //validar que si tiene el permiso EGA y si ingreso un valor en diasEntrega
                                                                            if ((Permisos.Field<string>("PermisoEGA") == "EGA") | (consulta.Cliente.DiasEntrega > 0) | (consulta.Cliente.DiasEntrega == Permisos.Field<int>("DiasEntrega")))
                                                                            {
                                                                                bool permisoParaFEC = false;
                                                                                //se realiza la evaluacion de los datos para saber si cumple con las  condiciones para la fecha
                                                                                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmFechaPedido))
                                                                                {
                                                                                    if (DateTime.TryParse(consulta.DatosDelPedido.pmFechaPedido, out DateTime fechaingresada))
                                                                                    {
                                                                                        if (DateTime.TryParse(Permisos.Field<string>("FechaDelPedidod"), out DateTime fechabase))
                                                                                        {
                                                                                            if (fechaingresada.ToString("yyyyMMdd") == fechabase.ToString("yyyyMMdd"))
                                                                                            {
                                                                                                FechaPedido = fechaingresada;
                                                                                                permisoParaFEC = true;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Codigo = "023";
                                                                                                Mensaje = "No tiene el permiso habilitado fecha abierta";
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            //verificamos que el formato que tenemos y si cumple 
                                                                                            if (Permisos.Field<string>("PermisoFEC") == "FEC")
                                                                                            {
                                                                                                //comprobamos los valores que estan
                                                                                                if (Permisos.Field<string>("FechaDelPedidod") == "D")
                                                                                                {
                                                                                                    if (fechaingresada.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                                                                                                    {
                                                                                                        FechaPedido = fechaingresada;
                                                                                                        permisoParaFEC = true;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "024";
                                                                                                        Mensaje = "La fecha No validad debe ser la fecha del dia de hoy";
                                                                                                    }
                                                                                                }
                                                                                                else if (Permisos.Field<string>("FechaDelPedidod") == "S")
                                                                                                {
                                                                                                    DateTime fechaActual = DateTime.Now; // Fecha y hora actual

                                                                                                    // Calcular el primer día de la semana actual
                                                                                                    DateTime primerDiaSemana = fechaActual.Date.AddDays(-(int)fechaActual.DayOfWeek);

                                                                                                    // Calcular el último día de la semana actual
                                                                                                    DateTime ultimoDiaSemana = primerDiaSemana.AddDays(6);

                                                                                                    if (fechaingresada < primerDiaSemana || fechaingresada > ultimoDiaSemana)
                                                                                                    {
                                                                                                        Codigo = "025";
                                                                                                        Mensaje = "la fecha ingresada debe coincidir con la semana actual";
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        FechaPedido = fechaingresada;
                                                                                                        permisoParaFEC = true;
                                                                                                    }

                                                                                                }
                                                                                                else if (Permisos.Field<string>("FechaDelPedidod") == "M")
                                                                                                {
                                                                                                    if (fechaingresada.ToString("yyyyMM") == DateTime.Now.ToString("yyyyMM"))
                                                                                                    {
                                                                                                        FechaPedido = fechaingresada;
                                                                                                        permisoParaFEC = true;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "026";
                                                                                                        Mensaje = "La fecha ingresada debe coincidir con el mes actual";
                                                                                                    }
                                                                                                }
                                                                                                else if (Permisos.Field<string>("FechaDelPedidod") == "A")
                                                                                                {
                                                                                                    if (fechaingresada.Year == DateTime.Now.Year)
                                                                                                    {
                                                                                                        FechaPedido = fechaingresada;
                                                                                                        permisoParaFEC = true;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "027";
                                                                                                        Mensaje = "La fecha ingresada debe coincidir con el año actual";
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (double.TryParse(Permisos.Field<string>("FechaDelPedidod"), out double numero))
                                                                                                    {
                                                                                                        //se convierte el valor 
                                                                                                        if (numero > 0)
                                                                                                        {
                                                                                                            DateTime fechaActual = DateTime.Now; // Fecha y hora actual

                                                                                                            // Calcular el primer día de la semana actual
                                                                                                            DateTime primerDia = fechaActual;

                                                                                                            // Calcular el último día de la semana actual
                                                                                                            DateTime ultimoDiaSemana = primerDia.AddDays(numero);

                                                                                                            if (fechaingresada < primerDia || fechaingresada > ultimoDiaSemana)
                                                                                                            {
                                                                                                                Codigo = "035";
                                                                                                                Mensaje = "la fecha ingresada debe coincidir con los dias establecidos";
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                FechaPedido = fechaingresada;
                                                                                                                permisoParaFEC = true;
                                                                                                            }

                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            DateTime fechaActual = DateTime.Now; // Fecha y hora actual

                                                                                                            // Calcular el primer día de la semana actual
                                                                                                            DateTime primerDia = fechaActual.AddDays(numero);

                                                                                                            // Calcular el último día de la semana actual
                                                                                                            DateTime ultimoDiaSemana = fechaActual;

                                                                                                            if (fechaingresada < primerDia || fechaingresada > ultimoDiaSemana)
                                                                                                            {
                                                                                                                Codigo = "035";
                                                                                                                Mensaje = "la fecha ingresada debe coincidir con los dias establecidos";
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                FechaPedido = fechaingresada;
                                                                                                                permisoParaFEC = true;
                                                                                                            }

                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "034";
                                                                                                        Mensaje = "Formato de fecha establecida en syscom no establecidad en el sistema";
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Codigo = "022";
                                                                                        Mensaje = "el formato que ingreso EN pmFechaPedido no es valido";
                                                                                    }

                                                                                }
                                                                                else
                                                                                {
                                                                                    permisoParaFEC = true;
                                                                                }

                                                                                if (permisoParaFEC)
                                                                                {
                                                                                    //comprobar forma de pago del cliente
                                                                                    bool permisoFOR = false;
                                                                                    if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmFormaPago))
                                                                                    {
                                                                                        if (Permisos.Field<string>("FormaDePagoIngresado") != "dene")
                                                                                        {
                                                                                            if (Permisos.Field<string>("FormaDePagoIngresado") == Permisos.Field<string>("FormaDePago"))
                                                                                            {
                                                                                                FormaDePago = Permisos.Field<string>("FormaDePago");
                                                                                                permisoFOR = true;
                                                                                            }
                                                                                            else if (Permisos.Field<string>("PermisoFOR") == "FOR")
                                                                                            {
                                                                                                FormaDePago = Permisos.Field<string>("FormaDePagoIngresado");
                                                                                                permisoFOR = true;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Codigo = "029";
                                                                                                Mensaje = "El usuario no tiene permiso para cambiar la forma de pago";
                                                                                            }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Codigo = "028";
                                                                                            Mensaje = "la forma de pago ingresado no esta registrada en syscom";
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        FormaDePago = Permisos.Field<string>("FormaDePago");
                                                                                        permisoFOR = true;
                                                                                    }
                                                                                    if (permisoFOR)
                                                                                    {
                                                                                        //verificamos la informacion de contacto y si cuenta con el permiso 
                                                                                        bool permisoMCO = false;
                                                                                        //comprobamos si hay un valor ingresado
                                                                                        if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.NitContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.NombreContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.TelefonoContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.EmailContacto) | !string.IsNullOrEmpty(consulta.InformacionDeContacto.CargoContacto))
                                                                                        {
                                                                                            if (Permisos.Field<string>("PermisoMCO") == "MCO")
                                                                                            {
                                                                                                if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.NitContacto))
                                                                                                {
                                                                                                    NitContacto = consulta.InformacionDeContacto.NitContacto;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NitContacto = Permisos.Field<string>("ContaNIt");
                                                                                                }

                                                                                                if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.NombreContacto))
                                                                                                {
                                                                                                    NitContacto = consulta.InformacionDeContacto.NombreContacto;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NitContacto = Permisos.Field<string>("ContaNombre");
                                                                                                }

                                                                                                if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.TelefonoContacto))
                                                                                                {
                                                                                                    NitContacto = consulta.InformacionDeContacto.TelefonoContacto;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NitContacto = Permisos.Field<string>("ContaTelefono");
                                                                                                }

                                                                                                if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.EmailContacto))
                                                                                                {
                                                                                                    NitContacto = consulta.InformacionDeContacto.EmailContacto;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NitContacto = Permisos.Field<string>("ContaEmail");
                                                                                                }

                                                                                                if (!string.IsNullOrEmpty(consulta.InformacionDeContacto.CargoContacto))
                                                                                                {
                                                                                                    NitContacto = consulta.InformacionDeContacto.CargoContacto;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NitContacto = Permisos.Field<string>("ContaCargo");
                                                                                                }
                                                                                                permisoMCO = true;


                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Codigo = "030";
                                                                                                Mensaje = "El usuario no tiene permiso para cambiar la informacion de contacto";
                                                                                            }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            permisoMCO = true;
                                                                                            NitContacto = Permisos.Field<string>("ContaNIt");
                                                                                            NombreContacto = Permisos.Field<string>("ContaNombre");
                                                                                            TelefonoContacto = Permisos.Field<string>("ContaTelefono");
                                                                                            EmailContacto = Permisos.Field<string>("ContaEmail");
                                                                                            CargoContacto = Permisos.Field<string>("ContaCargo");
                                                                                        }
                                                                                        if (permisoMCO)
                                                                                        {
                                                                                            //verificar los dias de plazo 
                                                                                            bool permisoPZO = false;
                                                                                            if (consulta.DatosDelPedido.pmDiasPlazo > 0)
                                                                                            {
                                                                                                if ((Permisos.Field<string>("PermisoPZO") == "PZO") | (consulta.DatosDelPedido.pmDiasPlazo == Permisos.Field<int>("diasplazo")))
                                                                                                {
                                                                                                    DiasDePlazo = consulta.DatosDelPedido.pmDiasPlazo.ToString();
                                                                                                    permisoPZO = true;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Codigo = "031";
                                                                                                    Mensaje = "El usuario non tiene permisos para cambiar los dias de plazo";
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                DiasDePlazo = Permisos.Field<string>("diasplazo");
                                                                                                permisoPZO = true;
                                                                                            }
                                                                                            if (permisoPZO)
                                                                                            {
                                                                                                bool permisoVEN = false;
                                                                                                //permiso para vender a un cliente que no tiene asignado el vendedor
                                                                                                if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdVendedor))
                                                                                                {
                                                                                                    if ((Permisos.Field<string>("PermisoVEN") == "VEN") | (consulta.DatosDelPedido.pmIdVendedor == Permisos.Field<string>("VendedorDelCLiente")))
                                                                                                    {
                                                                                                        if (Permisos.Field<bool>("EsVendedorIngresado"))
                                                                                                        {
                                                                                                            permisoVEN = true;
                                                                                                            IdVendedor = consulta.DatosDelPedido.pmIdVendedor;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            Codigo = "036";
                                                                                                            Mensaje = "El pmIdVendedor ingresado no esta marcado como vendedor";
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "034";
                                                                                                        Mensaje = "El usuario no tiene permiso VEN para vender a otro cliente";
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (usuario == Permisos.Field<string>("VendedorDelCLiente"))
                                                                                                    {
                                                                                                        permisoVEN = true;
                                                                                                        IdVendedor = usuario;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Codigo = "032";
                                                                                                        Mensaje = "El usuario no tiene permiso VEN para vender a otro cliente";
                                                                                                    }

                                                                                                }

                                                                                                //comprobamos el valor de la compañia para registrarlo
                                                                                                if ((Permisos.Field<string>("PermisoCia") == "CIA") | (consulta.DatosDelPedido.pmIdCompania == compania))
                                                                                                {
                                                                                                    Cia = consulta.DatosDelPedido.pmIdCompania;

                                                                                                    //comprobamos para añadir una tarifa diferente a la del vendedor
                                                                                                    if (permisoVEN)
                                                                                                    {

                                                                                                        if (TablaPermisos.Tables[2].Rows.Count > 0)
                                                                                                        {

                                                                                                            bool permisoLTA = false;

                                                                                                            bool permisoDCT = false;
                                                                                                            bool permisoOBQ = false;
                                                                                                            bool permisoPEC = false;
                                                                                                            bool permisoBOD = false;
                                                                                                            bool verificacionCompleta = true;
                                                                                                            //verificamos la lista preterminada 
                                                                                                            if (Permisos.Field<string>("PermisoLTA") == "LTA")
                                                                                                            {
                                                                                                                permisoLTA = true;
                                                                                                            }

                                                                                                            //permiso para agregar una bodega 
                                                                                                            if (Permisos.Field<string>("PermisoBod") == "BOD")
                                                                                                            {
                                                                                                                permisoBOD = true;
                                                                                                            }
                                                                                                            if (Permisos.Field<string>("PermisoDCT") == "DCT")
                                                                                                            {
                                                                                                                permisoDCT = true;
                                                                                                            }

                                                                                                            DataTable productosConsulta = TablaPermisos.Tables[1];
                                                                                                            int recorrer = 0;
                                                                                                            //creamos dos columnas nuevas para añadir los datos del precio y descuento
                                                                                                            productosConsulta.Columns.Add("precio", typeof(decimal));
                                                                                                            productosConsulta.Columns.Add("descuento", typeof(decimal));
                                                                                                            productosConsulta.Columns.Add("PrecioTotalConIva", typeof(decimal));
                                                                                                            productosConsulta.Columns.Add("TotalIten", typeof(int));
                                                                                                            productosConsulta.Columns.Add("Obsequios", typeof(int));
                                                                                                            productosConsulta.Columns.Add("Cantidad", typeof(int));
                                                                                                            productosConsulta.Columns.Add("TotalIva", typeof(decimal));
                                                                                                            productosConsulta.Columns.Add("SubTotal", typeof(decimal));
                                                                                                            productosConsulta.Columns.Add("ValorUnitario", typeof(decimal));
                                                                                                            foreach (ProductosPed productos in consulta.Productos)
                                                                                                            {
                                                                                                                //comprobamos que el producto exista
                                                                                                                if (productosConsulta.Rows[recorrer].Field<string>("IdProducto") == productos.pmIdProducto)
                                                                                                                {
                                                                                                                    int ListaProd = 1;
                                                                                                                    //verificamos que el producto este disponible
                                                                                                                    if (productosConsulta.Rows[recorrer].Field<string>("disponibleCia") == "disponible")
                                                                                                                    {

                                                                                                                        //comprobamos el valor a escoger de la lista
                                                                                                                        if (productos.pmIdListaDePrecio != 0)
                                                                                                                        {
                                                                                                                            if (permisoLTA)
                                                                                                                            {
                                                                                                                                if (productos.pmIdListaDePrecio <= 5 & productos.pmIdListaDePrecio > 0)
                                                                                                                                {
                                                                                                                                    //realizamos el cambio del precio a la primera fila
                                                                                                                                    string nomColumPrecio = "pmVrPrecio" + productos.pmIdListaDePrecio;
                                                                                                                                    string nomColumDescuentp = "CdDct" + productos.pmIdListaDePrecio;
                                                                                                                                    productosConsulta.Rows[recorrer]["precio"] = productosConsulta.Rows[recorrer][nomColumPrecio];
                                                                                                                                    productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer][nomColumDescuentp];
                                                                                                                                    ListaProd = productos.pmIdListaDePrecio;

                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    Codigo = "039";
                                                                                                                                    Mensaje = "La lista ingresada es incorrecta debe estar en un valor entre 1 y 5";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else if (productos.pmIdListaDePrecio == productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio"))
                                                                                                                            {
                                                                                                                                string nomColumPrecio = "pmVrPrecio" + productos.pmIdListaDePrecio;
                                                                                                                                string nomColumDescuentp = "CdDct" + productos.pmIdListaDePrecio;
                                                                                                                                productosConsulta.Rows[recorrer]["precio"] = productosConsulta.Rows[recorrer][nomColumPrecio];
                                                                                                                                productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer][nomColumDescuentp];
                                                                                                                                ListaProd = productos.pmIdListaDePrecio;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Codigo = "041";
                                                                                                                                Mensaje = "No tiene permisos para ingresar una lista diferente a la establecidad";
                                                                                                                                verificacionCompleta = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            string nomColumPrecio = "pmVrPrecio" + productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio");
                                                                                                                            string nomColumDescuentp = "CdDct" + productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio");
                                                                                                                            productosConsulta.Rows[recorrer]["precio"] = productosConsulta.Rows[recorrer][nomColumPrecio];
                                                                                                                            productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer][nomColumDescuentp];
                                                                                                                            ListaProd = productosConsulta.Rows[recorrer].Field<int>("ListaDePrecio");
                                                                                                                        }

                                                                                                                        //validamos la bodega del producto
                                                                                                                        if (!string.IsNullOrEmpty(consulta.DatosDelPedido.pmIdBodega))
                                                                                                                        {
                                                                                                                            //comprobamos si el valor es distintop para el correspondiente cambio
                                                                                                                            if (consulta.DatosDelPedido.pmIdBodega != productosConsulta.Rows[recorrer].Field<string>("Bodega"))
                                                                                                                            {
                                                                                                                                if (permisoBOD)
                                                                                                                                {
                                                                                                                                    if (Permisos.Field<string>("BodegaPermitida") != "dene")
                                                                                                                                    {
                                                                                                                                        productosConsulta.Rows[recorrer]["Bodega"] = Permisos.Field<string>("BodegaPermitida");
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        Codigo = "043";
                                                                                                                                        Mensaje = "La bodega ingresada no esta permitida en la compañia";
                                                                                                                                        verificacionCompleta = false;
                                                                                                                                        break;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    Codigo = "042";
                                                                                                                                    Mensaje = "El usuario no tiene permiso BOD para cambiar la bodega";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }

                                                                                                                        //se realiza la modificacion del precio para saber si obsequio 
                                                                                                                        if ((productos.pmCantObsequio > 0) & (productos.pmCantidad == 0))
                                                                                                                        {
                                                                                                                            //si solo esta la cantida de obsequio se cambia el valor para registarar el nuevo valor de solo el iva
                                                                                                                            productosConsulta.Rows[recorrer]["precio"] = (productosConsulta.Rows[recorrer].Field<decimal>("precio") * productosConsulta.Rows[recorrer].Field<decimal>("IVA")) / 100;
                                                                                                                        }

                                                                                                                        //varificamos que el valor del producto sea igual a cero y que tenga el permiso PEC
                                                                                                                        if (productos.pmVrPrecio == 0)
                                                                                                                        {
                                                                                                                            if (Permisos.Field<string>("PermisoPEC") == "PEC")
                                                                                                                            {
                                                                                                                                productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                //se verifica que el producto 
                                                                                                                                Codigo = "048";
                                                                                                                                Mensaje = "El usuario no tiene permiso PEC para registrar el producto en valor cero";
                                                                                                                                verificacionCompleta = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else if (productosConsulta.Rows[recorrer].Field<decimal>("precio") == productos.pmVrPrecio)
                                                                                                                        {
                                                                                                                            //validacion que el precio sea igual a de la lista
                                                                                                                            productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                        }
                                                                                                                        else if (productosConsulta.Rows[recorrer].Field<decimal>("precio") != productos.pmVrPrecio)
                                                                                                                        {
                                                                                                                            //asiganamos una tabla para agregar la tabla de tarifas expeciales y saber en que parte esta el desceunto 
                                                                                                                            DataTable TablaTarifaEspecial = TablaPermisos.Tables[2];
                                                                                                                            //verificamos qu en la tabla tenga resultados para realizar el proceso 
                                                                                                                            if (TablaPermisos.Tables[2].Rows.Count > 0)
                                                                                                                            {
                                                                                                                                //Se realiza la consulta del valor
                                                                                                                                var filasCoincidentes = TablaTarifaEspecial.AsEnumerable()
                                                                                                                                    .Where(row => row.Field<decimal>("tarifa") == productos.pmVrPrecio && row.Field<string>("simb") == "$")
                                                                                                                                      .CopyToDataTable();

                                                                                                                                //se verifica si encuentra resultados
                                                                                                                                if (filasCoincidentes.Rows.Count > 0)
                                                                                                                                {
                                                                                                                                    //se realiza la comprobacion si cumple con los criterios del producto
                                                                                                                                    bool ValorExiste = filasCoincidentes.AsEnumerable()
                                                                                                                                    .Any(row => row.Field<string>("CdProducto") == productos.pmIdProducto ||
                                                                                                                                                row.Field<string>("CdMarca") == productosConsulta.Rows[recorrer].Field<string>("Marca") ||
                                                                                                                                                row.Field<string>("CdSubgrupo") == productosConsulta.Rows[recorrer].Field<string>("subgrupo") ||
                                                                                                                                                row.Field<string>("CdGrupo") == productosConsulta.Rows[recorrer].Field<string>("Grupo") ||
                                                                                                                                                row.Field<string>("cdlinea") == productosConsulta.Rows[recorrer].Field<string>("Linea"));

                                                                                                                                    if (ValorExiste)
                                                                                                                                    {
                                                                                                                                        productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                                    }
                                                                                                                                }


                                                                                                                            }
                                                                                                                        }
                                                                                                                        // se realiza la vrificacion si l precio ya coincide o sino se procede a verificar los permisos 
                                                                                                                        if (productosConsulta.Rows[recorrer].Field<decimal>("precio") != productos.pmVrPrecio)
                                                                                                                        {
                                                                                                                            string permisoMP = "PermisoMP" + ListaProd;
                                                                                                                            string MP = "MP" + ListaProd;
                                                                                                                            if (Permisos.Field<string>(permisoMP) == MP)
                                                                                                                            {
                                                                                                                                productosConsulta.Rows[recorrer]["precio"] = productos.pmVrPrecio;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Codigo = "049";
                                                                                                                                Mensaje = "El usuario no tiene permiso " + MP + " para cambiar el valor del producto";
                                                                                                                                verificacionCompleta = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                        }

                                                                                                                        //validamos la tarifa de descuento 
                                                                                                                        //varificamos que el usuario ingreso una tarifa
                                                                                                                        if (!string.IsNullOrEmpty(productos.pmIdTarDcto))
                                                                                                                        {
                                                                                                                            //verificamos la tarifa ingresada sea validad
                                                                                                                            if (productosConsulta.Rows[recorrer].Field<string>("ExisteTarifa") == "dene")
                                                                                                                            {
                                                                                                                                Codigo = "050";
                                                                                                                                Mensaje = "La tarifa de descuento " + productos.pmIdTarDcto + " no Existe";
                                                                                                                                verificacionCompleta = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                            else if (productosConsulta.Rows[recorrer].Field<decimal>("descuento") == productosConsulta.Rows[recorrer].Field<decimal>("Tarifa"))
                                                                                                                            {
                                                                                                                                //si los valores coinciden se dejan igual 
                                                                                                                            }
                                                                                                                            else if (permisoDCT)
                                                                                                                            {
                                                                                                                                //se cambia el valor por tener el permiso de cambiar la< tarifa
                                                                                                                                productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer]["Tarifa"];

                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                // se realiza la comprobacion con la tarifa en especial si no cumple arrojar error


                                                                                                                                //asiganamos una tabla para agregar la tabla de tarifas expeciales y saber en que parte esta el desceunto 
                                                                                                                                DataTable TablaTarifaDescuento = TablaPermisos.Tables[2];
                                                                                                                                //verificamos qu en la tabla tenga resultados para realizar el proceso 
                                                                                                                                if (TablaPermisos.Tables[2].Rows.Count > 0)
                                                                                                                                {
                                                                                                                                    //Se realiza la consulta del valor
                                                                                                                                    var filasCoincidentes = TablaTarifaDescuento.AsEnumerable()
                                                                                                                                        .Where(row => row.Field<decimal>("tarifa") == productos.pmVrPrecio && row.Field<string>("simb") == "%")
                                                                                                                                          .CopyToDataTable();

                                                                                                                                    //se verifica si encuentra resultados
                                                                                                                                    if (filasCoincidentes.Rows.Count > 0)
                                                                                                                                    {
                                                                                                                                        //se realiza la comprobacion si cumple con los criterios del producto
                                                                                                                                        bool ValorExiste = filasCoincidentes.AsEnumerable()
                                                                                                                                        .Any(row => row.Field<string>("CdProducto") == productos.pmIdProducto ||
                                                                                                                                                    row.Field<string>("CdMarca") == productosConsulta.Rows[recorrer].Field<string>("Marca") ||
                                                                                                                                                    row.Field<string>("CdSubgrupo") == productosConsulta.Rows[recorrer].Field<string>("subgrupo") ||
                                                                                                                                                    row.Field<string>("CdGrupo") == productosConsulta.Rows[recorrer].Field<string>("Grupo") ||
                                                                                                                                                    row.Field<string>("cdlinea") == productosConsulta.Rows[recorrer].Field<string>("Linea"));

                                                                                                                                        if (ValorExiste)
                                                                                                                                        {
                                                                                                                                            productosConsulta.Rows[recorrer]["descuento"] = productosConsulta.Rows[recorrer]["Tarifa"];
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            Codigo = "051";
                                                                                                                                            Mensaje = "El usuario no tiene permiso DCT para cambiar la tarifa de descuento";
                                                                                                                                            verificacionCompleta = false;
                                                                                                                                            break;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        Codigo = "051";
                                                                                                                                        Mensaje = "El usuario no tiene permiso DCT para cambiar la tarifa de descuento";
                                                                                                                                        verificacionCompleta = false;
                                                                                                                                        break;
                                                                                                                                    }


                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    Codigo = "051";
                                                                                                                                    Mensaje = "El usuario no tiene permiso DCT para cambiar la tarifa de descuento";
                                                                                                                                    verificacionCompleta = false;
                                                                                                                                    break;
                                                                                                                                }
                                                                                                                            }


                                                                                                                        }

                                                                                                                        //se realiza la suma total para saber el valor total del producto
                                                                                                                        //obtenemos a valor con el descuento


                                                                                                                        decimal valor = productosConsulta.Rows[recorrer].Field<decimal>("precio") + ((productosConsulta.Rows[recorrer].Field<decimal>("precio") * (productosConsulta.Rows[recorrer].Field<decimal?>("descuento") ?? 0)) * 100);

                                                                                                                        productosConsulta.Rows[recorrer]["SubTotal"] = valor;

                                                                                                                        //obtenemos el valor con el iva
                                                                                                                        valor = ((valor * (productosConsulta.Rows[recorrer].Field<decimal?>("IVA") ?? 0)) / 100) + valor;

                                                                                                                        //obtenemos el valor para obsequios
                                                                                                                        decimal CanObsequio = (productosConsulta.Rows[recorrer].Field<decimal>("precio") * (productosConsulta.Rows[recorrer].Field<decimal?>("IVA") ?? 0)) / 100;
                                                                                                                        CanObsequio = CanObsequio * productos.pmCantObsequio;



                                                                                                                        productosConsulta.Rows[recorrer]["PrecioTotalConIva"] = valor * productos.pmCantidad;
                                                                                                                        productosConsulta.Rows[recorrer]["ValorUnitario"] = valor;

                                                                                                                        valor = valor * productos.pmCantObsequio;
                                                                                                                        productosConsulta.Rows[recorrer]["PrecioTotalConIva"] = productosConsulta.Rows[recorrer].Field<decimal>("PrecioTotalConIva") + CanObsequio;
                                                                                                                        productosConsulta.Rows[recorrer]["TotalIten"] = productos.pmCantidad + productos.pmCantObsequio;
                                                                                                                        productosConsulta.Rows[recorrer]["Obsequios"] = productos.pmCantObsequio;
                                                                                                                        productosConsulta.Rows[recorrer]["Cantidad"] = productos.pmCantidad;
                                                                                                                        productosConsulta.Rows[recorrer]["TotalIva"] = (productosConsulta.Rows[recorrer].Field<decimal>("precio") * (productosConsulta.Rows[recorrer].Field<decimal?>("IVA") ?? 0)) / 100;

                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        Codigo = "047";
                                                                                                                        Mensaje = "El producto ingresado con codigo " + productos.pmIdProducto + "y descripcion " + productosConsulta.Rows[recorrer].Field<string>("DescripProd") + " No esta disponible en la compañia";
                                                                                                                        verificacionCompleta = false;
                                                                                                                        break;
                                                                                                                    }

                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    Codigo = "040";
                                                                                                                    Mensaje = "El producto ingresado " + productos.pmIdProducto + " No Existe";
                                                                                                                    verificacionCompleta = false;
                                                                                                                    break;
                                                                                                                }
                                                                                                                recorrer++;
                                                                                                            }

                                                                                                            if (verificacionCompleta)
                                                                                                            {
                                                                                                                //verificamos el cupo si cumple 
                                                                                                                decimal sumaPrecio = productosConsulta.AsEnumerable()
                                                                                                                         .Sum(row => row.Field<decimal>("PrecioTotalConIva"));

                                                                                                                int sumaIten = productosConsulta.AsEnumerable()
                                                                                                                    .Sum(row => row.Field<int>("TotalIten"));

                                                                                                                decimal TotalIva = productosConsulta.AsEnumerable()
                                                                                                                         .Sum(row => row.Field<decimal>("TotalIva"));

                                                                                                                decimal SubTotal = productosConsulta.AsEnumerable()
                                                                                                                         .Sum(row => row.Field<decimal>("SubTotal"));

                                                                                                                int cantidad = productosConsulta.AsEnumerable()
                                                                                                                  .Sum(row => row.Field<int>("Cantidad"));

                                                                                                                int Obsequio = productosConsulta.AsEnumerable()
                                                                                                                  .Sum(row => row.Field<int>("Obsequios"));

                                                                                                                decimal precio = productosConsulta.AsEnumerable()
                                                                                                                        .Sum(row => row.Field<decimal>("precio"));

                                                                                                                if (sumaPrecio <= decimal.Parse(Permisos.Field<string>("saldo")) | (Permisos.Field<string>("PermisoCupo") == "CUP"))
                                                                                                                {


                                                                                                                    string TipDoc = "PED";
                                                                                                                    int Pedido = 0; //consecutivo para factura
                                                                                                                    string IdCia = Cia;
                                                                                                                    string Fecha = FechaPedido.ToString("yyyy-MM-dd  HH:mm:ss");
                                                                                                                    string FechaVence = FechaPedido.AddDays(DiasEntrega).ToString("yyyy-MM-dd HH:mm:ss");
                                                                                                                    string IdConcepto = "PED";
                                                                                                                    string IdCliente = consulta.Cliente.Documento ?? "";
                                                                                                                    string IdAgencia = agencia;
                                                                                                                    string IdClieFact = consulta.Cliente.Documento ?? "";
                                                                                                                    decimal VrSubTotal = SubTotal;
                                                                                                                    decimal VrDescuento = SubTotal - precio;
                                                                                                                    decimal VrImpuesto = TotalIva;
                                                                                                                    float VrFletes = 0;
                                                                                                                    float VrOtros = 0;
                                                                                                                    float VrCargos = 0;
                                                                                                                    float VrOtrDcto = 0;
                                                                                                                    float VrSobretasa = 0;
                                                                                                                    float VrImpGlobal = 0;
                                                                                                                    decimal VrNeto = sumaPrecio;
                                                                                                                    int Cantidad = cantidad;
                                                                                                                    string IdVend = IdVendedor;
                                                                                                                    decimal TarifaCom = ValorTarifaVendedor;
                                                                                                                    string CodTarCom = TarifaVendedor;
                                                                                                                    string DirEnvio = consulta.Cliente.Direccion ?? "";
                                                                                                                    string IdLocEnv = consulta.Cliente.Municipio ?? "";
                                                                                                                    string LugarEnvio = Permisos.Field<string>("Localidad");
                                                                                                                    int DiasEntraga = DiasEntrega;
                                                                                                                    string NitContac = NitContacto;
                                                                                                                    string NomContac = NombreContacto;
                                                                                                                    string TelContac = TelefonoContacto;
                                                                                                                    string emlContac = EmailContacto;
                                                                                                                    string CargoContac = CargoContacto;
                                                                                                                    string IdForma = FormaDePago;
                                                                                                                    string DetallePago = "";
                                                                                                                    bool MulPlazos = false;
                                                                                                                    string IdPlazo = Plazo;
                                                                                                                    string CdMney = Permisos.Field<string>("CodMon");
                                                                                                                    string NitEmpTrans = "0";
                                                                                                                    string EmpTrans = "";
                                                                                                                    bool AsignarVeh = false;
                                                                                                                    string pVehiculo = "0";
                                                                                                                    string CdConductor = "0";
                                                                                                                    string CdRuta = Ruta;
                                                                                                                    int ListaPrec = 0;
                                                                                                                    string RefPedido = "CONTADO"; //falta
                                                                                                                    string Modalidad = "INVENTARIO";
                                                                                                                    string Vigencia = "NORMAL";
                                                                                                                    int NumAutoriza = 0;
                                                                                                                    int NumAutCupo = 0;
                                                                                                                    int NumAutCheq = 0;
                                                                                                                    int NumAprob = 0;
                                                                                                                    string IdCiaApr = Cia;
                                                                                                                    string FecAprob = ""; //falta
                                                                                                                    string DetalleAprob = "APROBADO WS"; //falta
                                                                                                                    string CdUsuAprob = usuario;
                                                                                                                    string TipFac = "0"; //falta
                                                                                                                    int Factura = 0; //falta
                                                                                                                    string IdCiaFac = "00"; //falta
                                                                                                                    string FechaFact = DBNull.Value.ToString(); //falta
                                                                                                                    string TipRem = "0"; //falta
                                                                                                                    int Remision = 0; //falta
                                                                                                                    string IdCiaRem = "00"; //falta
                                                                                                                    string FechaRem = DBNull.Value.ToString(); //falta
                                                                                                                    int NumCotizac = 0;
                                                                                                                    string CdCiaCotizac = compania;
                                                                                                                    string OrigenAdd = "WS";
                                                                                                                    int ZonaFrontera = 0;
                                                                                                                    int TipoTrans = 0;
                                                                                                                    string TipoOrden = ""; //falta
                                                                                                                    string TipoModifica = "";
                                                                                                                    bool Anulado = false;
                                                                                                                    string FecDev = DBNull.Value.ToString(); //falta
                                                                                                                    string Observacion = "Generado interfaz Web Services Incko"; //falta
                                                                                                                    string IdEstado = consulta.DatosDelPedido.pmEstadoPedido;
                                                                                                                    string TimeSys = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"); //falta
                                                                                                                    string FecUpdate = DBNull.Value.ToString(); //falta
                                                                                                                    string IdCiaCrea = compania;
                                                                                                                    string IdUsuario = usuario;
                                                                                                                    string NumAutSicom = "";
                                                                                                                    float VrImpCarbono = 0;
                                                                                                                    float BaseIvaIgp = 0;
                                                                                                                    float VrIvaIngProd = 0;



                                                                                                                    //realizamos la insercion en la base de datos 
                                                                                                                    DataSet TablaInsert = new DataSet();
                                                                                                                    List<SqlParameter> parametro = new List<SqlParameter>();

                                                                                                                    parametro.Add(new SqlParameter("@TipDoc", TipDoc));
                                                                                                                    parametro.Add(new SqlParameter("@Pedido", Pedido));
                                                                                                                    parametro.Add(new SqlParameter("@IdCia", IdCia));
                                                                                                                    parametro.Add(new SqlParameter("@Fecha", Fecha));
                                                                                                                    parametro.Add(new SqlParameter("@FechaVence", FechaVence));
                                                                                                                    parametro.Add(new SqlParameter("@IdConcepto", IdConcepto));
                                                                                                                    parametro.Add(new SqlParameter("@IdCliente", IdCliente));
                                                                                                                    parametro.Add(new SqlParameter("@IdAgencia", IdAgencia));
                                                                                                                    parametro.Add(new SqlParameter("@IdClieFact", IdClieFact));
                                                                                                                    parametro.Add(new SqlParameter("@VrSubTotal", VrSubTotal));
                                                                                                                    parametro.Add(new SqlParameter("@VrDescuento", VrDescuento));
                                                                                                                    parametro.Add(new SqlParameter("@VrImpuesto", VrImpuesto));
                                                                                                                    parametro.Add(new SqlParameter("@VrFletes", VrFletes));
                                                                                                                    parametro.Add(new SqlParameter("@VrOtros", VrOtros));
                                                                                                                    parametro.Add(new SqlParameter("@VrCargos", VrCargos));
                                                                                                                    parametro.Add(new SqlParameter("@VrOtrDcto", VrOtrDcto));
                                                                                                                    parametro.Add(new SqlParameter("@VrSobretasa", VrSobretasa));
                                                                                                                    parametro.Add(new SqlParameter("@VrImpGlobal", VrImpGlobal));
                                                                                                                    parametro.Add(new SqlParameter("@VrNeto", VrNeto));
                                                                                                                    parametro.Add(new SqlParameter("@Cantidad", Cantidad));
                                                                                                                    parametro.Add(new SqlParameter("@IdVend", IdVend));
                                                                                                                    parametro.Add(new SqlParameter("@TarifaCom", TarifaCom));
                                                                                                                    parametro.Add(new SqlParameter("@CodTarCom", CodTarCom));
                                                                                                                    parametro.Add(new SqlParameter("@DirEnvio", DirEnvio));
                                                                                                                    parametro.Add(new SqlParameter("@IdLocEnv", IdLocEnv));
                                                                                                                    parametro.Add(new SqlParameter("@LugarEnvio", LugarEnvio));
                                                                                                                    parametro.Add(new SqlParameter("@DiasEntraga", DiasEntraga));
                                                                                                                    parametro.Add(new SqlParameter("@NitContac", NitContac));
                                                                                                                    parametro.Add(new SqlParameter("@NomContac", NomContac));
                                                                                                                    parametro.Add(new SqlParameter("@TelContac", TelContac));
                                                                                                                    parametro.Add(new SqlParameter("@emlContac", emlContac));
                                                                                                                    parametro.Add(new SqlParameter("@CargoContac", CargoContac));
                                                                                                                    parametro.Add(new SqlParameter("@IdForma", IdForma));
                                                                                                                    parametro.Add(new SqlParameter("@DetallePago", DetallePago));
                                                                                                                    parametro.Add(new SqlParameter("@MulPlazos", MulPlazos));
                                                                                                                    parametro.Add(new SqlParameter("@IdPlazo", IdPlazo));
                                                                                                                    parametro.Add(new SqlParameter("@CdMney", CdMney));
                                                                                                                    parametro.Add(new SqlParameter("@NitEmpTrans", NitEmpTrans));
                                                                                                                    parametro.Add(new SqlParameter("@EmpTrans", EmpTrans));
                                                                                                                    parametro.Add(new SqlParameter("@AsignarVeh", AsignarVeh));
                                                                                                                    parametro.Add(new SqlParameter("@pVehiculo", pVehiculo));
                                                                                                                    parametro.Add(new SqlParameter("@CdConductor", CdConductor));
                                                                                                                    parametro.Add(new SqlParameter("@CdRuta", CdRuta));
                                                                                                                    parametro.Add(new SqlParameter("@ListaPrec", ListaPrec));
                                                                                                                    parametro.Add(new SqlParameter("@RefPedido", RefPedido));
                                                                                                                    parametro.Add(new SqlParameter("@Modalidad", Modalidad));
                                                                                                                    parametro.Add(new SqlParameter("@Vigencia", Vigencia));
                                                                                                                    parametro.Add(new SqlParameter("@NumAutoriza", NumAutoriza));
                                                                                                                    parametro.Add(new SqlParameter("@NumAutCupo", NumAutCupo));
                                                                                                                    parametro.Add(new SqlParameter("@NumAutCheq", NumAutCheq));
                                                                                                                    parametro.Add(new SqlParameter("@NumAprob", NumAprob));
                                                                                                                    parametro.Add(new SqlParameter("@IdCiaApr", IdCiaApr));
                                                                                                                    parametro.Add(new SqlParameter("@FecAprob", FecAprob));
                                                                                                                    parametro.Add(new SqlParameter("@DetalleAprob", DetalleAprob));
                                                                                                                    parametro.Add(new SqlParameter("@CdUsuAprob", CdUsuAprob));
                                                                                                                    parametro.Add(new SqlParameter("@TipFac", TipFac));
                                                                                                                    parametro.Add(new SqlParameter("@Factura", Factura));
                                                                                                                    parametro.Add(new SqlParameter("@IdCiaFac", IdCiaFac));
                                                                                                                    parametro.Add(new SqlParameter("@FechaFact", FechaFact));
                                                                                                                    parametro.Add(new SqlParameter("@TipRem", TipRem));
                                                                                                                    parametro.Add(new SqlParameter("@Remision", Remision));
                                                                                                                    parametro.Add(new SqlParameter("@IdCiaRem", IdCiaRem));
                                                                                                                    parametro.Add(new SqlParameter("@FechaRem", FechaRem));
                                                                                                                    parametro.Add(new SqlParameter("@NumCotizac", NumCotizac));
                                                                                                                    parametro.Add(new SqlParameter("@CdCiaCotizac", CdCiaCotizac));
                                                                                                                    parametro.Add(new SqlParameter("@OrigenAdd", OrigenAdd));
                                                                                                                    parametro.Add(new SqlParameter("@ZonaFrontera", ZonaFrontera));
                                                                                                                    parametro.Add(new SqlParameter("@TipoTrans", TipoTrans));
                                                                                                                    parametro.Add(new SqlParameter("@TipoOrden", TipoOrden));
                                                                                                                    parametro.Add(new SqlParameter("@TipoModifica", TipoModifica));
                                                                                                                    parametro.Add(new SqlParameter("@Anulado", Anulado));
                                                                                                                    parametro.Add(new SqlParameter("@FecDev", FecDev));
                                                                                                                    parametro.Add(new SqlParameter("@Observacion", Observacion));
                                                                                                                    parametro.Add(new SqlParameter("@IdEstado", IdEstado));
                                                                                                                    parametro.Add(new SqlParameter("@TimeSys", TimeSys));
                                                                                                                    parametro.Add(new SqlParameter("@FecUpdate", FecUpdate));
                                                                                                                    parametro.Add(new SqlParameter("@IdCiaCrea", IdCiaCrea));
                                                                                                                    parametro.Add(new SqlParameter("@IdUsuario", IdUsuario));
                                                                                                                    parametro.Add(new SqlParameter("@NumAutSicom", NumAutSicom));
                                                                                                                    parametro.Add(new SqlParameter("@VrImpCarbono", VrImpCarbono));
                                                                                                                    parametro.Add(new SqlParameter("@BaseIvaIgp", BaseIvaIgp));
                                                                                                                    parametro.Add(new SqlParameter("@VrIvaIngProd", VrIvaIngProd));



                                                                                                                    if (ClassConexion.ejecutarQuery("WSPedido_ConsAgregarPedido", parametro, out TablaInsert, out string[] nuevoMennsajes, CommandType.StoredProcedure))
                                                                                                                    {
                                                                                                                        bool guardarProd = true;

                                                                                                                        Pedido = TablaInsert.Tables[0].Rows[0].Field<int>("Pedido");
                                                                                                                        int iten = 1;
                                                                                                                        foreach (DataRow dr in productosConsulta.Rows)
                                                                                                                        {

                                                                                                                            int ItemProducto = iten;
                                                                                                                            string IdProducto = dr.Field<string>("IdProducto");

                                                                                                                            string IdBodega = dr.Field<string>("Bodega");
                                                                                                                            string CdTanque = dr.Field<string>("Bodega"); ;

                                                                                                                            float Salidas = dr.Field<int>("TotalIten");
                                                                                                                            string IdUnd = dr.Field<string>("presentacion");
                                                                                                                            decimal VrUnitario = dr.Field<decimal?>("ValorUnitario") ?? 0m;
                                                                                                                            decimal VrPrecio = dr.Field<decimal?>("SubTotal") ?? 0m;
                                                                                                                            decimal VrCostProm = 0;
                                                                                                                            decimal TarifaIva = dr.Field<decimal?>("IVA") ?? 0m;

                                                                                                                            decimal VrIvaSal = dr.Field<decimal?>("TotalIva") ?? 0m;
                                                                                                                            decimal TarifaDct = dr.Field<decimal?>("descuento") ?? 0m;

                                                                                                                            decimal VrDctoSal = dr.Field<decimal?>("SubTotal") ?? 0m;

                                                                                                                            decimal VrCostoSal = dr.Field<int>("Cantidad");

                                                                                                                            decimal VrBruto = dr.Field<decimal>("PrecioTotalConIva");

                                                                                                                            string CdCCosto = consulta.Cliente.CentroCosto;
                                                                                                                            string CdSubCos = consulta.Cliente.SubCCosto;
                                                                                                                            string CdLocal = Permisos.Field<string>("IDLocal");
                                                                                                                            string CdSzona = Permisos.Field<string>("SZona");

                                                                                                                            decimal Comision = ValorTarifaVendedor;

                                                                                                                            string Referencia = "";
                                                                                                                            if (dr.Field<int>("Obsequios") > 0 & dr.Field<int>("Cantidad") == 0)
                                                                                                                            {
                                                                                                                                Referencia = "Producto Obsequio";
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Referencia = "Producto";
                                                                                                                            }
                                                                                                                            string Descripcion = dr.Field<string>("DescripProd");

                                                                                                                            bool EsCombo = false;
                                                                                                                            if (dr.Field<int>("Obsequios") > 0 & dr.Field<int>("Cantidad") > 0)
                                                                                                                            {
                                                                                                                                EsCombo = true;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {

                                                                                                                                EsCombo = false;
                                                                                                                            }

                                                                                                                            string CdMoneda = Permisos.Field<string>("CodMon");

                                                                                                                            string CodTarDct = dr.Field<string>("descuento") ?? "";
                                                                                                                            string CodTarIva = dr.Field<string>("CodigoIVA");
                                                                                                                            int ListaPrecc = dr.Field<int>("ListaDePrecio");
                                                                                                                            decimal VrBase = dr.Field<decimal>("TotalIva");


                                                                                                                            int CantObseq = dr.Field<int>("Obsequios");
                                                                                                                            decimal VrIvaObseq = dr.Field<decimal>("TotalIva");

                                                                                                                            //realizamos la insercion en la base de datos de productos 
                                                                                                                            DataSet TablaProductos = new DataSet();
                                                                                                                            List<SqlParameter> parametroProduc = new List<SqlParameter>();


                                                                                                                            parametroProduc.Add(new SqlParameter("@TipDoc", TipDoc));
                                                                                                                            parametroProduc.Add(new SqlParameter("@Pedido", Pedido));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdCia", IdCia));
                                                                                                                            parametroProduc.Add(new SqlParameter("@ItemProducto", ItemProducto));
                                                                                                                            parametroProduc.Add(new SqlParameter("@Fecha", Fecha));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdProducto", IdProducto));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdBodega", IdBodega));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CdTanque", CdTanque));
                                                                                                                            parametroProduc.Add(new SqlParameter("@Salidas", Salidas));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdUnd", IdUnd));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrUnitario", VrUnitario));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrPrecio", VrPrecio));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrCostProm", VrCostProm));
                                                                                                                            parametroProduc.Add(new SqlParameter("@TarifaIva", TarifaIva));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrIvaSal", VrIvaSal));
                                                                                                                            parametroProduc.Add(new SqlParameter("@TarifaDct", TarifaDct));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrDctoSal", VrDctoSal));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrCostoSal", VrCostoSal));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrBruto", VrBruto));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdCliente", IdCliente));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdAgencia", IdAgencia));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CdCCosto", CdCCosto));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CdSubCos", CdSubCos));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CdLocal", CdLocal));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CdSzona", CdSzona));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdVend", IdVend));
                                                                                                                            parametroProduc.Add(new SqlParameter("@Comision", Comision));
                                                                                                                            parametroProduc.Add(new SqlParameter("@Referencia", Referencia));
                                                                                                                            parametroProduc.Add(new SqlParameter("@Descripcion", Descripcion));
                                                                                                                            parametroProduc.Add(new SqlParameter("@EsCombo", EsCombo));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CodTarDct", CodTarDct));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CodTarIva", CodTarIva));
                                                                                                                            parametroProduc.Add(new SqlParameter("@ListaPrecc", ListaPrecc));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrBase", VrBase));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CdMoneda", CdMoneda));
                                                                                                                            parametroProduc.Add(new SqlParameter("@TimeSys", TimeSys));
                                                                                                                            parametroProduc.Add(new SqlParameter("@IdUsuario", IdUsuario));
                                                                                                                            parametroProduc.Add(new SqlParameter("@CantObseq", CantObseq));
                                                                                                                            parametroProduc.Add(new SqlParameter("@VrIvaObseq", VrIvaObseq));


                                                                                                                            if (ClassConexion.ejecutarQuery("WSPedido_ConsAgregarProductos", parametroProduc, out TablaProductos, out string[] nuevoMennsajeprod, CommandType.StoredProcedure))
                                                                                                                            {

                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                Codigo = nuevoMennsajeprod[0];
                                                                                                                                Mensaje = nuevoMennsajeprod[1];
                                                                                                                                guardarProd = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                            iten++;


                                                                                                                        }

                                                                                                                        if (guardarProd)
                                                                                                                        {
                                                                                                                            respuesta.IdCia = IdCia;
                                                                                                                            respuesta.TipoDoc = TipDoc;
                                                                                                                            respuesta.CdAgencia = IdAgencia;
                                                                                                                            respuesta.Fecha = Fecha;
                                                                                                                            Codigo = "066";
                                                                                                                            Mensaje = "Se ha registrado el pedido exitosamente con numero de factura " + Pedido;

                                                                                                                        }
                                                                                                                        else
                                                                                                                        {

                                                                                                                        }

                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        Codigo = nuevoMennsajes[0];
                                                                                                                        Mensaje = nuevoMennsajes[1];
                                                                                                                    }

                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    Codigo = "052";
                                                                                                                    Mensaje = "El usuario no tiene permiso para Superar el cupo del cliente";
                                                                                                                }
                                                                                                            }


                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            Codigo = "037";
                                                                                                            Mensaje = "No se han encontrado los productos";
                                                                                                        }




                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Codigo = "038";
                                                                                                    Mensaje = "El usuario no tiene permiso CIA para registrar el pedido en otra compañia";
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                    }

                                                                                }

                                                                            }
                                                                            else
                                                                            {
                                                                                Codigo = "022";
                                                                                Mensaje = "No tiene el usuario el permiso EGA para ingresar una fecha diferente a la establecida";
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Codigo = "045";
                                                                        Mensaje = "El codigo de pmTarifaComision no existe en syscom";
                                                                    }
                                                                }


                                                            }
                                                            else
                                                            {
                                                                Codigo = "022";
                                                                Mensaje = "El cliente se encuentra en mora y el usuario no tiene el permiso MOR";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Codigo = "055";
                                                            Mensaje = "El codigo de plazo ingresado no existe";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Codigo = "074";
                                                        Mensaje = "El Usuario no tiene permiso para cambiar el estado del pedido";
                                                    }
                                                }
                                                else
                                                {
                                                    Codigo = "054";
                                                    Mensaje = "La Ruta ingresada no existe";
                                                }
                                            }
                                            else
                                            {
                                                Codigo = "053";
                                                Mensaje = "El codigo ingresado de la localidad no existe";
                                            }
                                        }
                                        else
                                        {
                                            Codigo = "021";
                                            Mensaje = "El cliente no puede recibir pedidos de la compañia asiganada del usuario";
                                        }

                                    }
                                    else
                                    {
                                        if (Permisos.Field<string>("Cliente") == "Noexiste")
                                        {
                                            Codigo = "019";
                                            Mensaje = "El cliente no existe";
                                        }
                                        else
                                        {
                                            Codigo = "020";
                                            Mensaje = "El cliente se encuentra bloquedao y el vendedor no tiene el permiso BLO";
                                        }
                                    }

                                }
                                else
                                {
                                    Codigo = "018";
                                    Mensaje = "El usuario no tiene permisos para hacer pedidos";
                                }
                            }
                            else
                            {
                                Codigo = "017";
                                Mensaje = "El Usuario no pertenece al grupo de usuarios avanzados";
                            }
                        }
                        else
                        {
                            Codigo = "016";
                            Mensaje = "El Usuario no esta marcado como vendedor";
                        }


                    }
                    else
                    {
                        Codigo = "015";
                        Mensaje = "El Usuario no esta regiostrado como vendedor";
                    }

                }
                else
                {
                    Codigo = nuevoMennsaje[0];
                    Mensaje = nuevoMennsaje[1];
                }
                respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje };
            }
            else
            {
                respuesta.Registro = new Log { Codigo = nuevoMensaje[0], Descripcion = nuevoMensaje[1] };
            }

            return respuesta;
        }
        #endregion

        #region InfoMaestra
        public ResInfoMaestra resInfoMaestra(InfoMaestra Info)
        {
            ResInfoMaestra respuesta = new ResInfoMaestra();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            string Codigo = "";
            string Mensaje = "";
            if (token.VerificarToken(Info.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                //verificacion que ingrese el parametro establecido             
                if (Info.TipoRegistro > 0 & Info.TipoRegistro < 11)
                {

                    //realizamos el consulta de los permisos
                    ConexionBD ClassConexion = new ConexionBD();
                    ConexionSQLite conSqlite = new ConexionSQLite("");
                    string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                    ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                    SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                    DataSet TablaInfo = new DataSet();
                    List<SqlParameter> parametros = new List<SqlParameter>();
                    parametros.Add(new SqlParameter("@TipoRegistro", Info.TipoRegistro));


                    if (ClassConexion.ejecutarQuery("WSPedido_consInfoMaestra", parametros, out TablaInfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                    {
                        if (TablaInfo.Tables[0].Rows.Count > 0)
                        {
                            DataTable dataTable = TablaInfo.Tables[0];

                            //Convierte el DataTable en una lista de diccionarios
                            List<Dictionary<string, object>> tableList = new List<Dictionary<string, object>>();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                Dictionary<string, object> rowDict = new Dictionary<string, object>();
                                foreach (DataColumn col in dataTable.Columns)
                                {
                                    rowDict[col.ColumnName] = row[col];
                                }
                                tableList.Add(rowDict);
                            }

                            respuesta.tableList = tableList;
                            Codigo = "058";
                            Mensaje = "Se ejecutó correctamente la consulta.";


                        }
                        else
                        {
                            Codigo = "057";
                            Mensaje = "Error Con la conexion de la base de datos";
                        }
                    }
                    else
                    {
                        Codigo = nuevoMennsaje[0];
                        Mensaje = nuevoMennsaje[1];
                    }

                }
                else
                {
                    Codigo = "056";
                    Mensaje = "El Tipo de registro no esta en el parametro estrablecido";
                }
                respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje };
            }
            else
            {
                respuesta.Registro = new Log { Codigo = nuevoMensaje[0], Descripcion = nuevoMensaje[1] };
            }

            return respuesta;
        }
        #endregion

        #region Obtener Cartera Total
        public ResObtenerCarteraTotal resObtCartTotal(ObtCarTotal Info)
        {
            ResObtenerCarteraTotal respuesta = new ResObtenerCarteraTotal();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            string Codigo = "";
            string Mensaje = "";
            if (token.VerificarToken(Info.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                string cliente = "";
                if (!string.IsNullOrEmpty(Info.IdCliente))
                {
                    cliente = Info.IdCliente;
                }

                ConexionBD ClassConexion = new ConexionBD();
                ConexionSQLite conSqlite = new ConexionSQLite("");
                string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                DataSet TablaInfo = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Cliente", cliente));


                if (ClassConexion.ejecutarQuery("WSPedido_consObtenerCarteraTotal", parametros, out TablaInfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    if (TablaInfo.Tables[0].Rows.Count > 0)
                    {
                        DataTable dataTable = TablaInfo.Tables[0];

                        List<ResCarteraTotal> listaObtclien = new List<ResCarteraTotal>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            ResCarteraTotal tabla = new ResCarteraTotal
                            {
                                Cliente = row["Cliente"].ToString(), // Ajusta el nombre de la columna si es diferente
                                SaldoCartera = Convert.ToInt32(row["SaldoCartera"]), // Ajusta el nombre de la columna si es diferente
                                                                                     // Agrega otras propiedades aquí según tu modelo de datos
                            };

                            listaObtclien.Add(tabla);
                        }
                        respuesta.DatosCartera = listaObtclien;
                        Codigo = "060";
                        Mensaje = "Se ejecutó correctamente la consulta.";
                    }
                    else
                    {
                        Codigo = "059";
                        Mensaje = "¡No hay clientes registrados o con Deuda!";
                    }
                    
                }
                else
                {
                    Codigo = nuevoMensaje[0];
                    Mensaje = nuevoMensaje[1];
                }
            }
            else
            {
                Codigo = nuevoMensaje[0];
                Mensaje = nuevoMensaje[1] ;
            }
            respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje };
            return respuesta;
        }
        #endregion

        #region Obtener Cartera Total Def
        public ResObtenerCarteraTotalDef resObtCartTotalDef(ObtCarTotalDef Info)
        {
            ResObtenerCarteraTotalDef respuesta = new ResObtenerCarteraTotalDef();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            string Codigo = "";
            string Mensaje = "";
            if (token.VerificarToken(Info.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                string cliente = "";
                if (!string.IsNullOrEmpty(Info.IdCliente))
                {
                    cliente = Info.IdCliente;
                }


                DateTime fechaInicial;

                if (DateTime.TryParse(Info.FechaInicial, out fechaInicial))
                {
                    DateTime fechaFinal;
                    if (DateTime.TryParse(Info.FechaFinal, out fechaFinal))
                    {
                        int pagina = 1;
                        int inicio = 0;
                        int fin = 10;
                        int total = 0;
                        int resPagina = 10;

                        if (Info.pagina.Pagina > 0)
                        {
                            pagina = Info.pagina.Pagina;
                        }
                        if (Info.pagina.NumResgitroPagina > 0)
                        {
                            resPagina = Info.pagina.NumResgitroPagina;
                            fin = Info.pagina.NumResgitroPagina * pagina;
                            inicio = (fin - Info.pagina.NumResgitroPagina) + 1;
                        }

                        ConexionBD ClassConexion = new ConexionBD();
                        ConexionSQLite conSqlite = new ConexionSQLite("");
                        string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                        ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                        SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                        DataSet TablaInfo = new DataSet();
                        List<SqlParameter> parametros = new List<SqlParameter>();                     
                        parametros.Add(new SqlParameter("@Cliente", cliente));
                        parametros.Add(new SqlParameter("@fechaInicial", fechaInicial.ToString("yyyyMMdd")));
                        parametros.Add(new SqlParameter("@fechaFinal", fechaFinal.ToString("yyyyMMdd")));
                        parametros.Add(new SqlParameter("@Inicio", inicio));
                        parametros.Add(new SqlParameter("@Fin", fin));


                        if (ClassConexion.ejecutarQuery("WSPedido_consObtenerCarteraTotalDEF", parametros, out TablaInfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                        {
                            if (TablaInfo.Tables[0].AsEnumerable().FirstOrDefault().Field<int>("TotalFilas") > 0)
                            {
                                total = TablaInfo.Tables[0].AsEnumerable().FirstOrDefault().Field<int>("TotalFilas");
                                DataTable dataTable = TablaInfo.Tables[1];

                                if (total > inicio)
                                {
                                    
                                    List<itemCartera> listaObtclien = new List<itemCartera>();

                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        itemCartera tabla = new itemCartera
                                        {
                                            TipoDocumento = row["TipoDocumento"].ToString(), // Ajusta el nombre de la columna si es diferente
                                            Documento = Convert.ToInt32(row["Tercero"]), // Ajusta el nombre de la columna si es diferente
                                            Compañia = row["Compañia"].ToString(),
                                            Vencimiento = Convert.ToInt32(row["Vencimiento"]),
                                            FechaEmision = Convert.ToDateTime(row["FechaEmision"]),
                                            FechaVencimiento = Convert.ToDateTime(row["FechaVencimiento"]),
                                            ValorTotal = Convert.ToInt32(row["ValorTotal"]),
                                            Abono = Convert.ToInt32(row["Abono"]),
                                            Saldo = Convert.ToInt32(row["Saldo"]),
                                        };

                                        listaObtclien.Add(tabla);
                                    }
                                    respuesta.DatosCartera = listaObtclien;

                                    Codigo = "064";
                                    Mensaje = "Se ejecutó correctamente la consulta.";
                                    respuesta.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };
                                    
                                }
                                else
                                {
                                    Codigo = "063";
                                    Mensaje = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)total / resPagina);
                                    respuesta.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };

                                }

                            }
                            else
                            {
                                Codigo = "065";
                                Mensaje = "No se encontraron registros en la base de datos";
                              
                            }

                            return respuesta;
                        }
                        else
                        {
                            Codigo = nuevoMensaje[0];
                            Mensaje = nuevoMensaje[1];

                        }
                    }
                    else
                    {

                        Codigo = "062";
                        Mensaje = "El formato de fecha ingresado en FechaFinal no es valido";
                    }

                }
                else
                {
                    Codigo = "061";
                    Mensaje = "El formato de fecha ingresado en FechaInicial no es valido";
                }

            }
            else
            {
                Codigo = nuevoMensaje[0];
                Mensaje = nuevoMensaje[1];

            }
            respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje };
            return respuesta;
        }
        #endregion

        #region Anular Pedido
        public RestAnulPedido restAnulPedido(AnulPedido Info)
        {
            RestAnulPedido respuesta = new RestAnulPedido();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            string Codigo = "";
            string Mensaje = "";
            if (token.VerificarToken(Info.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario, out string compania))
            {
                if (string.IsNullOrEmpty(Info.IdPedido))
                {
                    Codigo = "067";
                    Mensaje = "No puede ser nulo o vacio el parametro de IdPedido.";
                }
                else if (string.IsNullOrEmpty(Info.IdCia))
                {
                    Codigo = "068";
                    Mensaje = "No puede ser nulo o vacio el parametro de IdCia.";
                }
                else
                {
                    //luwego de comprobar todos los parametros se realiza la llamada a cambiar el estado 
                    ConexionBD ClassConexion = new ConexionBD();
                    ConexionSQLite conSqlite = new ConexionSQLite("");
                    string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                    ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                    SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                    DataSet TablaInfo = new DataSet();
                    List<SqlParameter> parametros = new List<SqlParameter>();
                    parametros.Add(new SqlParameter("@Usuario", usuario));
                    parametros.Add(new SqlParameter("@IdPedido", Info.IdPedido));
                    parametros.Add(new SqlParameter("@Idcia", Info.IdCia));



                    if (ClassConexion.ejecutarQuery("WSPedido_ConsAnularPedido", parametros, out TablaInfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                    {
                        DataRow TablaAnular = TablaInfo.Tables[0].Rows[0];

                        //comprobar que tenga habilidato el permiso de vendedor
                        if (TablaAnular.Field<string>("permiso") == "permiso")
                        {
                            if (TablaAnular.Field<string>("factura") == "Existe")
                            {
                                if (TablaAnular.Field<string>("cambioRealizado") == "Exitoso")
                                {
                                    Codigo = "073";
                                    Mensaje = "Se ha anulado el pedido " + Info.IdPedido + " de la compañia "+ Info.IdCia;
                                }
                                else
                                {
                                    Codigo = "072";
                                    Mensaje = "Ha ocurrido un error con la base de datos al actualizar al anular el pedido";
                                }
                            }
                            else
                            {
                                if (TablaAnular.Field<string>("factura") == "desactiva")
                                {
                                    Codigo = "070";
                                    Mensaje = "La factura ya se encuentra anulada";
                                }
                                else
                                {
                                    Codigo = "071";
                                    Mensaje = "La factura no existe o ya cambio de estado activo o radicado";

                                }

                            }
                        }
                        else
                        {
                            Codigo = "069";
                            Mensaje = "El Usuario no tiene permiso para anual el pedido";
                        }
                    }
                    else
                    {
                        Codigo = nuevoMensaje[0];
                        Mensaje = nuevoMensaje[1];

                    }
                }

            }
            else
            {
                Codigo = nuevoMensaje[0];
                Mensaje = nuevoMensaje[1];

            }
            respuesta.Registro = new Log { Codigo = Codigo, Descripcion = Mensaje };
            return respuesta;
        }
        #endregion

    }
}
