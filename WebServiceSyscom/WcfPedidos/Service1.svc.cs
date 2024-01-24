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

        public static object lockObject = new object();

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
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
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
                if (consulta.Pagina.NumRegistroPagina > 0)
                {
                    ResPorPagina = consulta.Pagina.NumRegistroPagina;
                    fin = ResPorPagina;
                }
                if (consulta.Pagina.Pagina > 0)
                {
                    NumPagina = consulta.Pagina.Pagina;
                    fin = ResPorPagina * NumPagina;
                    inicio = (fin - ResPorPagina) + 1;
                }

                if (!(consulta.NitCliente == null || String.IsNullOrWhiteSpace(consulta.NitCliente)))
                {
                    cliente = consulta.NitCliente;
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
                Pedido pedido = new Pedido();
                respuesta =  pedido.GenerarPedido(consulta, usuario, compania);

                //Mensaje de exito es 066 y de error con el proceso es 076
                if (respuesta.Registro.Codigo == "066")
                {
                    LogErrores.tareas.Add(respuesta.Registro.Descripcion + "-" + respuesta.IdCia + " Cliente: " + consulta.Cliente.Documento + " Agencia:" + respuesta.CdAgencia);
                    LogErrores.write();
                }
                else
                {
                    //metodo para serializacion 
                    var serializerSettings = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        // Otras configuraciones si es necesario
                    };

                    LogErrores.tareas.Add("============================================================================");
                    LogErrores.tareas.Add(JsonConvert.SerializeObject(consulta, Formatting.Indented, serializerSettings));
                    LogErrores.tareas.Add(JsonConvert.SerializeObject(respuesta, Formatting.Indented, serializerSettings));
                    LogErrores.write();
                }
                
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
                            Mensaje = "Error Con la conexión de la base de datos";
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
                    Mensaje = "El Tipo de registro no está en el parámetro establecido";
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
                        Mensaje = "¡No hay clientes registrados o con deudas!";
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
                        if (Info.pagina.NumRegistroPagina > 0)
                        {
                            resPagina = Info.pagina.NumRegistroPagina;
                            fin = Info.pagina.NumRegistroPagina * pagina;
                            inicio = (fin - Info.pagina.NumRegistroPagina) + 1;
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
                        Mensaje = "El formato de fecha ingresado en FechaFinal no es válido";
                    }

                }
                else
                {
                    Codigo = "061";
                    Mensaje = "El formato de fecha ingresado en FechaInicial no es válido";
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
                    Mensaje = "No puede ser nulo o vacío el parámetro de IdPedido.";
                }
                else if (string.IsNullOrEmpty(Info.IdCia))
                {
                    Codigo = "068";
                    Mensaje = "No puede ser nulo o vacío el parámetro de IdCia.";
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
                                    Mensaje = "Se ha anulado el pedido " + Info.IdPedido + " de la compañía " + Info.IdCia;
                                }
                                else
                                {
                                    Codigo = "072";
                                    Mensaje = "Ha ocurrido un error con la base de datos al actualizar al anulación el pedido";
                                }
                            }
                            else
                            {
                                if (TablaAnular.Field<string>("factura") == "desactiva")
                                {
                                    Codigo = "070";
                                    Mensaje = "El pedido ya se encuentra anulada";
                                }
                                else
                                {
                                    Codigo = "071";
                                    Mensaje = "El pedido no existe o ya cambio de estado activo a radicado";

                                }

                            }
                        }
                        else
                        {
                            Codigo = "069";
                            Mensaje = "El Usuario no tiene permiso para anular el pedido";
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
