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
            //Instanciamo la conexion
            connect.Conexion con = new connect.Conexion();

            //Instanciamos la clase de ResObtenerCarteraTotal para poder ingresar los resultados en dicha clase
            ResObtenerCarteraTotal respuesta = new ResObtenerCarteraTotal();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario y contraseña
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
                               //Llamamos el metodo  DataTableTolist y le pasamos los parametros necesarios para asi 

                                datItemCart = con.DataTableToList<ResCarteraTotal>("Tercero,SaldoCartera".Split(','), Tablainfo);

                                //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                respuesta.DatosCartera = datItemCart;
                                //respuesta.DatosCartera.Add(cartItem);

                            }
                            // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                            con.resetQuery();

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
                                
                                //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                                if (con.ejecutarQuery("WSPedidoS40_consObtenerCarteraTotalDEF", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                                {
                                    //Implementamos el metodo DataTableToList para de esta manera pasarle los parametros y convertir un DataSet en una lista que se llama "datItemCart"

                                    datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);
                                    datItemCart.ForEach(m =>
                                    {
                                        //A medida que se itera ItemCartera, se crea una nueva lista de objetos de tipo Cartera y se asigna a la propiedad Detalle del objeto ItemCartera. La lista de objetos Cartera.
                                        // Esta se crea a partir de una tabla de datos que se filtra utilizando el valor de la propiedad Tercero
                                        m.Detalle = new List<Cartera>();

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
                                    
                                    //Creo una lista de tipo diccionario
                                    List<Dictionary<string, object>> ListData = new List<Dictionary<string, object>>();
                                    //Iniciamos recorriendo la primera tabla del DataSet y verificamos si esta tiene algunos resultados
                                    dataTable = Tablainfo.Tables[0];
                                    if (Tablainfo.Tables[0].Rows.Count > 0) {
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
        #endregion
    }

}
