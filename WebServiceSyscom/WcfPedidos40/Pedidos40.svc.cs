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
using static WcfPedidos40.Model.Clientes;

namespace WcfPedidos40
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Pedidos40 : IPedidos40
    {

        private connect.Conexion con = new connect.Conexion();
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
                log.setConnection("dbsLog");
                log.qryFields.Add("Numero, Fecha, ClaveReg, TipoProc, IdCliente, CdAgencia, CdVendAnt, IdVend, FechaCrea, IdUsuario, Nombre, NomCliente, NomAgencia, NomVendedor, NomVendAnt");
                log.qryTables.Add("LogVendedores");
                log.addWhereAND("FechaCrea >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "'");
                log.qryOrderBy.Add("Numero desc");
                log.select();
                log.ejecutarQuery(_aliasConsulta: "_LogVendedores");
                DataTable tmpDt = log.getDataTable();
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

                //LogErrores.tareas.Add("LogVend  registros ==>"+vendedoresNuevos.Count() +" Hora ==>"+ DateTime.Now);
                //LogErrores.write();
                //LogErrores.tareas.Add(String.Join(",", vendedoresNuevos.GroupBy(c => c.IdCliente).Select(c => c.First().IdCliente).ToList().Select(s => "'" + s + "'").ToArray()));
                ////LogErrores.tareas.Add("cantidad de registrso in ==>" + vendedoresNuevos.GroupBy(c => c.IdCliente).Select(c => c.First().IdCliente).ToList().Select(s => "'" + s + "'").ToArray().Count());
                //LogErrores.write();
                //List<string> Clientes = vendedoresNuevos.GroupBy(c => c.IdCliente).Select(c => c.First().IdCliente).ToList();
                try
                {
                    con.resetQuery();
                    con.qryFields.Add(@"t.IdTercero, t.RazonSocial, t.Direccion, l.IdLocal, l.Localidad, t.Telefono, tv.Codigo , rd.IdRegimen, rd.Regimen, tc.Inactivo, 
                    tc.IdGrupo, tc.CdDiaEnt, tc.NumLista, se.IdSector, se.SectorEco, p.IdPlazo, p.DiasPago as Plazo, isnull(sz.IdSZona, '0') AS IdSZona, isnull(sz.Subzona, '0') AS Subzona, isnull(z.IdZona, '0') AS IdZona, isnull(z.Zona, '0') AS Zona, isnull(tc.IdRuta, '0') AS IdRuta, isnull(r.Ruta, '0') AS Ruta, 
                    tp.IdTarifa as CdCms, tp.Tarifa as TarCms, tv.IdTercero as IdVend, tv.RazonSocial as Vendedor, 
                    ag.IdAgencia, ag.Agencia, ag.DirAgncia, agl.IdLocal as AgIdLocal, agl.Localidad as AgLocalidad, ag.TelAgncia, ag.NomCont, ag.emlCont, agv.IdTercero as AgIdVend, 
                    agv.RazonSocial as AgVendedor, agtp.IdTarifa as AgCdCms, agtp.Tarifa as AgTarCms, agdc.IdTarifa as AgCdDct, agdc.Tarifa as AgTarDct, ag.CodDiaEnt,
                    isnull(sza.IdSzona, '0') AS IdSzonaA, isnull(sza.Subzona, '0') AS SubzonaA, isnull(za.IdZona,'0') AS IdZonaA, isnull(za.Zona, '0') AS ZonaA, isnull(ag.CodRuta, '0') AS IdRutaA, isnull(ra.Ruta, '0') as RutaA
                    ");
                    con.qryTables.Add(@"terccliente tc
                    join terceros t on (tc.IdClie = t.IdTercero)
                    join localidades l on (l.IdLocal = t.IdLocal)
                    join Terceros tv on (tv.IdTercero = tc.IdVend)
                    join SectoresEco se on (se.IdSector = t.IdSector)
                    join Plazos p on (p.IdPlazo = tc.IdPlazo)
                    join SubZonas sz on (sz.IdSzona = tc.IdSzona)
                    join Zonas as z on (sz.IdZona = z.IdZona)
                    join Rutas as r on (tc.IdRuta = r.IdRuta)
                    join RegimenDian rd on (rd.IdRegimen = t.IdRegimen)
                    left join TablaPor tp on (tp.IdTarifa = tc.CdCms and tp.IdClase = 'COM')
                    left join Agencias ag on (tc.IdClie = ag.IdClie)
                    left join SubZonas sza on (sza.IdSzona = ag.IdSzona)
                    left join Zonas as za on (sza.IdZona = za.IdZona)
                    left join Rutas as ra on (ag.CodRuta = ra.IdRuta)
                    left join Localidades agl on (agl.IdLocal = ag.IdLocal)
                    left join Terceros agv on (agv.IdTercero = ag.IdVend)
                    left join Tablapor agtp on (agtp.IdTarifa = ag.CdCms and agtp.IdClase = 'COM')
                    left join Tablapor agdc on (agdc.IdTarifa = ag.CdDct and agtp.IdClase = 'DCT')");
                    con.addWhereAND("((ISNULL(tc.FechaUpdate, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or ISNULL(tc.FechaAdd, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "') or (ISNULL(t.FechaUpdate, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or ISNULL(t.FechaAdd, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "')) " + (vendedoresNuevos.Count > 0 ? " or t.IdTercero in (" + String.Join(",", vendedoresNuevos.GroupBy(c => c.IdCliente).Select(c => c.First().IdCliente).ToList().Select(s => "'" + s + "'").ToArray()) + ")" : "" + ""));
                    //21/11/2019 Jeferson solicta quitar el id de vendedor para la empresa pic AND tc.IdVend ='" + usuario.IdUsuario + "'"
                    //12/11/2020 and (tc.Inactivo = 0 and t.Inactivo = 0) se quita para que pimovil pueda ver el clietne y sie stá inactivo no lo muestre
                    //con.addWhereAND("((isnull(tc.FechaUpdate, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or isnull(tc.FechaAdd, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "') or (isnull(t.FechaUpdate, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or isnull(t.FechaAdd, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "'))" + (vendedoresNuevos.Count > 0 ? " or t.IdTercero in (" + String.Join(",", vendedoresNuevos.Select(s => "'" + s.IdCliente + "'").ToArray()) + ")" : "" + ""));
                    con.select();
                    con.ejecutarQuery();
                    tmpDt = con.getDataTable();
                    //DataTable dtClientes = tmpDt.Copy().AsDataView().ToTable(true, "IdTercero,RazonSocial,Direccion,IdLocal,Localidad,Telefono,IdRegimen,Regimen,IdGrupo,IdSector,SectorEco,IdPlazo,Plazo,IdSZona,SubZona,IdZona,Zona,IdRuta,Ruta,CdCms,TarCms,IdVend,Vendedor,CdDiaEnt,NumLista,Codigo,Inactivo".Split(','));
                    //clientes = 
                    //tmpDt.Copy().AsEnumerable().GroupBy(g => g.Field<string>("IdTercero")).Select(g => g.First()).ToList().ForEach(i => clientes.Add(new ClienteResponse
                    //{


                    //LogErrores.tareas.Add("despues de consulta ==>" + DateTime.Now);
                    //LogErrores.write();
                    //LogErrores.tareas.Add("Registros  de clientes ==>" + tmpDt.Rows.Count);
                    //LogErrores.write();
                    IEnumerable<DataRow> data = tmpDt.AsEnumerable();
                    IEnumerable<DataRow> dataFil = data.GroupBy(g => g.Field<string>("IdTercero")).Select(g => g.First());
                    //LogErrores.tareas.Add("Registros  de clientes filtrados ==>" + dataFil.Count());
                    //LogErrores.write();
                    dataFil.ToList().ForEach(i => clientes.Add(new ClienteResponse
                    {
                        //data.ToList().ForEach(i => clientes.Add(new ClienteResponse
                        //{
                        //dtClientes.AsEnumerable().ToList().ForEach(i => clientes.Add(new ClienteResponse
                        //{
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
                        Plazo = Convert.ToInt32(i.Field<string>("Plazo")),
                        CdCms = i.Field<string>("CdCms"),
                        TarCms = i.Field<decimal?>("TarCms"),
                        IdVend = i.Field<string>("IdVend"),
                        Vendedor = i.Field<string>("Vendedor"),
                        //(vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("cliente") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("IdVend")),
                        //pmVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("cliente") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("Vendedor")),
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

                    //LogErrores.tareas.Add("despues de llenar la clase clientes ==>" + DateTime.Now);
                    //LogErrores.write();
                    //clientes = clientes.GroupBy(a => a.pmIdTercero).Select(g => g.First()).ToList();
                    //.GroupBy(a => a.pmIdTercero).Select(g => g.First()).ToList();

                    //LogErrores.tareas.Add("paso clientes"  +clientes.Count);
                    //LogErrores.write();
                    try
                    {
                        //foreach (ClienteResponse cliente in clientes)
                        //{
                        //    if (tmpDt.AsEnumerable().Where(r => r.Field<string>("IdTercero") == cliente.pmIdTercero && r.Field<string>("IdAgencia") != null).Count() > 0)
                        //    {
                        //        cliente.pmAgencias = (from i in tmpDt.AsEnumerable()
                        //        where i.Field<string>("IdTercero") == cliente.pmIdTercero && i.Field<string>("IdAgencia") != null
                        //        select new Agencia
                        //        {
                        //            pmIdAgencia = i.Field<string>("IdAgencia"),
                        //            pmNomAgencia = i.Field<string>("Agencia"),
                        //            pmDirAgncia = i.Field<string>("DirAgncia"),
                        //            pmAgIdLocal = i.Field<string>("AgIdLocal"),
                        //            pmAgLocalidad = i.Field<string>("AgLocalidad"),
                        //            pmTelAgncia = i.Field<string>("TelAgncia"),
                        //            pmNomCont = i.Field<string>("NomCont"),
                        //            pmemlCont = i.Field<string>("emlCont"),
                        //            pmAgIdVend = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("AgIdVend")),
                        //            pmAgVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("AgVendedor")),
                        //            pmAgCdCms = i.Field<string>("AgCdCms"),
                        //            pmAgTarCms = i.Field<decimal?>("AgTarCms"),
                        //            pmAgCdDct = i.Field<string>("AgCdDct"),
                        //            pmAgTarDct = i.Field<decimal?>("AgTarDct"),
                        //            pmDiasEntrega = i.Field<string>("CodDiaEnt"),
                        //            //Versión 003
                        //            pmAgIdSZona = i.Field<string>("IdSZonaA"),
                        //            pmAgSubzona = i.Field<string>("SubzonaA"),
                        //            pmAgIdZona = i.Field<string>("IdZonaA"),
                        //            pmAgZona = i.Field<string>("ZonaA"),
                        //            pmAgIdRuta = i.Field<string>("IdRutaA"),
                        //            pmAgRuta = i.Field<string>("RutaA")
                        //        }).ToList();
                        //    }
                        //    else
                        //        cliente.pmAgencias = null;
                        //}
                        //DataTable dtAgencias = tmpDt.AsEnumerable().Where(r => r.Field<string>("IdAgencia") != null).AsDataView().ToTable(true, "IdTercero,IdAgencia,Agencia,DirAgncia,AgIdLocal,AgLocalidad,TelAgncia,NomCont,emlCont,AgCdCms,AgTarCms,AgCdDct,AgTarDct,CodDiaEnt,IdSZonaA,SubzonaA,IdZonaA,ZonaA,IdRutaA,RutaA".Split(','));
                        clientes.ForEach(c =>
                        {
                            if (data.Where(r => r.Field<string>("IdTercero") == c.IdTercero && r.Field<string>("IdAgencia") != null).Count() > 0)
                            {
                                c.Agencias = new List<Agencia>();
                                data.Where(ca => ca.Field<string>("IdTercero") == c.IdTercero && ca.Field<string>("IdAgencia") != null).ToList().ForEach(i => c.Agencias.Add(new Agencia
                                {
                                    //if (dtAgencias.AsEnumerable().Where(r => r.Field<string>("IdTercero") == c.pmIdTercero).Count() > 0)
                                    //{
                                    //    c.pmAgencias = new List<Agencia>();
                                    //    dtAgencias.AsEnumerable().Where(ca => ca.Field<string>("IdTercero") == c.pmIdTercero).ToList().ForEach(i => c.pmAgencias.Add(new Agencia
                                    //    {
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
                                    //pmAgIdVend = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("AgIdVend")),
                                    //pmAgVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("AgVendedor")),
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
                    //LogErrores.tareas.Add("paso age");
                    //LogErrores.write();
                }
                catch (Exception ex)
                {
                    LogErrores.tareas.Add(ex.Message);
                    LogErrores.write();
                }

                //LogErrores.tareas.Add("total  agencias ==>" + clientes.Sum(x => x.pmAgencias.Count()));
                //LogErrores.write();

                //LogErrores.tareas.Add("despues de llenar la clase agencias ==>" + DateTime.Now);
                //LogErrores.write();

                resultado.Clientes = clientes;
                resultado.Registro = new Log { Codigo = "1", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "Se ejecutó correctamente la consulta." };
            }
            else
                resultado.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "No se encontró el usuario o la contraseña es incorrecta." };

            return resultado;
        }
    }
}
