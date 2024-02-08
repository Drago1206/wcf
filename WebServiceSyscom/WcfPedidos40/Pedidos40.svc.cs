using Newtonsoft.Json;
using SyscomUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using WcfPedidos40.Model;
using WcfPedidos40.Models;
using WcfPruebas40.Models;

namespace WcfPedidos40
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Pedidos40 : IPedidos40
    {

        private connect.Conexion con = new connect.Conexion();
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        public RespProducto GetProducto(ProductoReq reqProducto)
        {
            //List<Errores> Errores = new List<Errores>();
            RespProducto rta = new RespProducto();
            rta.Errores = new List<Errores>();
            try
            {
                if (string.IsNullOrWhiteSpace(reqProducto.IdUsuario))
                    rta.Errores.Add(new Errores { coderror = "001", descripcion = "El parámetro 'Id Usuario' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProducto.Contrasena))
                    rta.Errores.Add(new Errores { coderror = "002", descripcion = "El parámetro 'Contraseña' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProducto.IdProducto))
                    rta.Errores.Add(new Errores { coderror = "003", descripcion = "El parámetro 'Id Producto' no puede ser nulo!" });
                if (rta.Errores.Count <= 0)
                {
                    DataTable us = existeUsuario(reqProducto.IdUsuario, reqProducto.Contrasena);
                    if (us == null || us.Rows.Count <= 0)
                        rta.Errores.Add(new Errores { coderror = "004", descripcion = "El usuario '" + reqProducto.IdUsuario + "', NO existe!" });
                    else
                    {
                        pwdSyscom pwd = new pwdSyscom();
                        pwd.Decodificar(us.Rows[0].Field<string>("PwdLog"));
                        if (pwd.contrasenna.Split('=')[2] != reqProducto.Contrasena)
                            rta.Errores.Add(new Errores { coderror = "005", descripcion = "Contraseña o Usuario incorrectos!" });
                        else
                        {
                            ProductosResponse p = new ProductosResponse();
                            rta = p.GetProducto(reqProducto);
                            if (rta == null || rta.Codigo == null)
                            {
                                rta = new RespProducto();
                                rta.Errores = new List<Errores>();
                                rta.Errores.Add(new Errores { coderror = "006", descripcion = "El producto '" + reqProducto.IdProducto + "', NO existe!" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rta.Errores.Add(new Errores { coderror = "100", descripcion = ex.Message });
            }
            return rta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductoTP", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        public RespProducto GetProductoTP(ProductoTPReq reqProdTP)
        {
            //List<Errores> Errores = new List<Errores>();
            RespProducto rta = new RespProducto();
            rta.Errores = new List<Errores>();
            try
            {
                if (string.IsNullOrWhiteSpace(reqProdTP.IdUsuario))
                    rta.Errores.Add(new Errores { coderror = "001", descripcion = "El parámetro 'Id Usuario' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProdTP.Contrasena))
                    rta.Errores.Add(new Errores { coderror = "002", descripcion = "El parámetro 'Contraseña' no puede ser nulo!" });
                if (string.IsNullOrWhiteSpace(reqProdTP.IdProducto))
                    rta.Errores.Add(new Errores { coderror = "003", descripcion = "El parámetro 'Id Producto' no puede ser nulo!" });
                if (rta.Errores.Count <= 0)
                {
                    DataTable us = existeUsuario(reqProdTP.IdUsuario, reqProdTP.Contrasena);
                    if (us == null || us.Rows.Count <= 0)
                        rta.Errores.Add(new Errores { coderror = "004", descripcion = "El usuario '" + reqProdTP.IdUsuario + "', NO existe!" });
                    else
                    {
                        pwdSyscom pwd = new pwdSyscom();
                        pwd.Decodificar(us.Rows[0].Field<string>("PwdLog"));
                        if (pwd.contrasenna.Split('=')[2] != reqProdTP.Contrasena)
                            rta.Errores.Add(new Errores { coderror = "005", descripcion = "Contraseña o Usuario incorrectos!" });
                        else
                        {
                            ProductosResponse p = new ProductosResponse();
                            rta = p.GetProductoTP(reqProdTP);
                            if (rta == null || rta.Codigo == null)
                            {
                                rta = new RespProducto();
                                rta.Errores = new List<Errores>();
                                rta.Errores.Add(new Errores { coderror = "006", descripcion = "El producto '" + reqProdTP.IdProducto + "', NO existe!" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rta.Errores.Add(new Errores { coderror = "100", descripcion = ex.Message });
            }
            return rta;
        }
        public RespProductos GetProductos(Usuario usuario)
        {
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            RespProductos productos = new RespProductos();
            List<ProductosResponse> prods = new List<ProductosResponse>();
            List<SqlParameter> parametros = new List<SqlParameter>();
            DataSet TablaProductos = new DataSet();
            if (existeUsuario(usuario))
            {
                con.setConnection("Syscom");
                DateTime pmFecha_Actual = DateTime.Parse(usuario.Fecha_Act);
                parametros.Add(new SqlParameter("@IdUsuario", usuario.IdUsuario));
                parametros.Add(new SqlParameter("@FechaActual", pmFecha_Actual));
                if (con.ejecutarQuery("WSPedidos40Productos", parametros, out TablaProductos, out string[] mensajeProductos, CommandType.StoredProcedure))
                {
                    if (TablaProductos.Tables[0].Rows.Count > 0)
                    {


                        TablaProductos.Tables[0].AsEnumerable().ToList().ForEach(i =>
                        {
                            ProductosResponse prod = new ProductosResponse
                            {
                                IdProducto = i.Field<string>("IdProducto"),
                                DescripProd = i.Field<string>("DescripProd"),
                                IvaInc = i.Field<string>("IvaInc"),
                                LtPreDef = i.Field<string>("LtPreDef"),
                                VrPrecio1 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("1") ? i.Field<decimal>("VrPrecio1") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio1"),
                                VrPrecio2 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("2") ? i.Field<decimal>("VrPrecio2") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio2"),
                                VrPrecio3 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("3") ? i.Field<decimal>("VrPrecio3") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio3"),
                                VrPrecio4 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("4") ? i.Field<decimal>("VrPrecio4") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio4"),
                                VrPrecio5 = i.Field<string>("IvaInc") != null && i.Field<string>("IvaInc").Contains("5") ? i.Field<decimal>("VrPrecio5") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio5"),
                                ExcluidoImp = i.Field<bool>("ExcluidoImp"),
                                TarifaIva = i.Field<decimal>("Tarifa")
                            };

                            //Jefersón dice que esto no se trae para ecom 16/10/2019
                            //PrecioEspecial TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 1, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio1 = TarEsp.Tarifa.Value;

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 2, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //{
                            //    LogErrores.tareas.Add("Ingreso dos");
                            //    LogErrores.tareas.Add(TarEsp.Numero.ToString());
                            //    LogErrores.write();
                            //    prod.pmVrPrecio2 = TarEsp.Tarifa.Value;
                            //}

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 3, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio3 = TarEsp.Tarifa.Value;

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 4, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio4 = TarEsp.Tarifa.Value;

                            //TarEsp = new PrecioEspecial();
                            //TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 5, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                            //if (TarEsp.Numero != null)
                            //    prod.pmVrPrecio5 = TarEsp.Tarifa.Value;

                            prods.Add(prod);
                        });

                        prods.ForEach(r =>
                        {
                            con.resetQuery();
                            con.qryFields.Add(@"IdCia");
                            con.qryTables.Add(@"ProdCompanias");
                            con.addWhereAND("IdProducto = '" + r.IdProducto + "'");
                            con.select();
                            con.ejecutarQuery();
                            r.DisponibleEnCia = con.getDataTable().AsEnumerable().Select(s => s.Field<string>("IdCia")).ToList();
                        });
                        productos.Productos = prods;
                        //productos.Productos = JValue.Parse(JsonConvert.SerializeObject(prods.Select(r => new { r.pmIdProducto, r.pmDescripProd, r.pmIvaInc, r.pmLtPreDef, r.pmVrPrecio1, r.pmVrPrecio2, r.pmVrPrecio3, r.pmVrPrecio4, r.pmVrPrecio5, r.pmExcluidoImp, r.pmTarifaIva, r.pmDisponibleEnConpania }), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                    }
                }
                productos.Registro = new Log { Codigo = "1", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = prods.Count, Msg = "Se ejecutó correctamente la consulta." };

            }
            else
            {
                productos.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = prods.Count, Msg = "No se encontró el usuario o la contraseña es incorrecta." };
            }

            return productos;
        }
        private DataTable existeUsuario(string agusuario, string agcontrasena)
        {
            DataSet TablaUsuario = new DataSet();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@IdUsuario", agusuario));
            con.setConnection("Syscom");
            bool respuesta = con.ejecutarQuery("WSPedidos40Sesion", parametros, out TablaUsuario, out string[] mensajeUsuario, CommandType.StoredProcedure);
            return TablaUsuario.Tables[0];
        }
        private bool existeUsuario(Usuario usuario)
        {
            bool existe = false;
            con.setConnection("Syscom");
            con.resetQuery();
            con.qryFields.Add("IdUsuario, PwdLog");
            con.qryTables.Add("adm_Usuarios");
            con.addWhereAND("Inactivo = 0 and lower(IdUsuario) = lower('" + usuario.IdUsuario + "')");
            con.select();
            con.ejecutarQuery();
            DataTable ds = con.getDataTable();
            DataRow row = ds.Rows[0];
            if (ds.Rows.Count > 0)
            {
                pwdSyscom pwdSys = new pwdSyscom();
                pwdSys.Decodificar(row["PwdLog"].ToString());
                var contra = pwdSys.contrasenna.Split('=');
                if (contra[2] == usuario.Contrasena)
                {
                    existe = true;

                }
                else
                {
                    existe = false;
                }
            }
            return existe;
        }


        public CarteraResp RespCartera(CarteraReq ReqCartera)
        {
            //Instanciamo la conexion
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            CarteraResp respuesta = new CarteraResp();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;
                if (ReqCartera.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else
                {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(ReqCartera.usuario.UserName, ReqCartera.usuario.Password, out string[] mensajeNuevo))
                    {
                        respuesta.Error = new Log { Codigo = "999", Msg = "Ok" };
                        if (ReqCartera.NitCliente == null || string.IsNullOrWhiteSpace(ReqCartera.NitCliente))
                        {
                            // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                            DataSet Tablainfo = new DataSet();
                            //Instanciamos la clase de cartera para poder ingresar los datos obtenidos en el metodo
                            Cartera cart = new Cartera();
                            //Instanciamos la clase item cartera para instanciar la lista de cartera que esta clase contiene 
                            ItemCartera cartItem = new ItemCartera();
                            //Creamos una nueva instancia de la lista de cartera la cual la contienen un nombre que se llama detalle.

                            List<ItemCartera> datItemCart = new List<ItemCartera>();
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                            con.setConnection("Syscom");
                            //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                            List<SqlParameter> parametros = new List<SqlParameter>();
                            //Indicamos el parametro que vamos a pasar 
                            parametros.Add(new SqlParameter("@NitCliente", ReqCartera.NitCliente));
                            con.addParametersProc(parametros);

                            //Ejecuta procedimiento almacenado
                            //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                            DataTable DT = new DataTable();
                            // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                            con.resetQuery();
                            //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                            if (con.ejecutarQuery("WcfPedidos_ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                            {
                                //IEnumerable Convierte la tabla en una secuencia de objetos DataRow que se pueden usar en consultas LINQ.



                                datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);
                                datItemCart.ForEach(m =>
                                {
                                    m.Detalle = new List<Cartera>();

                                    m.Detalle = con.DataTableToList<Cartera>(Tablainfo.Tables[0].Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                                });


                                //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                respuesta.DatosCartera = datItemCart;
                                //respuesta.DatosCartera.Add(cartItem);
                            }
                            else {
                                respuesta.Error = new Log { Msg = "No se pudieron ejecutar las consultas" };
                            }

                        }
                        else
                        {
                            // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                            // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                            DataSet Tablainfo = new DataSet();
                            //Instanciamos la clase de cartera para poder ingresar los datos obtenidos en el metodo
                            Cartera cart = new Cartera();
                            //Instanciamos la clase item cartera para instanciar la lista de cartera que esta clase contiene 
                            ItemCartera cartItem = new ItemCartera();
                            //Creamos una nueva instancia de la lista de cartera la cual la contienen un nombre que se llama detalle.

                            List<ItemCartera> datItemCart = new List<ItemCartera>();


                            try
                            {
                                //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                                // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                                con.setConnection("Syscom");
                                //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                                List<SqlParameter> parametros = new List<SqlParameter>();
                                //Indicamos el parametro que vamos a pasar 
                                parametros.Add(new SqlParameter("@NitCliente", ReqCartera.NitCliente));
                                con.addParametersProc(parametros);

                                //Ejecuta procedimiento almacenado
                                //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                                DataTable DT = new DataTable();
                                // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                                con.resetQuery();
                                //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                                if (con.ejecutarQuery("WcfPedidos_ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                                {
                                    //IEnumerable Convierte la tabla en una secuencia de objetos DataRow que se pueden usar en consultas LINQ.



                                    datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);
                                    datItemCart.ForEach(m =>
                                    {
                                        m.Detalle = new List<Cartera>();

                                        m.Detalle = con.DataTableToList<Cartera>(Tablainfo.Tables[0].Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                                    });


                                    //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                    respuesta.DatosCartera = datItemCart;
                                    //respuesta.DatosCartera.Add(cartItem);


                                }

                            }
                            catch (Exception e)
                            {
                                respuesta.Error = new Log { Msg = e.Message };
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Msg = ex.Message };
            }
            return respuesta;
        }

        public ResObtenerCarteraTotal resObtCartTotal(ObtCarTotal Info)
        {
            //Instanciamo la conexion
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            ResObtenerCarteraTotal respuesta = new ResObtenerCarteraTotal();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;
                if (Info.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else
                {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(Info.usuario.UserName, Info.usuario.Password, out string[] mensajeNuevo))
                    { 

                        // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                        DataSet Tablainfo = new DataSet();
                        //Instanciamos la clase de cartera para poder ingresar los datos obtenidos en el metodo
                        Cartera cart = new Cartera();
                        //Instanciamos la clase item cartera para instanciar la lista de cartera que esta clase contiene 
                        ItemCartera cartItem = new ItemCartera();
                        //Creamos una nueva instancia de la lista de cartera la cual la contienen un nombre que se llama detalle.

                        List<ResCarteraTotal> datItemCart = new List<ResCarteraTotal>();

                        try
                        {
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                            con.setConnection("Syscom");
                            //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                            List<SqlParameter> parametros = new List<SqlParameter>();
                            //Ejecuta procedimiento almacenado
                            //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                            DataTable DT = new DataTable();
                            // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                            con.resetQuery();
                            //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                            if (con.ejecutarQuery("WSPedidoS40_consObtenerCarteraTotal", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                            {
                                //IEnumerable Convierte la tabla en una secuencia de objetos DataRow que se pueden usar en consultas LINQ.

                                datItemCart = con.DataTableToList<ResCarteraTotal>("Tercero,SaldoCartera".Split(','), Tablainfo);

                                //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                respuesta.DatosCartera = datItemCart;
                                //respuesta.DatosCartera.Add(cartItem);

                            }

                        }
                        catch (Exception e)
                        {
                            respuesta.Error = new Log { Msg = e.Message };
                        }
                    
                    } 
                }


            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Msg = ex.Message };
            }
            return respuesta;
        }


        public ResObtenerCartera getCarteraTotalDet(authCartera Modelo)
        {
            //Instanciamo la conexion
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            ResObtenerCartera respuesta = new ResObtenerCartera();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;
                if (Modelo.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else
                {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(Modelo.usuario.UserName, Modelo.usuario.Password, out string[] mensajeNuevo))
                    {
                        respuesta.Error = new Log { Codigo = "999", Msg = "Ok" };
                       
                      


                           

                            // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                            DataSet Tablainfo = new DataSet();
                            //Instanciamos la clase de cartera para poder ingresar los datos obtenidos en el metodo
                            Cartera cart = new Cartera();
                            //Instanciamos la clase item cartera para instanciar la lista de cartera que esta clase contiene 
                            ItemCartera cartItem = new ItemCartera();
                            //Creamos una nueva instancia de la lista de cartera la cual la contienen un nombre que se llama detalle.

                            List<ItemCartera> datItemCart = new List<ItemCartera>();
                             string fechaIni = DateTime.Parse(Modelo.FechaInicial).ToString("yyyyMMdd");
                             string fechaFin = DateTime.Parse(Modelo.FechaFinal).ToString("yyyyMMdd");
                        try
                            {
                                //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                                // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                                con.setConnection("Syscom");
                                //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                                List<SqlParameter> parametros = new List<SqlParameter>();
                                //Indicamos el parametro que vamos a pasar
                                parametros.Add(new SqlParameter("@FechaInicial", fechaIni));
                                parametros.Add(new SqlParameter("@FechaFinal", fechaFin));
                                con.addParametersProc(parametros);

                                //Ejecuta procedimiento almacenado
                                //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                                DataTable DT = new DataTable();
                                // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                                con.resetQuery();
                                //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                                if (con.ejecutarQuery("WSPedidoS40_consObtenerCarteraTotalDEF", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                                {
                                    //IEnumerable Convierte la tabla en una secuencia de objetos DataRow que se pueden usar en consultas LINQ.

                                    datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);
                                    datItemCart.ForEach(m =>
                                    {
                                        m.Detalle = new List<Cartera>();

                                        m.Detalle = con.DataTableToList<Cartera>(Tablainfo.Tables[0].Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                                    });

                                    //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                    respuesta.Datoscartera = datItemCart;
                                    //respuesta.DatosCartera.Add(cartItem);


                                }

                            }
                            catch (Exception e)
                            {
                                respuesta.Error = new Log { Msg = e.Message };
                            }
                        
                    }
                }


            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Msg = ex.Message };
            }
            return respuesta;
        }

       
        public ResInfoMaestra GetInfMaestra(InfoMaestra Parametros)
        {
            //Instanciamo la conexion
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            ResInfoMaestra respuesta = new ResInfoMaestra();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;
                if (Parametros.Usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else
                {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(Parametros.Usuario.IdUsuario, Parametros.Usuario.Contrasena, out string[] mensajeNuevo))
                    {
                        respuesta.Error = new Log { Codigo = "999", Msg = "Ok" };

                        if (Parametros.TipoRegistro != 0)
                        {
                            // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                            DataSet Tablainfo = new DataSet();
                            //Instanciamos la clase de cartera para poder ingresar los datos obtenidos en el metodo

                            try
                            {
                                //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                                con.setConnection("Syscom");
                                //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                                List<SqlParameter> parametros = new List<SqlParameter>();
                                //Indicamos el parametro que vamos a pasar
                                parametros.Add(new SqlParameter("@TipoRegistro", Parametros.TipoRegistro));
                                con.addParametersProc(parametros);

                                //Ejecuta procedimiento almacenado
                                //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                                DataTable dataTable = new DataTable();
                                // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                                con.resetQuery();
                                //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                                if (con.ejecutarQuery("WSPedido_consInfoMaestra", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                                {
                                    //IEnumerable Convierte la tabla en una secuencia de objetos DataRow que se pueden usar en consultas LINQ.

                                    List<Dictionary<string, object>> ListData = new List<Dictionary<string, object>>();
                                    dataTable = Tablainfo.Tables[0];
                                    if (Tablainfo.Tables[0].Rows.Count > 0) {
                                        foreach (DataRow row in dataTable.Rows)
                                        {
                                            Dictionary<string, object> rowDicc = new Dictionary<string, object>();
                                            foreach (DataColumn col in dataTable.Columns)
                                            {
                                                rowDicc[col.ColumnName] = row[col];
                                            }
                                            ListData.Add(rowDicc);
                                        }
                                        respuesta.Respuesta = ListData;
                                    }
                                    //respuesta.Respuesta = con.DataTableToList<Regimen>("IdRegimen,regimen".Split(','), Tablainfo);
                                }

                            }
                            catch (Exception e)
                            {
                                respuesta.Error = new Log { Msg = e.Message };
                            }

                        }
                        else {
                         
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Msg = ex.Message };
            }
            return respuesta;

        }
    }

}
