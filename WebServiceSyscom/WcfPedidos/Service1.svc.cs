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
        

        public RespProducto ConProducto(ObtProducto obtProducto)
        {
            RespProducto respuesta = new RespProducto();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            if (token.VerificarToken(obtProducto.token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario))
            {

                Producto p = new Producto();
                respuesta = p.GetProducto(obtProducto, out string[] nuevomensaje);
                respuesta.Registro = new Log { Codigo = nuevomensaje[0], Descripcion = nuevomensaje[1] };
            }
            else
            {
                respuesta.Registro = new Log { Codigo = nuevoMensaje[0], Descripcion = nuevoMensaje[1] };
            }
            return respuesta;

        }

        #region Obtener Productos 
        /// <summary>
        /// Método para obtener información de productos a partir de un token y una fecha especificada.
        /// </summary>
        /// <param name="obtenerConFecha">Objeto que contiene el token de seguridad y la fecha para la consulta.</param>
        /// <returns>Objeto RespProductos que contiene la información de productos y un registro de eventos.</returns>
        public RespProductos GetProductos(ObtInfoGeneral consulta)
        {
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            RespProductos productos = new RespProductos();
            productos.Registro = new Log();
            productos.Registro = null;
            GenerarToken token = new GenerarToken();

            if (token.VerificarToken(consulta.Token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario))
            {
                List<Producto> prods = new List<Producto>();
                ConexionBD con = new ConexionBD();
                ConexionSQLite conSQLite = new ConexionSQLite("");
                string connectionString = conSQLite.obtenerConexionSyscom().ConnectionString;
                con.setConnection(conSQLite.obtenerConexionSyscom());

                //se realiza la comprobacion para asignar los valores para las paginas
                if (consulta.NumResgitroPagina > 0)
                {
                    ResPorPagina = consulta.NumResgitroPagina;
                    fin = ResPorPagina;
                }
                if (consulta.Pagina > 0)
                {
                    NumPagina = consulta.Pagina;
                    fin = ResPorPagina * NumPagina;
                    inicio = (fin - ResPorPagina) + 1;
                }

                //acceder al procedimiento almacenado
                DataSet TablaClientes = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@IdUsuario", usuario));
                parametros.Add(new SqlParameter("@Inicio", inicio));
                parametros.Add(new SqlParameter("@Fin", fin));

                if (con.ejecutarQuery("WSPedido_consObtCliente", parametros, out TablaClientes, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    ResTotal = Convert.ToInt32(TablaClientes.Tables[0].Rows[0]["TotalFilas"]);
                    if (ResTotal > 0)
                    {
                        if (NumPagina <= (int)Math.Ceiling((double)ResTotal / ResPorPagina))
                        {
                            TablaClientes.Tables[1].AsEnumerable().ToList().ForEach(i =>
                                {
                                    Producto prod = new Producto
                                    {
                                        pmIdProducto = i.Field<string>("IdProducto"),
                                        pmDescripProd = i.Field<string>("DescripProd"),
                                        pmIvaInc = i.Field<string>("IvaInc"),
                                        pmLtPreDef = i.Field<string>("LtPreDef"),

                                        pmVrPrecio1 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("1") ? i.Field<decimal>("VrPrecio1") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio1"),
                                        pmVrPrecio2 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("2") ? i.Field<decimal>("VrPrecio2") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio2"),
                                        pmVrPrecio3 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("3") ? i.Field<decimal>("VrPrecio3") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio3"),
                                        pmVrPrecio4 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("4") ? i.Field<decimal>("VrPrecio4") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio4"),
                                        pmVrPrecio5 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("5") ? i.Field<decimal>("VrPrecio5") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio5"),

                                        pmExcluidoImp = i.Field<bool>("ExcluidoImp"),
                                        pmTarifaIva = i.Field<decimal>("Tarifa")
                                    };
                                    prods.Add(prod);
                                });

                            prods.ForEach(r =>
                            {
                                con.resetQuery();
                                con.qryFields.Add(@"IdCia");
                                con.qryTables.Add(@"ProdCompanias");
                                con.addWhereAND("IdProducto = '" + r.pmIdProducto + "'");
                                con.select();
                                con.ejecutarQuery();
                                r.pmDisponibleEnConpania = con.getDataTable().AsEnumerable().Select(s => s.Field<string>("IdCia")).ToList();
                            });
                            productos.Productos = prods;
                            productos.Registro = new Log { Codigo = "015", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Descripcion = "Se ejecutó correctamente la consulta." };
                        }
                        else
                        {
                            productos.Registro = new Log { Codigo = "034", Descripcion = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)ResTotal / ResPorPagina) };
                        }
                        productos.paginas = new OrganizadorPagina { PaginaActual = NumPagina, NumeroDePaginas = (int)Math.Ceiling((double)ResTotal / ResPorPagina), RegistroPorPagina = ResPorPagina, RegistroTotal = ResTotal };


                    }
                    else
                    {
                        productos.Registro = new Log { Codigo = "035", Descripcion = "No se encuentran Productos disponibles" };
                    }




                }
                else
                {
                    productos.Registro = new Log { Codigo = "036", Descripcion = "No se encuentra la base de datos" };
                }
            }
            else
            {
                productos.Registro = new Log { Codigo = nuevoMensaje[0], Descripcion = nuevoMensaje[1] };
            }

            return productos;
        }
        #endregion

        #region Obtener Clientes
        public RespClientes GetClientes(ObtInfoClientes consulta)
        {
            RespClientes respuesta = new RespClientes();
            respuesta.Registro = new Log();
            GenerarToken token = new GenerarToken();
            if (token.VerificarToken(consulta.Token.Token, NomProyecto, out string[] nuevoMensaje, out string usuario))
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
                            respuesta.Registro = new Log { Codigo = "038", Descripcion = "Se ejecutó correctamente la consulta." };


                            }
                        else
                        {
                            respuesta.Registro = new Log { Codigo = "039", Descripcion = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)ResTotal / ResPorPagina) };

                        }

                    }
                    else
                    {
                        respuesta.Registro = new Log { Codigo = "037", Descripcion = "No se encuentran Clientes disponibles" };

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
        
        
    }
}
