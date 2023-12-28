using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using wcfSyscom.Model;

namespace wcfSyscom
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Servicio : IServicio
    {
        private Conexion con = new Conexion();
        private static object lockObject = new object();

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Clientes")]
        public RespClientes GetClientes(Usuario usuario)
        {
            //LogErrores.tareas.Add("ingreso ==>" + DateTime.Now.ToShortDateString());
            //LogErrores.write();
            RespClientes resultado = new RespClientes();
            List<ClienteResponse> clientes = new List<ClienteResponse>();
            if (existeUsuario(usuario))
            {
                DateTime pmFecha_Actual = DateTime.Parse(usuario.Fecha_Act);

                Conexion log = new Conexion();
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
                    con.addWhereAND("((ISNULL(tc.FechaUpdate, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or ISNULL(tc.FechaAdd, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "') or (ISNULL(t.FechaUpdate, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or ISNULL(t.FechaAdd, GETDATE()) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "')) and (tc.Inactivo = 0 and t.Inactivo = 0) AND tc.IdVend ='" + usuario.IdUsuario + "'" + (vendedoresNuevos.Count > 0 ? " or t.IdTercero in (" + String.Join(",", vendedoresNuevos.GroupBy(c => c.IdCliente).Select(c => c.First().IdCliente).ToList().Select(s => "'" + s + "'").ToArray()) + ")" : "" + ""));
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
                        pmPlazo = Convert.ToInt32(i.Field<string>("Plazo")),
                        pmCdCms = i.Field<string>("CdCms"),
                        pmTarCms = i.Field<decimal?>("TarCms"),
                        pmIdVend = i.Field<string>("IdVend"),
                        pmVendedor = i.Field<string>("Vendedor"),
                        //(vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("cliente") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("IdVend")),
                        //pmVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("cliente") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("Vendedor")),
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
                            if (data.Where(r => r.Field<string>("IdTercero") == c.pmIdTercero && r.Field<string>("IdAgencia") != null).Count() > 0)
                            {
                                c.pmAgencias = new List<Agencia>();
                                data.Where(ca => ca.Field<string>("IdTercero") == c.pmIdTercero && ca.Field<string>("IdAgencia") != null).ToList().ForEach(i => c.pmAgencias.Add(new Agencia
                                {
                                    //if (dtAgencias.AsEnumerable().Where(r => r.Field<string>("IdTercero") == c.pmIdTercero).Count() > 0)
                                    //{
                                    //    c.pmAgencias = new List<Agencia>();
                                    //    dtAgencias.AsEnumerable().Where(ca => ca.Field<string>("IdTercero") == c.pmIdTercero).ToList().ForEach(i => c.pmAgencias.Add(new Agencia
                                    //    {
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
                                    //pmAgIdVend = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("AgIdVend")),
                                    //pmAgVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("AgVendedor")),
                                    pmAgCdCms = i.Field<string>("AgCdCms"),
                                    pmAgTarCms = i.Field<decimal?>("AgTarCms"),
                                    pmAgCdDct = i.Field<string>("AgCdDct"),
                                    pmAgTarDct = i.Field<decimal?>("AgTarDct"),
                                    pmDiasEntrega = i.Field<string>("CodDiaEnt"),
                                    //Versión 003
                                    pmAgIdSZona = i.Field<string>("IdSZonaA"),
                                    pmAgSubzona = i.Field<string>("SubzonaA"),
                                    pmAgIdZona = i.Field<string>("IdZonaA"),
                                    pmAgZona = i.Field<string>("ZonaA"),
                                    pmAgIdRuta = i.Field<string>("IdRutaA"),
                                    pmAgRuta = i.Field<string>("RutaA")
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
                resultado.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "No se encontró el usuario." };

            return resultado;
        }

        #region CodAntes
        //public RespClientes GetClientes(Usuario usuario)
        //{
        //    //LogErrores.tareas.Add("ingreos");
        //    //LogErrores.write();
        //    RespClientes resultado = new RespClientes();
        //    List<ClienteResponse> clientes = new List<ClienteResponse>();
        //    if (existeUsuario(usuario))
        //    {
        //        DateTime pmFecha_Actual = DateTime.Parse(usuario.Fecha_Act);

        //        Conexion log = new Conexion();
        //        log.setConnection("dbsLog");
        //        log.qryFields.Add("Numero, Fecha, ClaveReg, TipoProc, IdCliente, CdAgencia, CdVendAnt, IdVend, FechaCrea, IdUsuario, Nombre, NomCliente, NomAgencia, NomVendedor, NomVendAnt");
        //        log.qryTables.Add("LogVendedores");
        //        log.addWhereAND("FechaCrea >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "'");
        //        log.qryOrderBy.Add("Numero desc");
        //        log.select();
        //        log.ejecutarQuery(_aliasConsulta: "_LogVendedores");
        //        DataTable tmpDt = log.getDataTable();
        //        List<LogVendedores> vendedoresNuevos = tmpDt.AsEnumerable().Select(r => new LogVendedores
        //        {
        //            Numero = r.Field<int>("Numero"),
        //            Fecha = r.Field<DateTime>("Fecha"),
        //            ClaveReg = r.Field<string>("ClaveReg"),
        //            TipoProc = r.Field<string>("TipoProc"),
        //            IdCliente = r.Field<string>("IdCliente"),
        //            CdAgencia = r.Field<string>("CdAgencia"),
        //            CdVendAnt = r.Field<string>("CdVendAnt"),
        //            IdVend = r.Field<string>("IdVend"),
        //            FechaCrea = r.Field<DateTime>("FechaCrea"),
        //            IdUsuario = r.Field<string>("IdUsuario"),
        //            Nombre = r.Field<string>("Nombre"),
        //            NomCliente = r.Field<string>("NomCliente"),
        //            NomAgencia = r.Field<string>("NomAgencia"),
        //            NomVendedor = r.Field<string>("NomVendedor"),
        //            NomVendAnt = r.Field<string>("NomVendAnt")
        //        }).ToList();

        //        //LogErrores.tareas.Add("paso vend");
        //        //LogErrores.write();
        //        try
        //        {
        //            con.resetQuery();
        //            con.qryFields.Add(@"t.IdTercero, t.RazonSocial, t.Direccion, l.IdLocal, l.Localidad, t.Telefono, tv.Codigo , rd.IdRegimen, rd.Regimen, tc.Inactivo, 
        //            tc.IdGrupo, tc.CdDiaEnt, tc.NumLista, se.IdSector, se.SectorEco, p.IdPlazo, p.DiasPago as Plazo, isnull(sz.IdSZona, '0') AS IdSZona, isnull(sz.Subzona, '0') AS Subzona, isnull(z.IdZona, '0') AS IdZona, isnull(z.Zona, '0') AS Zona, isnull(tc.IdRuta, '0') AS IdRuta, isnull(r.Ruta, '0') AS Ruta, 
        //            tp.IdTarifa as CdCms, tp.Tarifa as TarCms, tv.IdTercero as IdVend, tv.RazonSocial as Vendedor, 
        //            ag.IdAgencia, ag.Agencia, ag.DirAgncia, agl.IdLocal as AgIdLocal, agl.Localidad as AgLocalidad, ag.TelAgncia, ag.NomCont, ag.emlCont, agv.IdTercero as AgIdVend, 
        //            agv.RazonSocial as AgVendedor, agtp.IdTarifa as AgCdCms, agtp.Tarifa as AgTarCms, agdc.IdTarifa as AgCdDct, agdc.Tarifa as AgTarDct, ag.CodDiaEnt,
        //            isnull(sza.IdSzona, '0') AS IdSzonaA, isnull(sza.Subzona, '0') AS SubzonaA, isnull(za.IdZona,'0') AS IdZonaA, isnull(za.Zona, '0') AS ZonaA, isnull(ag.CodRuta, '0') AS IdRutaA, isnull(ra.Ruta, '0') as RutaA
        //            ");
        //            con.qryTables.Add(@"terccliente tc
        //            join terceros t on (tc.IdClie = t.IdTercero)
        //            join localidades l on (l.IdLocal = t.IdLocal)
        //            join Terceros tv on (tv.IdTercero = tc.IdVend)
        //            join SectoresEco se on (se.IdSector = t.IdSector)
        //            join Plazos p on (p.IdPlazo = tc.IdPlazo)
        //            join SubZonas sz on (sz.IdSzona = tc.IdSzona)
        //            join Zonas as z on (sz.IdZona = z.IdZona)
        //            join Rutas as r on (tc.IdRuta = r.IdRuta)
        //            join RegimenDian rd on (rd.IdRegimen = t.IdRegimen)
        //            left join TablaPor tp on (tp.IdTarifa = tc.CdCms and tp.IdClase = 'COM')
        //            left join Agencias ag on (tc.IdClie = ag.IdClie)
        //            left join SubZonas sza on (sza.IdSzona = ag.IdSzona)
        //            left join Zonas as za on (sza.IdZona = za.IdZona)
        //            left join Rutas as ra on (ag.CodRuta = ra.IdRuta)
        //            left join Localidades agl on (agl.IdLocal = ag.IdLocal)
        //            left join Terceros agv on (agv.IdTercero = ag.IdVend)
        //            left join Tablapor agtp on (agtp.IdTarifa = ag.CdCms and agtp.IdClase = 'COM')
        //            left join Tablapor agdc on (agdc.IdTarifa = ag.CdDct and agtp.IdClase = 'DCT')");
        //            con.addWhereAND("((tc.FechaUpdate >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or tc.FechaAdd >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "') or (t.FechaUpdate >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' and t.FechaAdd >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "'))" + (vendedoresNuevos.Count > 0 ? " or t.IdTercero in (" + String.Join(",", vendedoresNuevos.Select(s => "'" + s.IdCliente + "'").ToArray()) + ")" : "" + ""));
        //            //con.addWhereAND("((isnull(tc.FechaUpdate, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or isnull(tc.FechaAdd, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "') or (isnull(t.FechaUpdate, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "' or isnull(t.FechaAdd, '" + pmFecha_Actual.ToString("yyyyMMdd") + "') >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "'))" + (vendedoresNuevos.Count > 0 ? " or t.IdTercero in (" + String.Join(",", vendedoresNuevos.Select(s => "'" + s.IdCliente + "'").ToArray()) + ")" : "" + ""));
        //            con.select();
        //            con.ejecutarQuery();
        //            tmpDt = con.getDataTable();
        //            //DataTable dtFiltrado = tmpDt.Copy().AsDataView().ToTable(true, "IdTercero,RazonSocial,Direccion,IdLocal,Localidad,Telefono,IdRegimen,Regimen,IdGrupo,IdSector,SectorEco,IdPlazo,Plazo,IdSZona,SubZona,IdZona,Zona,IdRuta,Ruta,CdCms,TarCms,CdDiaEnt,NumLista,Codigo,Inactivo".Split(','));
        //            clientes = (from i in tmpDt.Copy().AsEnumerable()
        //                        select new ClienteResponse
        //                        {
        //                            pmIdTercero = i.Field<string>("IdTercero"),
        //                            pmRazonSocial = i.Field<string>("RazonSocial"),
        //                            pmDireccion = i.Field<string>("Direccion"),
        //                            pmIdLocal = i.Field<string>("IdLocal"),
        //                            pmLocalidad = i.Field<string>("Localidad"),
        //                            pmTelefono = i.Field<string>("Telefono"),
        //                            pmIdRegimen = i.Field<string>("IdRegimen"),
        //                            pmRegimen = i.Field<string>("Regimen"),
        //                            pmIdGrupo = i.Field<string>("IdGrupo"),
        //                            pmIdSector = i.Field<string>("IdSector"),
        //                            pmSectorEco = i.Field<string>("SectorEco"),
        //                            pmIdPlazo = i.Field<string>("IdPlazo"),
        //                            pmPlazo = Convert.ToInt32(i.Field<string>("Plazo")),
        //                            pmIdSZona = i.Field<string>("IdSZona"),
        //                            pmSubzona = i.Field<string>("SubZona"),
        //                            pmIdZona = i.Field<string>("IdZona"),
        //                            pmZona = i.Field<string>("Zona"),
        //                            pmIdRuta = i.Field<string>("IdRuta"),
        //                            pmRuta = i.Field<string>("Ruta"),
        //                            pmCdCms = i.Field<string>("CdCms"),
        //                            pmTarCms = i.Field<decimal?>("TarCms"),
        //                            pmIdVend = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("cliente") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("IdVend")),
        //                            pmVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("cliente") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("Vendedor")),
        //                            pmDiasEntrega = i.Field<string>("CdDiaEnt"),
        //                            pmNumLista = i.Field<string>("NumLista"),
        //                            pmCodVend = i.Field<string>("Codigo"),
        //                            pmInactivo = (i.Field<bool>("Inactivo") ? "1" : "0")
        //                        }).GroupBy(a => a.pmIdTercero).Select(g => g.First()).ToList();

        //            //LogErrores.tareas.Add("paso clientes");
        //            //LogErrores.write();
        //            try
        //            {
        //                foreach (ClienteResponse cliente in clientes)
        //                {
        //                    if (tmpDt.AsEnumerable().Where(r => r.Field<string>("IdTercero") == cliente.pmIdTercero && r.Field<string>("IdAgencia") != null).Count() > 0)
        //                    {
        //                        cliente.pmAgencias = (from i in tmpDt.AsEnumerable()
        //                                              where i.Field<string>("IdTercero") == cliente.pmIdTercero
        //                                              select new Agencia
        //                                              {
        //                                                  pmIdAgencia = i.Field<string>("IdAgencia"),
        //                                                  pmNomAgencia = i.Field<string>("Agencia"),
        //                                                  pmDirAgncia = i.Field<string>("DirAgncia"),
        //                                                  pmAgIdLocal = i.Field<string>("AgIdLocal"),
        //                                                  pmAgLocalidad = i.Field<string>("AgLocalidad"),
        //                                                  pmTelAgncia = i.Field<string>("TelAgncia"),
        //                                                  pmNomCont = i.Field<string>("NomCont"),
        //                                                  pmemlCont = i.Field<string>("emlCont"),
        //                                                  pmAgIdVend = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().IdVend : i.Field<string>("AgIdVend")),
        //                                                  pmAgVendedor = (vendedoresNuevos.Where(f => (f.TipoProc.ToLower().Equals("agencia") || f.TipoProc.ToLower().Equals("vendedor")) && f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).Count() > 0 ? vendedoresNuevos.Where(f => f.IdCliente.Equals(i.Field<string>("IdTercero")) && f.CdAgencia.Equals("0")).OrderByDescending(t => t.FechaCrea).FirstOrDefault().NomVendedor : i.Field<string>("AgVendedor")),
        //                                                  pmAgCdCms = i.Field<string>("AgCdCms"),
        //                                                  pmAgTarCms = i.Field<decimal?>("AgTarCms"),
        //                                                  pmAgCdDct = i.Field<string>("AgCdDct"),
        //                                                  pmAgTarDct = i.Field<decimal?>("AgTarDct"),
        //                                                  pmDiasEntrega = i.Field<string>("CodDiaEnt"),
        //                                                  //Versión 003
        //                                                  pmAgIdSZona = i.Field<string>("IdSZonaA"),
        //                                                  pmAgSubzona = i.Field<string>("SubzonaA"),
        //                                                  pmAgIdZona = i.Field<string>("IdZonaA"),
        //                                                  pmAgZona = i.Field<string>("ZonaA"),
        //                                                  pmAgIdRuta = i.Field<string>("IdRutaA"),
        //                                                  pmAgRuta = i.Field<string>("RutaA")
        //                                              }).ToList();
        //                    }
        //                    else
        //                        cliente.pmAgencias = null;
        //                }
        //            }
        //            catch (Exception exx)
        //            {
        //                LogErrores.tareas.Add(exx.Message);
        //                LogErrores.write();
        //            }
        //            //LogErrores.tareas.Add("paso age");
        //            //LogErrores.write();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogErrores.tareas.Add(ex.Message);
        //            LogErrores.write();
        //        }

        //        resultado.Clientes = clientes;
        //        resultado.Registro = new Log { Codigo = "1", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "Se ejecutó correctamente la consulta." };
        //    }
        //    else
        //    {
        //        resultado.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = clientes.Count, Msg = "No se encontró el usuario." };
        //    }


        //    return resultado;
        //}
        #endregion

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Productos")]
        public RespProductos GetProductos(Usuario usuario)
        {
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            RespProductos productos = new RespProductos();
            List<Producto> prods = new List<Producto>();
            if (existeUsuario(usuario))
            {
                DateTime pmFecha_Actual = DateTime.Parse(usuario.Fecha_Act);
                con.resetQuery();
                con.qryFields.Add(@"p.IdProducto, p.DescripProd, g.IdLinea, g.IdGrupo, p.IdSubGrupo, p.IdMarca, p.IvaInc, p.LtPreDef, pp.VrPrecio1, pp.VrPrecio2, pp.VrPrecio3, pp.VrPrecio4, pp.VrPrecio5, p.ExcluidoImp, tp.Tarifa, tp.Simbolo, uo.Valor as IdCia");
                con.qryTables.Add(@"ProdMcias p join adm_UOpciones uo on (uo.NomOpcion = 'COMPANIA' and lower(uo.IdUsuario)=lower('" + usuario.IdUsuario + "')) left join ProdPrecios pp on (p.IdProducto = pp.IdProducto and pp.IdCia = uo.Valor) join tablapor tp on (pp.CdTarIva = tp.IdTarifa and tp.IdClase = 'IVA') join SubGrupos sg on (p.IdSubgrupo = sg.IdSubGrupo) join Grupos g on (g.IdGrupo = sg.IdGrupo)");
                con.addWhereAND("p.Inactivo = 0 and isnull(FechaUpdate,FechaAdd) >= '" + pmFecha_Actual.ToString("yyyyMMdd") + "'");
                con.select();
                con.ejecutarQuery();
                if (con.getDataTable().Rows.Count > 0)
                {
                    con.getDataTable().AsEnumerable().ToList().ForEach(i=>{
                        Producto prod = new Producto
                             {
                                 pmIdProducto = i.Field<string>("IdProducto"),
                                 pmDescripProd = i.Field<string>("DescripProd"),
                                 pmIvaInc = i.Field<string>("IvaInc"),
                                 pmLtPreDef = i.Field<string>("LtPreDef"),
                                 pmVrPrecio1 = i.Field<string>("IvaInc").Contains("1") ? i.Field<decimal>("VrPrecio1") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio1"),
                                 pmVrPrecio2 = i.Field<string>("IvaInc").Contains("2") ? i.Field<decimal>("VrPrecio2") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio2"),
                                 pmVrPrecio3 = i.Field<string>("IvaInc").Contains("3") ? i.Field<decimal>("VrPrecio3") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio3"),
                                 pmVrPrecio4 = i.Field<string>("IvaInc").Contains("4") ? i.Field<decimal>("VrPrecio4") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio4"),
                                 pmVrPrecio5 = i.Field<string>("IvaInc").Contains("5") ? i.Field<decimal>("VrPrecio5") / ((i.Field<decimal>("Tarifa") / 100) + 1) : i.Field<decimal>("VrPrecio5"),
                                 pmExcluidoImp = i.Field<bool>("ExcluidoImp"),
                                 pmTarifaIva = i.Field<decimal>("Tarifa")
                             };

                        PrecioEspecial TarEsp = new PrecioEspecial();
                        TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 1, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                        if (TarEsp.Numero != null)
                            prod.pmVrPrecio1 = TarEsp.Tarifa.Value;

                        TarEsp = new PrecioEspecial();
                        TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 2, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                        if (TarEsp.Numero != null)
                            prod.pmVrPrecio2 = TarEsp.Tarifa.Value;

                        TarEsp = new PrecioEspecial();
                        TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 3, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                        if (TarEsp.Numero != null)
                            prod.pmVrPrecio3 = TarEsp.Tarifa.Value;

                        TarEsp = new PrecioEspecial();
                        TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 4, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                        if (TarEsp.Numero != null)
                            prod.pmVrPrecio4 = TarEsp.Tarifa.Value;

                        TarEsp = new PrecioEspecial();
                        TarEsp.ObtenerTarifaEspecial(pmFecha_Actual, 5, "", i.Field<string>("IdLinea"), i.Field<string>("IdGrupo"), i.Field<string>("IdSubGrupo"), i.Field<string>("IdMarca"), i.Field<string>("IdProducto"), "", "", "", usuario.IdUsuario, "", "", "", "", "");
                        if (TarEsp.Numero != null)
                            prod.pmVrPrecio5 = TarEsp.Tarifa.Value;

                        prods.Add(prod);
                    });
                    
                    prods.ForEach(r => {
                                 con.resetQuery();
                                 con.qryFields.Add(@"IdCia");
                                 con.qryTables.Add(@"ProdCompanias");
                                 con.addWhereAND("IdProducto = '" + r.pmIdProducto + "'");
                                 con.select();
                                 con.ejecutarQuery();
                                 r.pmDisponibleEnConpania = con.getDataTable().AsEnumerable().Select(s => s.Field<string>("IdCia")).ToList();
                             });

                    productos.Productos = JValue.Parse(JsonConvert.SerializeObject(prods.Select(r => new { r.pmIdProducto, r.pmDescripProd, r.pmIvaInc, r.pmLtPreDef, r.pmVrPrecio1, r.pmVrPrecio2, r.pmVrPrecio3, r.pmVrPrecio4, r.pmVrPrecio5, r.pmExcluidoImp, r.pmTarifaIva, r.pmDisponibleEnConpania }), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                }
                productos.Registro = new Log { Codigo = "1", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = prods.Count, Msg = "Se ejecutó correctamente la consulta." };

            }
            else
            {
                productos.Registro = new Log { Codigo = "USER_001", Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Registros = prods.Count, Msg = "No se encontró el usuario." };
            }

            return productos;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerInfMaestra", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Maestro")]
        public ResInfoMaestra GetInfMaestra(InfoMaestra Parametros)
        {
            ResInfoMaestra inf = new ResInfoMaestra();
            List<ClienteResponse> clientes = new List<ClienteResponse>();
            if (existeUsuario(Parametros.Usuario))
            {
                var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Conexion con = new Conexion();
                con.setConnection("Syscom");
                switch (Parametros.TipoRegistro)
                {
                    case 1:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdRegimen, regimen from RegimenDian where Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;
                    case 2:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdZona,Zona,CdSubCos from Zonas where Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;

                    case 3:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdGrupo, GrupoClie from GruposCli where modapp= 'cliente' and Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;

                    case 4:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdPlazo, Plazo, NVmto, diaspago from Plazos where Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;

                    case 5:
                        con.resetQuery();
                        con.setCustomQuery(@"select r.IdRuta, r.Ruta, r.idlocori as CdOrigen, lori.Localidad as MunOrigen, r.IdLocDes as CdDestino, ldes.Localidad as MunDestino from Rutas r join Localidades lori on (r.idlocori = lori.IdLocal) join Localidades ldes on (r.idlocdes = ldes.IdLocal) where r.Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;

                    case 6:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdCCosto, CCosto from CentroCosto where Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;

                    case 7:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdCCosto, CCosto from CentroCosto where Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;

                    case 8:
                        con.resetQuery();
                        con.setCustomQuery(@"select IdSector, SectorEco from sectoreseco where Inactivo = 0");
                        con.ejecutarQuery();
                        inf.Respuesta = JValue.Parse(JsonConvert.SerializeObject(con.getDataTable(), Formatting.Indented, serializerSettings)).ToString(Formatting.Indented);
                        break;
                }
            }
            return inf;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        public PedidoResponse SetPedido(PedidoRequest pedido)
        {            
            PedidoResponse resp = new PedidoResponse();
            lock (lockObject)
            {
                //metodo para serializacion 
                var serializerSettings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    // Otras configuraciones si es necesario
                };

                if (String.IsNullOrEmpty(pedido.cdAgencia))
                    pedido.cdAgencia = "0";

                DataTable Trn_OPedido = new DataTable();
                DataTable Trn_Kardex = new DataTable();               
                Pedido ped = new Pedido(pedido.Usuario.IdUsuario, pedido.Cliente, pedido.TipoPedido, pedido.cdAgencia);

                con = new Conexion();
                con.setConnection("Syscom");

                con.resetQuery();
                con.qryFields.Add("TipDoc,Pedido,IdCia,IdCliente");
                con.qryTables.Add("Trn_Opedido");
                con.addWhereAND("IdConcepto = '" + ped.ObtenerTipoPed(pedido.TipoPedido) + "' and IdCliente = '" + pedido.Cliente.Documento + " ' and IdAgencia = '" + pedido.cdAgencia + "' and TimeSys >= '" + DateTime.Now.ToString("yyyyMMdd") + "' and Observacion = 'Generado interfaz Web Services Incko'");
                con.select();
                con.ejecutarQuery();
                DataTable repetido = con.getDataTable();


                if (repetido.Rows.Count > 0)
                {
                    LogErrores.tareas.Add("============================================================================");
                    LogErrores.tareas.Add(JsonConvert.SerializeObject(pedido, Formatting.Indented, serializerSettings));
                    LogErrores.tareas.Add("Ya se ha realizado un pedido con las mismas caracteristicas al cliente " + repetido.Rows[0].Field<string>("IdCliente") + "'" + repetido.Rows[0].Field<string>("TipDoc") + "-" + repetido.Rows[0].Field<Int32>("Pedido") + "-" + repetido.Rows[0].Field<string>("IdCia") + "'");
                    LogErrores.write();
                    resp.Errores = "Ya se ha realizado un pedido con las mismas caracteristicas al cliente " + repetido.Rows[0].Field<string>("IdCliente") + "'" + repetido.Rows[0].Field<string>("TipDoc") + "-" + repetido.Rows[0].Field<Int32>("Pedido") + "-" + repetido.Rows[0].Field<string>("IdCia") + "'";
                    goto fin;
                }

                con.resetQuery();
                con.qryFields.Add("u.IdUsuario, u.Usuario, c.IdCia, c.Compania, c.FechaActual, c.IdCCosto, c.IdSubCos, l.IdLocal, l.Localidad, tv.IdTarCms, tv.IdSzona, tv.IdGrupo, tv.IdClase");
                con.qryTables.Add("adm_usuarios u join adm_UOpciones au on au.NomOpcion = 'COMPANIA' and au.IdUsuario = u.IdUsuario join Companias c on c.IdCia = au.Valor join localidades l on l.IdLocal = c.IdLocal join TercVendedor tv on tv.IdVend = u.IdUsuario");
                con.addWhereAND("u.IdUsuario = '" + pedido.Usuario.IdUsuario + "'");
                con.select();
                con.ejecutarQuery();
                DataTable adm_Usuario = con.getDataTable();


                DataTable productos = ped.obtenerProductos(pedido.Productos);
                List<string> inactivos = productos.Rows.Cast<DataRow>().Where(r => r.Field<bool>("Inactivo") == true).Select(r => r.Field<string>("IdProducto")).ToList();
                if (inactivos.Count == 0)
                {
                    if (productos.Rows.Cast<DataRow>().Where(r => !new string[] { "1000", "1001", "1002" }.Contains(r.Field<string>("IdProducto"))).Count() > 0 && pedido.TipoPedido == "A")
                    {
                        LogErrores.tareas.Add("============================================================================");
                        LogErrores.tareas.Add(JsonConvert.SerializeObject(pedido, Formatting.Indented, serializerSettings));
                        LogErrores.tareas.Add("Los pedidos tipo Abono, solo pueden contener productos con los codigos '1000', '1001' y '1002'");
                        LogErrores.write();
                        resp.Errores = "Los pedidos tipo Abono, solo pueden contener productos con los codigos '1000', '1001' y '1002'";
                        goto fin;
                    }

                    ped.setProductos(productos);

                    ped.Procesar();

                }

                if (ped.obtenerErrores().Count == 0)
                {
                    resp.Pedido = JsonConvert.SerializeObject(ped.OPedido.AsEnumerable().Select(r => new { TipDoc = r.Field<string>("TipDoc"), Pedido = r.Field<Int32>("Pedido"), Fecha = r.Field<DateTime>("Fecha").ToString("dd/MM/yyyy"), cdAgencia = pedido.cdAgencia, Cliente = r.Field<string>("IdCliente"), Cia = r.Field<string>("IdCia") }), Formatting.Indented, serializerSettings);
                }
                else
                {
                    LogErrores.tareas.Add("============================================================================");
                    LogErrores.tareas.Add(JsonConvert.SerializeObject(pedido, Formatting.Indented, serializerSettings));
                    LogErrores.tareas.Add(JsonConvert.SerializeObject(ped.obtenerErrores(), Formatting.Indented, serializerSettings));
                    LogErrores.write();
                }


                resp.Errores = JsonConvert.SerializeObject(ped.obtenerErrores(), Formatting.Indented, serializerSettings);
                if (inactivos.Count > 0)
                    resp.Errores = String.Join(@"\n", inactivos.Select(r => "El producto '" + r + "' está inactivo.").ToArray());

                //resp.Pedido = JsonConvert.SerializeObject(ped.OPedido, Formatting.Indented, serializerSettings);
                //resp.Productos = JsonConvert.SerializeObject(ped.Kardex, Formatting.Indented, serializerSettings);
                //resp.Errores = JsonConvert.SerializeObject(ped.obtenerErrores(), Formatting.Indented, serializerSettings);
                               
                fin:
                { }                               
                con.Close();
            }
            return resp;

        }

        private bool existeUsuario(Usuario usuario)
        { 
            bool existe=false;
            con.setConnection("Syscom");
            con.resetQuery();            
            con.qryFields.Add("IdUsuario");
            con.qryTables.Add("adm_Usuarios");
            con.addWhereAND("Inactivo = 0 and lower(IdUsuario) = lower('" + usuario.IdUsuario + "')");
            con.select();
            con.ejecutarQuery();
            if (con.getDataTable().Rows.Count > 0)
                existe = true;
            return existe;
        }

    }
}
