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
                            List<SqlParameter> paramProductos = new List<SqlParameter>();
                            paramProductos.Add(new SqlParameter("@IdProducto", r.IdProducto));
                            con.addParametersProc(paramProductos);
                            con.ejecutarProcedimiento("WcfPedidos40_GetProductos");
                            /*con.qryFields.Add(@"IdCia");
                            con.qryTables.Add(@"ProdCompanias");
                            con.addWhereAND("IdProducto = '" + r.IdProducto + "'");
                            con.select();
                            con.ejecutarQuery();*/
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
        public RespClientes GetClientes(Usuario usuario)
        {
            //LogErrores.tareas.Add("ingreso ==>" + DateTime.Now.ToShortDateString());
            //LogErrores.write();
            RespClientes resultado = new RespClientes();
            List<ClienteResponse> clientes = new List<ClienteResponse>();
            if (existeUsuario(usuario))
            {
                DateTime pmFecha_Actual = DateTime.Parse(usuario.Fecha_Act);

                connect.Conexion log = new connect.Conexion();
                DataSet TablaVendedores = new DataSet();
                DataSet TablaClientes = new DataSet();
                List<SqlParameter> parametrosVendedores = new List<SqlParameter>();
                List<SqlParameter> parametrosClientes = new List<SqlParameter>();
                parametrosVendedores.Add(new SqlParameter("@FechaActual", pmFecha_Actual));
                log.setConnection("dbsLog");
                if (log.ejecutarQuery("WcfPedidos40_LogVendedores", parametrosVendedores, out TablaVendedores, out string[] mensajeVendedores, CommandType.StoredProcedure))
                {
                    DataTable tmpDt = TablaVendedores.Tables[0];
                    int count = TablaVendedores.Tables[0].Rows.Count;


                    List<LogVendedores> vendedoresNuevos = tmpDt.AsEnumerable().Select(r => new LogVendedores
                    {
                        Numero = r.Field<int>("Numero"),
                        Fecha = r.Field<DateTime>("Fecha"),
                        ClaveReg = r.Field<string>("ClaveReg"),
                        TipoProc = r.Field<string>("TipoProc"),
                        IdCliente = r.Field<string>("IdCliente"),
                        CdAgencia = r.Field<string>("CdAgencia"),
                        CdVendAnt = r.Field<string>("CdVendAnt"),
                        IdVend = r.Field<string>("IdVend"),
                        FechaCrea = r.Field<DateTime>("FechaCrea"),
                        IdUsuario = r.Field<string>("IdUsuario"),
                        Nombre = r.Field<string>("Nombre"),
                        NomCliente = r.Field<string>("NomCliente"),
                        NomAgencia = r.Field<string>("NomAgencia"),
                        NomVendedor = r.Field<string>("NomVendedor"),
                        NomVendAnt = r.Field<string>("NomVendAnt")
                    }).ToList();

                    try
                    {
                        parametrosClientes.Add(new SqlParameter("@FechaActual", pmFecha_Actual));
                        string idcliente = (vendedoresNuevos.Count > 0 ? String.Join(",", vendedoresNuevos.GroupBy(c => c.IdCliente).Select(c => c.First().IdCliente).ToList().Select(s => "'" + s + "'").ToArray()) : "" + "");
                        parametrosClientes.Add(new SqlParameter("@IdCliente", idcliente));
                        if (con.ejecutarQuery("WcfPedidos40_Clientes", parametrosClientes, out TablaClientes, out string[] mensajeClientes, CommandType.StoredProcedure))
                        {
                            tmpDt = TablaClientes.Tables[0];
                            count = TablaClientes.Tables[0].Rows.Count;
                            IEnumerable<DataRow> data = tmpDt.AsEnumerable();
                            IEnumerable<DataRow> dataFil = data.GroupBy(g => g.Field<string>("IdTercero")).Select(g => g.First());

                            dataFil.ToList().ForEach(i => clientes.Add(new ClienteResponse
                            {

                                IdTercero = i.Field<string>("IdTercero"),
                                RazonSocial = i.Field<string>("RazonSocial"),
                                Direccion = i.Field<string>("Direccion"),
                                IdLocal = i.Field<string>("IdLocal"),
                                Localidad = i.Field<string>("Localidad"),
                                Telefono = i.Field<string>("Telefono"),
                                IdRegimen = i.Field<string>("IdRegimen"),
                                Regimen = i.Field<string>("Regimen"),
                                IdGrupo = i.Field<string>("IdGrupo"),
                                IdSector = i.Field<string>("IdSector"),
                                SectorEco = i.Field<string>("SectorEco"),
                                IdPlazo = i.Field<string>("IdPlazo"),
                                Plazo = i.Field<string>("Plazo"),
                                CdCms = i.Field<string>("CdCms"),
                                TarCms = i.Field<decimal?>("TarCms"),
                                IdVend = i.Field<string>("IdVend"),
                                Vendedor = i.Field<string>("Vendedor"),
                                DiasEntrega = i.Field<string>("CdDiaEnt"),
                                NumLista = i.Field<string>("NumLista"),
                                CodVend = i.Field<string>("Codigo"),
                                IdSZona = i.Field<string>("IdSZona"),
                                Subzona = i.Field<string>("SubZona"),
                                IdZona = i.Field<string>("IdZona"),
                                Zona = i.Field<string>("Zona"),
                                IdRuta = i.Field<string>("IdRuta"),
                                Ruta = i.Field<string>("Ruta"),
                                Inactivo = (i.Field<bool>("Inactivo") ? "1" : "0")
                            }));

                            try
                            {
                                clientes.ForEach(c =>
                                {
                                    if (data.Where(r => r.Field<string>("IdTercero") == c.IdTercero && r.Field<string>("IdAgencia") != null).Count() > 0)
                                    {
                                        c.Agencias = new List<Agencia>();
                                        data.Where(ca => ca.Field<string>("IdTercero") == c.IdTercero && ca.Field<string>("IdAgencia") != null).ToList().ForEach(i => c.Agencias.Add(new Agencia
                                        {

                                            IdAgencia = i.Field<string>("IdAgencia"),
                                            NomAgencia = i.Field<string>("Agencia"),
                                            DirAgncia = i.Field<string>("DirAgncia"),
                                            AgIdLocal = i.Field<string>("AgIdLocal"),
                                            AgLocalidad = i.Field<string>("AgLocalidad"),
                                            TelAgncia = i.Field<string>("TelAgncia"),
                                            NomCont = i.Field<string>("NomCont"),
                                            emlCont = i.Field<string>("emlCont"),
                                            AgIdVend = i.Field<string>("AgIdVend"),
                                            AgVendedor = i.Field<string>("AgVendedor"),
                                            AgCdCms = i.Field<string>("AgCdCms"),
                                            AgTarCms = i.Field<decimal?>("AgTarCms"),
                                            AgCdDct = i.Field<string>("AgCdDct"),
                                            AgTarDct = i.Field<decimal?>("AgTarDct"),
                                            DiasEntrega = i.Field<string>("CodDiaEnt"),
                                    //Versión 003
                                    AgIdSZona = i.Field<string>("IdSZonaA"),
                                            AgSubZona = i.Field<string>("SubzonaA"),
                                            AgIdZona = i.Field<string>("IdZonaA"),
                                            AgZona = i.Field<string>("ZonaA"),
                                            AgIdRuta = i.Field<string>("IdRutaA"),
                                            AgRuta = i.Field<string>("RutaA")
                                        }));
                                    }
                                });
                            }
                            catch (Exception exx)
                            {
                                LogErrores.tareas.Add(exx.Message);
                                LogErrores.write();
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        LogErrores.tareas.Add(ex.Message);
                        LogErrores.write();
                    }
                    resultado.Clientes = clientes;
                    resultado.Registro = new Log { Codigo = "1", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "Se ejecutó correctamente la consulta." };
                }
            }
            else
                resultado.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "No se encontró el usuario o la contraseña es incorrecta." };

            return resultado;
        }

        public PedidoResponse SetPedido(PedidoRequest pedido)
        {
            bool continuar = true;
            if (String.IsNullOrEmpty(pedido.Agencia))
                pedido.Agencia = "0";
            PedidoResponse resp = new PedidoResponse();
            con = new connect.Conexion();
            con.setConnection("Syscom");
            if (existeUsuario(pedido.Usuario))
            {
                //Buscar si la agencia pertence al cliente
                if (pedido.Agencia != "0")
                {
                    List<SqlParameter> parametrosCliente = new List<SqlParameter>();
                    DataSet TablaAgencia = new DataSet();
                    parametrosCliente.Add(new SqlParameter("@IdTercero", pedido.Cliente.IdTercero));
                    parametrosCliente.Add(new SqlParameter("@IdAgencia", pedido.Agencia));
                    if (con.ejecutarQuery("WcfPedidos40_ConsultarAgencia", parametrosCliente, out TablaAgencia, out string[] mensajeAgencia, CommandType.StoredProcedure))
                    {
                        if (TablaAgencia.Tables[0].Rows.Count <= 0)
                        {
                            resp.Errores.Add("La agencia '" + pedido.Agencia + " del cliente '" + pedido.Cliente.IdTercero + "', NO existe!");
                            goto fin;
                        }
                    }
                }

                if (continuar)
                {
                    DataTable Trn_OPedido = new DataTable();
                    DataTable Trn_Kardex = new DataTable();

                    //LogErrores.tareas.Add(pedido.Agencia);
                    //LogErrores.tareas.Add(pedido.Cliente.IdTercero);
                    //LogErrores.write();
                    Pedido ped = new Pedido(pedido.Usuario.IdUsuario, pedido.Cliente, pedido.TipoPedido, pedido.Agencia);

                    DataSet tablaAdmusuario = new DataSet();
                    List<SqlParameter> parametrosAdmUsuario = new List<SqlParameter>();
                    parametrosAdmUsuario.Add(new SqlParameter("@IdUsuario", pedido.Usuario.IdUsuario));
                    con.ejecutarQuery("WcfPedidos40_admUsuario", parametrosAdmUsuario,out tablaAdmusuario, out string[] mensajeAdm, CommandType.StoredProcedure);

                    DataTable adm_Usuario = tablaAdmusuario.Tables[0];

                    DataTable productos = ped.obtenerProductos(pedido.Productos);
                    List<string> inactivos = productos.Rows.Cast<DataRow>().Where(r => r.Field<bool>("Inactivo") == true).Select(r => r.Field<string>("IdProducto")).ToList();
                    if (inactivos.Count == 0)
                    {
                        if (productos.Rows.Cast<DataRow>().Where(r => !new string[] { "1000", "1001", "1002" }.Contains(r.Field<string>("IdProducto"))).Count() > 0 && pedido.TipoPedido == "A")
                        {
                            resp.Errores.Add("Los pedidos tipo Abono, solo pueden contener productos con los codigos '1000', '1001' y '1002'");
                            goto fin;
                        }

                        ped.setProductos(productos);
                        ped.Procesar();
                    }

                    var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                    if (ped.obtenerErrores().Count == 0)
                        resp.Pedido = new PedidoResp
                        {
                            TipDoc = ped.OPedido.Rows[0]["TipDoc"].ToString(),
                            Pedido = ped.OPedido.Rows[0]["Pedido"].ToString() + "-" + ped.OPedido.Rows[0]["IdCia"].ToString(),
                            Fecha = ped.OPedido.Rows[0]["Fecha"].ToString(),
                            Agencia = pedido.Agencia
                        };

                    resp.Errores = ped.obtenerErrores();
                    if (inactivos.Count > 0)
                        resp.Errores.Add(String.Join(@"\n", inactivos.Select(r => "El producto '" + r + "' está inactivo.").ToArray()));

                }
            }
            else
            {
                resp.Errores = new List<string>();
                resp.Errores.Add("¡No se encontró el usuario o la contraseña es incorrecta!");
            }

        fin:
            { }
            LogErrores.tareas.Add("El pedido es==>" + resp.Pedido);
            LogErrores.tareas.Add("Los errores son: Errores ==> " + resp.Errores);
            LogErrores.write();
            return resp;
        }

        private bool existeUsuario(Usuario usuario)
        {
            DataTable ds = new DataTable();
            bool existe = false;
            DataSet TablaSesion = new DataSet();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@IdUsuario", usuario.IdUsuario));
            con.setConnection("Syscom");
            if (con.ejecutarQuery("WSPedidos40Sesion", parametros, out TablaSesion, out string[] mensajeSesion, CommandType.StoredProcedure))
            {
                ds = TablaSesion.Tables[0];

                if (ds.Rows.Count > 0)
                {
                DataRow row = ds.Rows[0];
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
            }
            return existe;

        }

         #region ObtenerCartera

        public CarteraResp RespCartera(CarteraReq ReqCartera)
        {
            //Instanciamos la conexion
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            CarteraResp respuesta = new CarteraResp();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;

                //Verficamos que los datos del usuario no vengan nulos
                if (ReqCartera.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
               
                //Verificamos que la contraseña del usuario no venga nula ni vacia
                if (ReqCartera.usuario.Password == null || string.IsNullOrWhiteSpace(ReqCartera.usuario.Password))
                {
                    respuesta.Error = new Log { Codigo = "002", Msg = "¡La contraseña no puede llegar vacia o nula!" };
                }

                //Verficamos que el Nombre del usuario no sea nula o venga vacia
                if (ReqCartera.usuario.UserName == null || string.IsNullOrWhiteSpace(ReqCartera.usuario.UserName))
                {
                    respuesta.Error = new Log { Codigo = "002", Msg = "¡El usuario no puede llegar vacio o nulo!" };
                }

                //Una vez validado que los datos del usuario vengan diligenciados verificamos si este existe en la base de datos
                else
                {
                    //Creamos un objeto usuario y lo instanciamos para obtener sus propiedades y poder verificar los valores
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(ReqCartera.usuario.UserName, ReqCartera.usuario.Password, out string[] mensajeNuevo))
                    {
                        //Si el Metodo Existe nos regresa true, entonces damos a entender 
                        respuesta.Error = new Log { Codigo = "001", Msg = "!!Usuario Logueado¡¡" };

                        // Creamos un objeto dataset y lo instanciamos  para almacenar los datos obtenidos del procedimiento de almacenado 
                        DataSet Tablainfo = new DataSet();

                        //Instanciamos la clase de cartera para poder obtener las propiedades de esta y darle los valores respectivos 
                        Cartera cart = new Cartera();

                        //Instanciamos la clase item cartera para instanciar la lista de cartera que esta clase contiene 
                        ItemCartera cartItem = new ItemCartera();

                        //Creamos una nueva instancia de la lista de cartera la cual la contienen un nombre que se llama detalle.
                        List<ItemCartera> datItemCart = new List<ItemCartera>();

                        try
                        {
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y de esta manera ejecutar los  procedimientos de almacenado que se encuentran en estas
                            con.setConnection("Syscom");

                            //Se realiza una lista de parametros para pasar los datos  obtenidos a los procedimientos de almacenado
                            List<SqlParameter> parametros = new List<SqlParameter>();

                            //Indicamos el nombre del parametro que vamos a pasar con el respectivo valor 
                            parametros.Add(new SqlParameter("@NitCliente", ReqCartera.NitCliente));

                            // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                            con.resetQuery();
                            //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                            if (con.ejecutarQuery("WcfPedidos_ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                            {
                                //Añadimos a la lista datItemCart los datos de tercero y saldo cartera mediante el metodo "DataTableToList"
                                //dicho metodo obtiene un data set y lo convierte en una cadena de strings 
                                datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);

                                //Recorremos la lista datItemCart para que dentro de esta podamos insertar los datos del detalle de la cartera
                                datItemCart.ForEach(m =>
                                {
                                    //m.Detalle es un objeto en el cual se instancia la clase de cartera para que por medio de esta podamos representar los datos de resultado
                                    m.Detalle = new List<Cartera>();

                                    //Luego del instanciamiento agregamos los valores de las columnas de las tablas si dichos datos vienen representados por el IdTercero
                                    m.Detalle = con.DataTableToList<Cartera>(Tablainfo.Tables[0].Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                                });

                                //Pasamos la lista con los valores obtenidos al modelo de cartera response para visualizar los resultados.
                                respuesta.DatosCartera = datItemCart;


                            }

                        }
                        catch (Exception e)
                        {
                            respuesta.Error = new Log { Msg = e.Message };

                        }
                        respuesta.Error = new Log { Codigo = "001", Msg = "El usuario no existe" };
                    }
                }


            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Msg = ex.Message };
            }
            return respuesta;
        }
        #endregion

        #region ObtenerCarteraTotal
        public ResObtenerCarteraTotal resObtCartTotal(ObtCarTotal Info)
        {
            //Creamos un objeto de conexion y instanciamos la clase de conexion para poder conectarse con la base de datos
            connect.Conexion con = new connect.Conexion();

            //Creo un objeto de ResObtenerCartera e instancio dicha clase para poder ingresarle los resultados 
            ResObtenerCarteraTotal respuesta = new ResObtenerCarteraTotal();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario y contraseña
               
                //Verificamos que los datos del usuario no sean nulos  asegurarlos que dicha clase contenga informacion
                if (Info.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                //Verificamos que el nombre del usuario sea diligenciado y no sea un nulo o venga vacio para asegurarnos que sea el mismo usuario
                if (Info.usuario.UserName == null || string.IsNullOrWhiteSpace(Info.usuario.UserName)) 
                {
                    respuesta.Error = new Log {Codigo = "002", Msg="El nombre del usuario no puede ser nulo" };
                }
                //Verificamos la contraseña del usuario que sea diligenciada para comprobar mas adelante la existencia del usuario 
                if (Info.usuario.Password == null || string.IsNullOrWhiteSpace(Info.usuario.Password))
                {
                    respuesta.Error = new Log { Codigo = "002", Msg = "La contraseña del usuario no puede ser nula" };
                }
                else
                {
                    //Creamos un nuevo objeto tipo ExisteUsuario y lo instanciamos para acceder a un metodo
                    ExisteUsuario ExistUsu = new ExisteUsuario();

                    //Accedemos al metodo "Existe" el cual es un booleano y nos arrojara true or false dependiendo si el usuario logueado se encuentra
                    if (ExistUsu.Existe(Info.usuario.UserName, Info.usuario.Password, out string[] mensajeNuevo))
                    {

                        // Creamos un nuevo objeto data set y lo instanciamos para poder obtener los resultados del procedimiento de datos 
                        DataSet Tablainfo = new DataSet();
                        //Creo un nuevo objeto de cartera y lo instancio para acceder a sus propiedades y asginarles un valor 
                        Cartera cart = new Cartera();
                       // Creo un objeto CartItem y lo instancio para acceder a unas propiedades   
                        ItemCartera cartItem = new ItemCartera();

                        //Creo una lista de la clase ResCarteraTotal, la cual es donde mostraremos los resultados obtenidos del procedimiento 
                        List<ResCarteraTotal> datItemCart = new List<ResCarteraTotal>();

                        try
                        {
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                            con.setConnection("Syscom");
                            //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                            List<SqlParameter> parametros = new List<SqlParameter>();
            
                            // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                            con.resetQuery();
                            //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                            if (con.ejecutarQuery("WSPedidoS40_consObtenerCarteraTotal", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                            {
                                //Llamamos el metodo  DataTableTolist y le paso los parametros del data set para asi obtener una lista de strings 
                                datItemCart = con.DataTableToList<ResCarteraTotal>("Tercero,SaldoCartera".Split(','), Tablainfo);

                                //Los los datos que tengo en la lista que alamceno en la lista datItemCart a la clase de datos cartera
                                //en la cual mostraremos los resultados del procedimiento 
                                respuesta.DatosCartera = datItemCart;

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
        #endregion

        #region ObtenerCarteraTotalDef
        public ResObtenerCartera getCarteraTotalDet(authCartera Modelo)
        {
            //Creo un nuevo objeto de conexion y creo una instancia para poder acceder a sus metodos 
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            ResObtenerCartera respuesta = new ResObtenerCartera();

            try
            {   

              //Verificamos que los datos del usuario no sean nulos , para pasar a validar campo por campo 
                if (Modelo.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                ////Verificamos que el nombre del usuario no sea nulo ni vacio 
                if (Modelo.usuario.UserName == null || String.IsNullOrWhiteSpace(Modelo.usuario.UserName))
                {
                    respuesta.Error = new Log { Codigo = "002", Msg = "¡El nombre del usuario no puede llegar nulo, ni venir vacio!" };
                }
                ////Verificamos que la contraseña no sea valida ni vacia  para verificar la identidad del usuario 
                if (Modelo.usuario.Password == null || String.IsNullOrWhiteSpace(Modelo.usuario.Password))
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡La contraseña del usuario no puede ser nula !" };
                }
                //Verificamos que la fecha ingresada sea valida ya que se necesitara como base para saber desde cuando obtendremos las carteras
                if (Modelo.FechaInicial == null || String.IsNullOrWhiteSpace(Modelo.FechaInicial))
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡Ingrese una fecha de incio valida!" };
                }
                //Verificamos la fecha final que sea valida para saber hasta que fecha obtendremos las carteras
                if (Modelo.FechaFinal == null || String.IsNullOrWhiteSpace(Modelo.FechaFinal))
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡Ingrese una fecha final valida para obtener las carteras!" };
                }
                else
                {
                    //Creo un objeto de existe usuario y lo instancio para acceder a sus metodos 
                    ExisteUsuario ExistUsu = new ExisteUsuario();

                    //Le ingreso los datos de Username y Password al metodo existe , para que me de un true or false dependiendo si se encuentra el usuario logueado
                    if (ExistUsu.Existe(Modelo.usuario.UserName, Modelo.usuario.Password, out string[] mensajeNuevo))
                    {
                        respuesta.Error = new Log { Codigo = "000", Msg = "Usuario_Logueado" };

                        //Creo un objeto data set y lo instancio para almacenar aqui los resultados del procedimiento de almacenado
                        DataSet Tablainfo = new DataSet();
                        //Creo un objeto de cartera y la instancio para poder accerder a sus propiedades y asignarles su debido valor
                        Cartera cart = new Cartera();
                       //Creo un objeto de cartItem y la instancio para acceder a sus propiedades
                      
                        //Creo una lista de la clase ItemCartera para acceder a sus propiedades y asignarles un valor respectivo
                        List<ItemCartera> datItemCart = new List<ItemCartera>();

                        //Las variables FechaIni y FechaFin las implemento para de esta manera darles el debido formato y asi pasarlas al procedimiento de almacenado
                        string fechaIni = DateTime.Parse(Modelo.FechaInicial).ToString("yyyyMMdd");
                        string fechaFin = DateTime.Parse(Modelo.FechaFinal).ToString("yyyyMMdd");
                        try
                        {
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                            con.setConnection("Syscom");

                            //Se realiza una lista de parametros para poder pasar los datos al procedimiento de almacenado 
                            List<SqlParameter> parametros = new List<SqlParameter>();

                            //Indicamos el parametro que vamos a pasar con su respectivo valor 
                            parametros.Add(new SqlParameter("@FechaInicial", fechaIni));
                            parametros.Add(new SqlParameter("@FechaFinal", fechaFin));


                            //Ejecuto el metodo "con.ejecutarQuery" donde le paso los parametros necesarios que son el data set , los parametros y el nombre del procedimiento
                            //Si el metodo "con.ejecutarQuery" nos da true entonces pasamos a asginarlos los valores del data set a las listas
                            if (con.ejecutarQuery("WSPedidoS40_consObtenerCarteraTotalDEF", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                            {
                                //Implementamos el metodo DataTableToList para de esta manera pasarle los parametros y convertir un DataSet en una lista que se llama "datItemCart"

                                datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);
                                datItemCart.ForEach(m =>
                                {
                                    //A medida que se itera ItemCartera, se crea una nueva lista de objetos de tipo Cartera y se asigna a la propiedad Detalle del objeto ItemCartera. La lista de objetos Cartera.
                                    
                                    m.Detalle = new List<Cartera>();

                                    //Usuamos la lista cartera y validamos por medio del where que solo traiga los datos que tengan el mismo id del tercero
                                    m.Detalle = con.DataTableToList<Cartera>(Tablainfo.Tables[0].Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                                });

                                //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                respuesta.Datoscartera = datItemCart;
                                // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                                con.resetQuery();
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
        #endregion

        #region InformacionMaestra
        public ResInfoMaestra GetInfMaestra(InfoMaestra Parametros)
        {
            //Creo un nuevo objeto de conexion y lo instancio para acceder a sus metodos 
            connect.Conexion con = new connect.Conexion();

            //Creo un nuevo objeto ResInfoMaestra y la instancio para de esta manera pasarle los resultados y poder mostrarlos al usuario
            ResInfoMaestra respuesta = new ResInfoMaestra();

            try
            {   
                //Verifico los datos del usuario para despues validar todos los campos que este contiene
                if (Parametros.Usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                //Verifico que el nombre de usuario sea valido para verificar la identidad del usuario
                if (Parametros.Usuario.IdUsuario == null || String.IsNullOrWhiteSpace(Parametros.Usuario.IdUsuario))
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡Ingrese un nombre de usuario valido!" };
                }
                //Verifico la contraseña del usuario para validar la identidad de este
                if (Parametros.Usuario.Contrasena == null || String.IsNullOrWhiteSpace(Parametros.Usuario.Contrasena))
                {
                    respuesta.Error = new Log { Codigo = "user_002", Msg = "¡Ingrese una contraseña valida!" };
                }
           
                else
                {
                    //Creo un nuevo objeto de Existe usuario y lo instancio para poder accerder a su metodo 
                    ExisteUsuario ExistUsu = new ExisteUsuario();

                    //Ingreso al metodo Existe y le paso los parametros los cuales son el nombre del usuario y la contraseña de este
                    //Dependiendo de la respuesta del metodo booleano pasamos las respuestas a las listas
                    if (ExistUsu.Existe(Parametros.Usuario.IdUsuario, Parametros.Usuario.Contrasena, out string[] mensajeNuevo))
                    {
                        respuesta.Error = new Log { Codigo = "000", Msg = "Usuario Logueado" };

                        //Verifico que el tipo de registro sea diferente a cero para asi pasarle un dato valido al parametro que se va al procedimiento de almacenado.
                        if (Parametros.TipoRegistro != 0)
                        {
                            //Creo un nuevo objeto de data set y lo instancio para poder ingresar los valores que reciba del procedimiento de almacenado 
                            DataSet Tablainfo = new DataSet();
                            try
                            {
                                //Por medio del metodo set connection de la clase conexion utilizamos la cadena de conexion que se llama syscom.
                                con.setConnection("Syscom");

                                //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                                List<SqlParameter> parametros = new List<SqlParameter>();

                                //Indicamos el respectivo parametro que pasaremos al procedimiento de almacenado con su debido valor 
                                parametros.Add(new SqlParameter("@TipoRegistro", Parametros.TipoRegistro));

                                //Creamos un objeto dataTable y lo instanciamos para poder pasarle los resultados del procedimiento de almacenado
                                DataTable dataTable = new DataTable();

                                // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                                con.resetQuery();

                                //Ingreso al metodo ejecutarQuery y le paso sus respectivos parametros como el data set , la lista de parametros y el nombre del procedimiento de almacenado 
                                if (con.ejecutarQuery("WSPedido40_consInfoMaestra", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                                {

                                    //Creo una lista de tipo diccionario y la instancia de una lista de diccionarios 
                                    List<Dictionary<string, object>> ListData = new List<Dictionary<string, object>>();

                                 
                                    dataTable = Tablainfo.Tables[0];
                                    if (Tablainfo.Tables[0].Rows.Count > 0)
                                    {
                                        //Si tiene resultados empezamos a recorrer las columnas 
                                        foreach (DataRow row in dataTable.Rows)
                                        {
                                            //
                                            Dictionary<string, object> rowDicc = new Dictionary<string, object>();
                                            foreach (DataColumn col in dataTable.Columns)
                                            {
                                                //Cuando iteramos el data set asignamos los valores por "Claves:Valores"
                                                rowDicc[col.ColumnName] = row[col];
                                            }
                                            //Añadimos los datos a la lista de diccionario
                                            ListData.Add(rowDicc);
                                        }
                                        //Pasamos la lista de diccionario a los bloques de contrato
                                        respuesta.Respuesta = ListData;
                                    }

                                }

                            }
                            catch (Exception e)
                            {
                                respuesta.Error = new Log { Msg = e.Message };
                            }

                        }
                        else
                        {

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
        #endregion
    }

}
