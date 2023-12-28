using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data.SqlClient;

namespace wcfSyscom.Model
{
    public class Pedido
    {
        public DataTable Kardex = new DataTable();
        public DataTable OPedido = new DataTable();
        #region Aprobacion
        public DataTable Aprobacion = new DataTable();
        #endregion
        public DataTable NuevoTercero = new DataTable();
        public DataTable NuevoCliente = new DataTable();
        private DataTable Productos = new DataTable();
        private List<string> Errores = new List<string>();
        private ClienteRequest Cliente = new ClienteRequest();
        private List<Producto> ProdRecibidos = new List<Producto>();
        private string Agencia = "0";
        private string Usuario = "";
        private string IdCia = "00";
        private string TipoPedido = "P";

        public Pedido(string pmUsuario = "", ClienteRequest Cliente = null, string pmTipoPedido = "P", string cdAgencia = "0")
        {
            this.Cliente = Cliente;
            this.Usuario = pmUsuario;
            this.TipoPedido = pmTipoPedido;
            this.Agencia = cdAgencia;
        }

        /*Obtiene los productos con el precio real*/
        public DataTable obtenerProductos(List<Producto> pmProductos)
        {
            DataTable _dtProductos = null;
            try
            {
                Conexion con = new Conexion();
                con.setConnection("Syscom");
                con.setCustomQuery(@"select top 0 'tarifaba' 'tarifaba', 'TieneImpSal' 'TieneImpSal', 'VrImpBu' 'VrImpBu', 'CadTarba' 'CadTarba',
pm.IdProducto,	pm.DescripProd,	pm.DescripAbrv,	pm.CodBarras,	pm.Referencia,	pm.TipoRef,	'' as IdLinea, '' as IdGrupo,	pm.IdSubgrupo,	pm.IdMarca,	pm.Color,	pm.Tamano,	pm.MedAlto,	pm.MedAncho,	pm.MedLargo,	pm.MedVolm,	pm.UndMed,	pm.IdUnd,	pm.IdUndP,	pm.CdUndS,	pm.CdUndE,	pm.CdUndEb,	pm.IdEmp,	pm.IdNat,	pm.IdMnjo,	pm.IdTmcia,	'' as IdBodega,	pm.IdUbic,	pm.DesUbic,	pm.IdProv,	pm.GarProv,	pm.GarClie,	pm.CdDctCom,	pm.VrCostAnt,	pm.VrCosto,	pm.VrCostPmd,	pm.IdTarIva, 0 as TarifaIva,	pm.IvaInc,	pm.LtPreDef, 0.0000 as VrPrecio,	'' as CdMoney,	pm.BaseMgn,	'' as CdMargen,	'' as CdDcto, 0 as TarifaDcto,	pm.CdTarIca, 	pm.CdTarRet,	pm.ExtciaMin,	pm.ExtciaMax,	pm.ExtciaAct,	'' as Factor,	pm.Seriales,	pm.Lotes,	pm.Combo,	pm.NoAjustes,	pm.Tanques,	pm.ValesComb,	pm.FecUltcom,	pm.FecUltVta,	pm.CodMcia,	pm.DescripLong,	pm.Cmntarios,	pm.PathFoto,	pm.CantTpv,	pm.IdEstado,	pm.Inactivo,	pm.FechaAdd,	pm.FechaUpdate,	pm.IdUsuario,	pm.TipoZonaFront,	pm.ExcluidoImp, 1 as NumLista, 0 as Cantidad, '' as Obsequio
from prodmcias pm");
                con.ejecutarQuery();
                this.Productos = con.getDataTable().Copy();
                _dtProductos = this.Productos.Clone();

                con.resetQuery();
                con.setCustomQuery("select Valor from adm_UOpciones where IdUsuario = '" + this.Usuario + "' and NomOpcion = 'COMPANIA'");
                con.ejecutarQuery();
                if (con.getDataTable().Rows.Count > 0)
                    IdCia = con.getDataTable().Rows[0][0].ToString();
                else
                    this.Errores.Add("El vendedor " + this.Usuario + " no tiene una compañia asignada");

                con.resetQuery();
                con.setCustomQuery("select Valor from adm_UOpciones where IdUsuario = '" + this.Usuario + "' and NomOpcion = 'BODEGA'");
                con.ejecutarQuery();
                string IdBodega = null;
                if (con.getDataTable().Rows.Count > 0)
                    IdBodega = con.getDataTable().Rows[0][0].ToString();


                foreach (Producto prod in pmProductos)
                {
                    con.resetQuery();
                    con.setCustomQuery(@"
                    select Id,Tipo,NumLista,CdMney as IdMoneda, Cddct as IdDcto from (
                    select top 1 1 as Id, 'Cliente' as Tipo, NumLista, CdMney, Cddct from TercCliente where IdClie = '" + this.Cliente.Documento + @"' and Inactivo = 0
                    UNION
                    select top 1 2 as Id, 'Usuario' as Tipo, Valor, null,null from adm_UOpciones where IdUsuario = '" + this.Usuario + @"' and NomOpcion = 'LISTA'
                    UNION
                    select top 1 3 as Id, case p.LtPredef when 'B'  then 'Bodega' else 'Producto' end as Tipo, case p.LtPredef when 'B'  then isnull(b.LtaPre,1) else ISNULL(p.LtPredef,1) end as NumLista, null,null
                    from ProdMcias p 
                    left join Bodegas b on (p.IdBodega = b.IdBodega)
                    where p.IdProducto = '" + prod.pmIdProducto + @"' and p.Inactivo = 0
                    ) NumLista order by Id
                ");
                    con.ejecutarQuery();
                    Int32 numLista = 0;
                    if (!Int32.TryParse(con.getDataTable().Rows[0].Field<string>("NumLista"), out numLista))
                        numLista = 2;

                    DataTable _tmpDt = this.getProducto(prod.pmIdProducto, this.IdCia, IdBodega, numLista, prod.pmCantidad, prod.pmEsObsequio, this.Cliente.Documento, con);

                    if (con.getDataTable().Rows.Count > 0)
                        _dtProductos.Rows.Add(con.getDataTable().Rows[0].ItemArray);
                }
            }
            catch (Exception ex)
            {
                Errores.Add("Ha ocurrido un error al intentar obtener la lista de precios: " + ex.Message);
            }

            return _dtProductos;
        }

        public void setProductos(DataTable _productos)
        {
            this.Productos = _productos;
        }

        public void Guardar()
        {
            try
            {
                Conexion con = new Conexion();
                con.setConnection("Syscom");
                con.insertDataTable("trn_opedido", this.OPedido);
                con.insertDataTable("trn_kardex", this.Kardex);
            }
            catch (Exception ex)
            {
                Errores.Add("Ha ocurrido un error al intentar guardar: " + ex.Message);
            }
        }

        public void Procesar()
        {
            Conexion con = new Conexion();
            if (this.IdCia != "00")
                try
                {
                    con.setConnection("Syscom");
                    /*Se importan los campos necesarios para grabar el pedido partiendo de las tablas OPedido y Kardex*/
                    #region Estructura de los registros
                    con.resetQuery();
                    con.setCustomQuery(@"select top 0 * from Trn_Kardex");
                    con.ejecutarQuery();
                    this.Kardex = con.getDataTable().Copy();

                    con.resetQuery();
                    con.setCustomQuery(@"select top 0 * from Trn_Opedido");
                    con.ejecutarQuery();
                    this.OPedido = con.getDataTable().Copy();

                    #region Aprobacion
                    /*
                    con.resetQuery();
                    con.setCustomQuery(@"select top 0 * from Trn_Aprobacion");
                    con.ejecutarQuery();
                    this.Aprobacion = con.getDataTable().Copy();
                    */
                    #endregion
                    #endregion

                    con.beginTran();
                inicio:

                    /*Se obtienen las variables comunes para el encabezado y detalle del pedido*/
                    #region VariablesComunes
                    con.resetQuery();
                    con.setCustomQuery(@"select a.IdAgencia, a.IdClie, a.DirAgncia, a.NitCont, a.NomCont, a.TelAgncia, a.EmlCont, a.CargoCont, a.IdForma, a.IdPlazo, a.CdCCBonif, a.CdSubCCBonif, a.IdLocal, l.Localidad, a.IdSZona, a.IdVend, a.CdCms, a.CodRuta, tp.Tarifa from Agencias a left join tablapor tp on (a.CdCms = tp.IdTarifa and tp.IdClase = 'COM') join Localidades l on (a.IdLocal = l.IdLocal) where IdClie = '" + this.Cliente.Documento + "' and IdAgencia = '" + this.Agencia + "'");
                    con.ejecutarQuery();
                    DataTable InfAgencia = con.getDataTable().Copy();

                    con.resetQuery();
                    con.setCustomQuery(@"select tc.IdClie, tc.DirEnv, tc.NitContac, tc.NomContac, tc.TelContac, tc.emlContac, tc.CargContac, tc.IdForma, tc.IdPlazo, tc.CodCCosto, tc.CodSubCosto, tc.IdLocEnv, l.Localidad, tc.IdSZona, tc.NitFact, tc.IdVend, tc.CdCms, tp.Tarifa, tc.NumLista, tc.IdRuta, tc.CdMney, tc.DiasEntga, tc.IdEstado from TercCliente tc left join tablapor tp on (tc.CdCms = tp.IdTarifa and tp.IdClase = 'COM') join Localidades l on (tc.IdLocEnv = l.IdLocal) where IdClie = '" + this.Cliente.Documento + "'");
                    con.ejecutarQuery();
                    DataTable InfCliente = con.getDataTable().Copy();

                    con.resetQuery();
                    con.setCustomQuery(@"select IdCCosto, IdSubCos, FechaActual from Companias where IdCia = '" + this.IdCia + "'");
                    con.ejecutarQuery();
                    DataTable InfCompania = con.getDataTable().Copy();

                    con.resetQuery();
                    con.setCustomQuery(@"select t.IdLocal, l.Localidad, t.Direccion, t.RazonSocial from Terceros t join Localidades l on (t.IdLocal = l.IdLocal) where IdTercero = '" + this.Cliente.Documento + "'");
                    con.ejecutarQuery();
                    DataTable InfTercero = con.getDataTable().Copy();

                    con.resetQuery();
                    con.setCustomQuery(@"select tv.IdTarCms, tp.Tarifa from TercVendedor tv left join tablapor tp on (tv.IdTarCms = tp.IdTarifa and tp.IdClase = 'COM') where IdVend = '" + this.Usuario + "'");
                    con.ejecutarQuery();
                    DataTable InfVendedor = con.getDataTable().Copy();
                    if (InfVendedor.Rows.Count == 0)
                        this.Errores.Add("El vendedor no existe en la base de datos.");

//                    verificarDoc:
//                    con.resetQuery();
//                    con.setCustomQuery(@"select Numero + 1 as Numero from tiposdoccons where IdDoc = 'PED' and IdCia = '" + this.IdCia + "'  update tiposdoccons set Numero = Numero + 1 where IdDoc = 'PED' and IdCia = '" + this.IdCia + "'");
//                    con.ejecutarQuery();
//                    DataTable InfDocumento = con.getDataTable().Copy();
//                    if (InfDocumento.Rows.Count == 0)
//                    {
//                        con.resetQuery();
//                        con.setCustomQuery(@"insert into TiposDocCons (IdDoc,IdCia,LDesde,LHasta,Resolucion,RangoNum,Prefijo,Numero,NumManual,IntLotes,ConfigFecha,Formato,TipoPapel,Orientacion,VistaPrevia,VerSetup,NumCopias,FechaAdd)
//values ('PED','" + this.IdCia + @"',0,0,'','','',0,0,0,'','',1,1,1,0,1,'" + InfCompania.Rows[0].Field<DateTime>("FechaActual").ToString("dd/MM/yyyy") + "')");
//                        con.ejecutarQuery();
//                        goto verificarDoc;
//                    }

                    #region Aprobacion
                    /*
                verificarAprob:
                    con.resetQuery();
                    con.setCustomQuery(@"select Numero + 1 as Numero from tiposdoccons where IdDoc = 'APR' and IdCia = '" + this.IdCia + "'");
                    con.ejecutarQuery();
                    DataTable InfDocAprobacion = con.getDataTable().Copy();
                    if (InfDocAprobacion.Rows.Count == 0)
                    {
                        con.resetQuery();
                        con.setCustomQuery(@"insert into TiposDocCons (IdDoc,IdCia,LDesde,LHasta,Resolucion,RangoNum,Prefijo,Numero,NumManual,IntLotes,ConfigFecha,Formato,TipoPapel,Orientacion,VistaPrevia,VerSetup,NumCopias,FechaAdd)
    values ('APR','" + this.IdCia + @"',0,0,'','','',0,0,0,'','',1,1,1,0,1,'" + InfCompania.Rows[0].Field<DateTime>("FechaActual").ToString("dd/MM/yyyy") +"')");
                        con.ejecutarQuery();
                        goto verificarAprob;
                    }
                     */
                    #endregion

                    string CdCCosto = "";
                    string CdSubCos = "";
                    string CdLocal = "";
                    string Localidad = "";
                    string CdSZona = "";
                    string CdCms = "";
                    decimal TarifaCms = 0;
                    string DirEnv = "";
                    string NitContac = "";
                    string NomContac = "";
                    string TelContac = "";
                    string emlContac = "";
                    string CargContac = "";
                    string IdForma = "";
                    string IdPlazo = "";


                    Int32 Documento = 1;
                    Int32 DocAprobacion = 1;

                   

                    //Documento = InfDocumento.Rows[0].Field<Int32>("Numero");
                    #region Aprobacion
                    //                DocAprobacion = InfDocAprobacion.Rows[0].Field<Int32>("Numero");
                    #endregion Aprobacion
                    NitContac = "";
                    TelContac = "";
                    emlContac = "";
                    CargContac = "";
                    CdCCosto = InfCompania.Rows[0].Field<string>("IdCCosto");
                    CdSubCos = InfCompania.Rows[0].Field<string>("IdSubCos");

                    if (InfTercero.Rows.Count > 0)
                    {
                        CdLocal = InfTercero.Rows[0].Field<string>("IdLocal");
                        Localidad = InfTercero.Rows[0].Field<string>("Localidad");
                        DirEnv = InfTercero.Rows[0].Field<string>("Direccion");
                        NomContac = InfTercero.Rows[0].Field<string>("RazonSocial");
                        CdCms = InfVendedor.Rows[0].Field<string>("IdTarCms");
                        TarifaCms = InfVendedor.Rows[0].Field<decimal>("Tarifa");
                    }
                   

                    if (InfCliente.Rows.Count > 0)
                    {
                        CdSZona = InfCliente.Rows[0].Field<string>("IdSZona");
                        CdCCosto = InfCliente.Rows[0].Field<string>("CodCCosto") ?? CdCCosto;
                        CdSubCos = InfCliente.Rows[0].Field<string>("CodSubCosto") ?? CdSubCos;
                        CdLocal = InfCliente.Rows[0].Field<string>("IdLocEnv") ?? CdLocal;
                        Localidad = InfCliente.Rows[0].Field<string>("Localidad") ?? Localidad;
                        CdCms = InfCliente.Rows[0].Field<string>("CdCms") ?? CdCms;
                        TarifaCms = String.IsNullOrWhiteSpace(InfCliente.Rows[0].Field<string>("CdCms")) ? TarifaCms : InfCliente.Rows[0].Field<decimal>("Tarifa");
                        DirEnv = InfCliente.Rows[0].Field<string>("DirEnv") ?? DirEnv;
                        NitContac = InfCliente.Rows[0].Field<string>("NitContac") ?? NitContac;
                        NomContac = InfCliente.Rows[0].Field<string>("NomContac") ?? NomContac;
                        TelContac = InfCliente.Rows[0].Field<string>("TelContac") ?? TelContac;
                        emlContac = InfCliente.Rows[0].Field<string>("emlContac") ?? emlContac;
                        CargContac = InfCliente.Rows[0].Field<string>("CargContac") ?? CargContac;
                        IdForma = InfCliente.Rows[0].Field<string>("IdForma") ?? IdForma;
                        IdPlazo = InfCliente.Rows[0].Field<string>("IdPlazo") ?? IdPlazo;
                    }

                   

                    if (InfAgencia.Rows.Count > 0)
                    {
                        this.Agencia = InfAgencia.Rows[0].Field<string>("IdAgencia");
                        CdSZona = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("IdSZona")) ? InfAgencia.Rows[0].Field<string>("IdSZona") : CdSZona;
                        CdCCosto = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("CdCCBonif")) ? InfAgencia.Rows[0].Field<string>("CdCCBonif") : CdCCosto;
                        CdSubCos = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("CdSubCCBonif")) ? InfAgencia.Rows[0].Field<string>("CdSubCCBonif") : CdSubCos;
                        CdLocal = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("IdLocal")) ? InfAgencia.Rows[0].Field<string>("IdLocal") : CdLocal;
                        Localidad = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("Localidad")) ? InfAgencia.Rows[0].Field<string>("Localidad") : Localidad;
                        CdCms = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("CdCms")) ? InfAgencia.Rows[0].Field<string>("CdCms") : CdCms;
                        TarifaCms = String.IsNullOrWhiteSpace(InfAgencia.Rows[0].Field<string>("CdCms")) ? TarifaCms : InfAgencia.Rows[0].Field<decimal?>("Tarifa") ?? 0m;
                        DirEnv = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("DirAgncia")) ? InfAgencia.Rows[0].Field<string>("DirAgncia") : DirEnv;
                        NitContac = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("NitCont")) ? InfAgencia.Rows[0].Field<string>("NitCont") : NitContac;
                        NomContac = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("NomCont")) ? InfAgencia.Rows[0].Field<string>("NomCont") : NomContac;
                        TelContac = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("TelAgncia")) ? InfAgencia.Rows[0].Field<string>("TelAgncia") : TelContac;
                        emlContac = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("emlCont")) ? InfAgencia.Rows[0].Field<string>("emlCont") : emlContac;
                        CargContac = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("CargoCont")) ? InfAgencia.Rows[0].Field<string>("CargoCont") : CargContac;
                        IdForma = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("IdForma")) ? InfAgencia.Rows[0].Field<string>("IdForma") : IdForma;
                        IdPlazo = !String.IsNullOrEmpty(InfAgencia.Rows[0].Field<string>("IdPlazo")) ? InfAgencia.Rows[0].Field<string>("IdPlazo") : IdPlazo;
                    }
                    #endregion

                   
                    /*Verifica que el cliente exista, si no existe inserta el cliente de lo contrario procede a actualizar el pedido*/
                    if (InfCliente.Rows.Count == 0)
                    {
                        if (!String.IsNullOrEmpty(this.Cliente.ActEconomica) && !String.IsNullOrEmpty(this.Cliente.Documento) && !String.IsNullOrEmpty(this.Cliente.CentroCosto) && !String.IsNullOrEmpty(this.Cliente.DiasEntrega) && !String.IsNullOrEmpty(this.Cliente.Direccion) && !String.IsNullOrEmpty(this.Cliente.Nombres) && !String.IsNullOrEmpty(this.Cliente.Plazo))
                        {
                            /*Se inserta la informacion del cliente*/
                            #region agregarTercero
                            con.resetQuery();
                            con.qryFields.Add("top 0 IdTercero,RazonSocial,Codigo,TipoId,Dv,NomCial,SiglaRaz,Direccion,IdLocal,Telefono,Fax,TelMovil,SitioWeb,e_mail,EsCliente,EsVendedor,EsConductor,EsPropietario,EsProveedor,EsEmpleado,EsOperario,EsAccnista,EsCiaAseg,EsCliePres,IdSector,IdProf,IdRegimen,TipEnte,IdLugarCed,FecExpCed,Observacion,IniStgNom,IdEstado,Inactivo,FechaAdd,FechaUpdate,IdUsuario,ImgFoto,ImgFirma");
                            con.qryTables.Add("Terceros");
                            con.select();
                            con.ejecutarQuery();
                            DataTable Terceros = con.getDataTable();
                            DataRow dr = Terceros.NewRow();
                            dr["IdTercero"] = Cliente.Documento;
                            dr["RazonSocial"] = Cliente.Apellidos + " " + Cliente.Nombres;
                            dr["Codigo"] = Cliente.Documento;
                            dr["TipoId"] = Cliente.TipoDoc;
                            dr["Dv"] = CalcularDv(Cliente.Documento);
                            dr["NomCial"] = null;
                            dr["SiglaRaz"] = null;
                            dr["Direccion"] = Cliente.Direccion;
                            dr["IdLocal"] = Cliente.Municipio;
                            dr["Telefono"] = Cliente.Telefono;
                            dr["Fax"] = null;
                            dr["TelMovil"] = null;
                            dr["SitioWeb"] = null;
                            dr["e_mail"] = null;
                            dr["EsCliente"] = true;
                            dr["EsVendedor"] = false;
                            dr["EsConductor"] = false;
                            dr["EsPropietario"] = false;
                            dr["EsProveedor"] = false;
                            dr["EsEmpleado"] = false;
                            dr["EsOperario"] = false;
                            dr["EsAccnista"] = false;
                            dr["EsCiaAseg"] = false;
                            dr["EsCliePres"] = false;
                            dr["IdSector"] = Cliente.ActEconomica;
                            dr["IdProf"] = "0";
                            dr["IdRegimen"] = Cliente.RegimenDIAN;
                            dr["TipEnte"] = Cliente.TipEnte;
                            dr["IdLugarCed"] = Cliente.TipoDoc == "C" ? Cliente.MunExpedicion : null;
                            dr["FecExpCed"] = DBNull.Value;
                            dr["Observacion"] = "Usuario Importado desde Inkco";
                            dr["IniStgNom"] = Cliente.Apellidos.Length + 1;
                            dr["IdEstado"] = true;
                            dr["Inactivo"] = false;
                            dr["FechaAdd"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                            dr["FechaUpdate"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                            dr["IdUsuario"] = this.Usuario;
                            dr["ImgFoto"] = null;
                            dr["ImgFirma"] = null;
                            Terceros.Rows.Add(dr);
                            #endregion AgregarTerceros

                            this.NuevoTercero = Terceros;

                            #region agregarCliente
                            con.resetQuery();
                            con.qryFields.Add("top 0 IdClie,NitRepLeg,NomRepLeg,NitContac,NomContac,TelContac,emlContac,CargContac,DirEnv,IdLocEnv,DiasEntga,IdSzona,IdGrupo,IdPlazo,IdForma,IdEstrato,CdBandera,IdVend,NitFact,IdRuta,IdClase,NumCuenta,IdBanco,CdMney,CdDct,CdRet,CdRiv,CdCms,PlazosImp,ExcIva,TrfIntMora,DiasGracia,LiqFletes,FactSold,Autoret,IncRet,IncRiv,IncIca,FactTipo,VrCupo,VrSaldo,UidClie,PwdClie,Contrato,NContrato,CiaContMay,CodClieSicom,FecIngreso,FecVigencia,FecRetiro,MatMerc,FecMat,PathFoto,PathFirma,Cmntario1,Cmntario2,Cmntario3,PrendGarant,FecUpCupo,TipoCliente,IdEstado,Inactivo,FechaAdd,FechaUpdate,IdUsuario,Restric_Cia,CdPlazoComb,CupoGalones,FecPlazoDoc,EdoRadicaDoc,CdTipBloq,DescEdoDoc,ComIndustrial,NumLista,Termicas,CodRetCom,CodCCosto,CodSubCosto");
                            con.qryTables.Add("TercCliente");
                            con.select();
                            con.ejecutarQuery();
                            DataTable TercCliente = con.getDataTable();
                            dr = TercCliente.NewRow();
                            dr["IdClie"] = Cliente.Documento;
                            dr["NitRepLeg"] = Cliente.Documento;
                            dr["NomRepLeg"] = Cliente.Nombres + " " + Cliente.Apellidos;
                            dr["NitContac"] = Cliente.Documento;
                            dr["NomContac"] = Cliente.Nombres + " " + Cliente.Apellidos;
                            dr["TelContac"] = Cliente.Telefono;
                            dr["emlContac"] = "";
                            dr["CargContac"] = "";
                            dr["DirEnv"] = Cliente.Direccion;
                            dr["IdLocEnv"] = Cliente.Municipio;
                            dr["DiasEntga"] = Cliente.DiasEntrega;
                            dr["IdSzona"] = Cliente.Zona;
                            dr["IdGrupo"] = Cliente.Grupo;
                            dr["IdPlazo"] = Cliente.Plazo;
                            dr["IdForma"] = "9";
                            dr["IdEstrato"] = "0";
                            dr["CdBandera"] = "0";
                            dr["IdVend"] = this.Usuario;
                            dr["NitFact"] = Cliente.Documento;
                            dr["IdRuta"] = Cliente.Ruta;
                            dr["IdClase"] = "0";
                            dr["NumCuenta"] = "";
                            dr["IdBanco"] = "0";
                            dr["CdMney"] = "COP";
                            dr["CdDct"] = "";
                            dr["CdRet"] = "";
                            dr["CdRiv"] = "";
                            dr["CdCms"] = "";
                            dr["PlazosImp"] = Cliente.Plazo;
                            dr["ExcIva"] = false;
                            dr["TrfIntMora"] = -10;
                            dr["DiasGracia"] = 0;
                            dr["LiqFletes"] = 0;
                            dr["FactSold"] = 0;
                            dr["Autoret"] = 0;
                            dr["IncRet"] = 0;
                            dr["IncRiv"] = 0;
                            dr["IncIca"] = 0;
                            dr["FactTipo"] = 0;
                            dr["VrCupo"] = 500000;
                            dr["VrSaldo"] = 0;
                            dr["UidClie"] = "0";
                            dr["PwdClie"] = "";
                            dr["Contrato"] = false;
                            dr["NContrato"] = "0";
                            dr["CiaContMay"] = 10;
                            dr["CodClieSicom"] = "0";
                            dr["FecIngreso"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                            dr["FecVigencia"] = DBNull.Value;
                            dr["FecRetiro"] = DBNull.Value;
                            dr["MatMerc"] = "";
                            dr["FecMat"] = DBNull.Value;
                            dr["PathFoto"] = "";
                            dr["PathFirma"] = "";
                            dr["Cmntario1"] = "";
                            dr["Cmntario2"] = "";
                            dr["Cmntario3"] = "";
                            dr["PrendGarant"] = "";
                            dr["FecUpCupo"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                            dr["TipoCliente"] = "NONE";
                            dr["IdEstado"] = "1001";
                            dr["Inactivo"] = false;
                            dr["FechaAdd"] = DateTime.Now;
                            dr["FechaUpdate"] = DateTime.Now;
                            dr["IdUsuario"] = this.Usuario;
                            dr["Restric_Cia"] = false;
                            dr["CdPlazoComb"] = "4";
                            dr["CupoGalones"] = "0";
                            dr["FecPlazoDoc"] = DBNull.Value;
                            dr["EdoRadicaDoc"] = 1;
                            dr["CdTipBloq"] = "";
                            dr["DescEdoDoc"] = "";
                            dr["ComIndustrial"] = 0;
                            dr["NumLista"] = 1;
                            dr["Termicas"] = 0;
                            dr["CodRetCom"] = "";
                            dr["CodCCosto"] = InfCompania.Rows[0].Field<string>("IdCCosto");
                            dr["CodSubCosto"] = InfCompania.Rows[0].Field<string>("IdSubCos");
                            TercCliente.Rows.Add(dr);
                            #endregion agregarcliente

                            this.NuevoCliente = TercCliente;

                            try
                            {
                                con.insertDataTable("Terceros", this.NuevoTercero);
                                con.insertDataTable("Terccliente", this.NuevoCliente);
                            }
                            catch (Exception ex)
                            {
                                con.rollback();
                                this.Errores.Add(ex.Message);
                            }

                            goto inicio;
                        }
                        else
                            Errores.Add("Ha ocurrido un error al intentar generar el pedido: El Cliente no existe.");
                    }

                    /*Se almacena el detalle del pedido*/
                    #region Detalle
                    Int32 posRow = 0;
                    decimal TotalTarifaIva = 0;
                    decimal TotalVrPrecio = 0;
                    decimal TotalCantidad = 0;
                    decimal TotalTarifaDcto = 0;
                    //Impuesto saludable variables para calcular el valor total de los productos
                    decimal VrImpBU = 0;
                    decimal VrImpCup = 0;
                    

                    foreach (DataRow dr in Productos.Rows)
                    {
                        if (dr.Field<bool>("Inactivo"))
                            throw new Exception("El producto '" + dr.Field<string>("IdProducto") + "' está inactivo.");

                        posRow += 1;

                        PrecioEspecial TarEsp = new PrecioEspecial();
                        TarEsp.ObtenerTarifaEspecial(InfCompania.Rows[0].Field<DateTime>("FechaActual"), dr.Field<Int32>("NumLista"), IdCia, dr.Field<string>("IdLinea"), dr.Field<string>("IdGrupo"), dr.Field<string>("IdSubGrupo"), dr.Field<string>("IdMarca"), dr.Field<string>("IdProducto"), "", this.Cliente.Documento, Agencia, this.Usuario, CdLocal, "", CdSZona, "", "");
                        con.resetQuery();
                        con.setCustomQuery("select Tarifa from tablapor where IdClase = 'IVA' and IdTarifa = '" + dr.Field<string>("IdTarIva") + "'");
                        con.ejecutarQuery();

                        decimal TarifaIva = con.getDataTable().Rows[0].Field<decimal>("Tarifa");
                        decimal VrPrecio = TarEsp.Numero != null ? (TarEsp.SimbTfa == "$" ? TarEsp.Tarifa.Value : dr.Field<decimal>("VrPrecio") - (dr.Field<decimal>("VrPrecio") * (TarEsp.Tarifa.Value) / 100)) : dr.Field<decimal>("VrPrecio");
                        decimal Cantidad = dr.Field<Int32>("Cantidad");
                        string vall = (dr.Field<int?>("TarifaDcto") ?? 0).ToString();
                        decimal TarifaDcto = decimal.Parse(vall);
                       
                        
                        //Si es combo se debe acumular por item
                        if (dr.Field<bool>("Combo"))
                        {
                            Int32 numLista = dr.Field<Int32>("NumLista");

                            con.resetQuery();
                            con.setCustomQuery("SELECT IdProducto, IdProdBas, Cant FROM ProdCombo WHERE IdProducto = '" + dr.Field<string>("IdProducto") + "'");
                            con.ejecutarQuery();

                            decimal ComboVrIva = 0;
                            DataTable dtItems = con.getDataTable();
                            dtItems.Rows.Cast<DataRow>().ToList().ForEach(item =>
                            {

                                DataRow cmbDr = this.obtenerProductos(new List<Producto>() { new Producto { pmIdProducto = item.Field<string>("IdProdBas"), pmCantidad = Convert.ToInt32(item.Field<decimal>("Cant")) } }).Rows[0];
                                PrecioEspecial TarEspCombo = new PrecioEspecial();
                                TarEspCombo.ObtenerTarifaEspecial(InfCompania.Rows[0].Field<DateTime>("FechaActual"), numLista, IdCia, cmbDr.Field<string>("IdLinea"), cmbDr.Field<string>("IdGrupo"), cmbDr.Field<string>("IdSubGrupo"), cmbDr.Field<string>("IdMarca"), cmbDr.Field<string>("IdProducto"), "", this.Cliente.Documento, Agencia, this.Usuario, CdLocal, "", CdSZona, "", "");

                                decimal VrBaseIva = TarEspCombo.Numero != null ? (TarEspCombo.SimbTfa == "$" ? TarEspCombo.Tarifa.Value : cmbDr.Field<decimal>("VrPrecio") - (cmbDr.Field<decimal>("VrPrecio") * (TarEspCombo.Tarifa.Value) / 100)) : cmbDr.Field<decimal>("VrPrecio");

                                ComboVrIva += (VrBaseIva * (Convert.ToDecimal(cmbDr.Field<Int32>("TarifaIva")) / 100)) * item.Field<decimal>("Cant");
                            });
                        }


                        TotalTarifaIva += (VrPrecio * (TarifaIva / 100)) * Cantidad;
                        TotalVrPrecio += (VrPrecio * Cantidad);
                        TotalCantidad += Cantidad;
                        TotalTarifaDcto += (((VrPrecio * Cantidad) / 100) * TarifaDcto);

                        DataRow drKardex = this.Kardex.NewRow();
                        drKardex["TipDoc"] = "PED";
                        drKardex["Documento"] = Documento;
                        drKardex["IdCia"] = IdCia;
                        drKardex["Item"] = posRow;
                        drKardex["Fecha"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                        drKardex["IdProducto"] = dr.Field<string>("IdProducto");
                        drKardex["IdBodega"] = dr.Field<string>("IdBodega");
                        drKardex["CdTanque"] = "";
                        drKardex["Entradas"] = 0;
                        drKardex["Salidas"] = Cantidad;
                        drKardex["IdUnd"] = dr.Field<string>("IdUnd");
                        drKardex["VrUnitario"] = dr.Field<decimal>("VrCostPmd");
                        drKardex["VrPrecio"] = VrPrecio;
                        drKardex["VrCostProm"] = dr.Field<decimal>("VrCostPmd"); ;
                        drKardex["TarifaIva"] = TarifaIva;
                        drKardex["VrIvaEnt"] = 0;
                        drKardex["VrIvaSal"] = (((VrPrecio * Cantidad) - (((VrPrecio * Cantidad) / 100) * TarifaDcto)) / 100) * TarifaIva;
                        drKardex["TarifaDct"] = TarifaDcto;
                        drKardex["VrDctoEnt"] = 0;
                        drKardex["VrDctoSal"] = ((VrPrecio * Cantidad) / 100) * TarifaDcto;
                        drKardex["VrCostoEnt"] = 0;
                        drKardex["VrCostoSal"] = dr.Field<decimal>("VrCostPmd") * Cantidad;
                        drKardex["TarifaRet"] = 0;
                        drKardex["VrReteEnt"] = 0;
                        drKardex["VrReteSal"] = 0;
                        drKardex["TarifaIca"] = 0;
                        drKardex["VrIcaEnt"] = 0;
                        drKardex["VrIcaSal"] = 0;
                        drKardex["VrBruto"] = VrPrecio;
                        drKardex["CdUbic"] = "";
                        drKardex["NumLote"] = "";
                        //                drKardex["FechLote"] = DBNull.Value;
                        drKardex["IdConcepto"] = this.ObtenerTipoPed(this.TipoPedido);
                        drKardex["IdTercero"] = this.Cliente.Documento;
                        drKardex["CdAgencia"] = String.IsNullOrEmpty(this.Agencia) ? "0" : this.Agencia;
                        drKardex["CdCCosto"] = CdCCosto;
                        drKardex["CdSubCos"] = CdSubCos;
                        drKardex["CdLocal"] = CdLocal;
                        drKardex["CdSzona"] = CdSZona;
                        drKardex["pVehiculo"] = "";
                        drKardex["IdVend"] = this.Usuario;
                        drKardex["Comision"] = TarifaCms;
                        drKardex["CdOperario"] = "";
                        drKardex["ComisnOper"] = 0;
                        drKardex["Referencia"] = Convert.ToBoolean(dr.Field<string>("Obsequio")) ? "Producto Obsequio" : "";
                        drKardex["Descripcion"] = dr.Field<string>("DescripProd");
                        drKardex["Comptmntos"] = "";
                        drKardex["CdProdEquiv"] = "";
                        drKardex["TipOrd"] = "0";
                        drKardex["NumOrden"] = 0;
                        drKardex["IdCiaOrd"] = "00";
                        drKardex["Cotizacion"] = 0;
                        drKardex["IdCiaCot"] = "00";
                        drKardex["Remision"] = 0;
                        drKardex["IdCiaRem"] = 01;
                        drKardex["Factura"] = "";
                        drKardex["TipDocDev"] = 0;
                        drKardex["NumDocDev"] = 0;
                        drKardex["CdMngra"] = "";
                        drKardex["NumInicial"] = 0;
                        drKardex["NumFinal"] = 0;
                        drKardex["Sobretasa"] = 0;
                        drKardex["TasaNac"] = 0;
                        drKardex["TasaDep"] = 0;
                        drKardex["TasaMun"] = 0;
                        drKardex["Soldicom"] = 0;
                        drKardex["ImpGlobal"] = 0;
                        drKardex["OtroImpto"] = 0;
                        drKardex["Unidades"] = 0;
                        drKardex["ItemCombo"] = 0;
                        drKardex["Servcios"] = 0;
                        drKardex["NoVentas"] = 0;
                        drKardex["EsCombo"] = dr.Field<bool>("Combo");
                        drKardex["EsProdBase"] = 0;
                        drKardex["CodTarDct"] = dr.Field<string>("CdDcto");
                        drKardex["CodTarIva"] = dr.Field<string>("IdTarIva");
                        drKardex["CodTarIca"] = "";
                        drKardex["CodTarRet"] = "";
                        drKardex["CodTarCom"] = CdCms;
                        drKardex["CodTarCmc"] = "";
                        drKardex["ListaPrec"] = dr.Field<Int32>("NumLista");
                        drKardex["VrBase"] = VrPrecio;
                        drKardex["CdMoneda"] = dr.Field<string>("CdMoney");
                        drKardex["VrTasaCamb"] = 0;
                        drKardex["VrDivisa1"] = 0;
                        drKardex["VrDivisa2"] = 0;
                        drKardex["VrDivisa3"] = 0;
                        drKardex["Referencia2"] = "";
                        //              drKardex["FecOrden"] = null;
                        drKardex["galsbruto"] = 0;
                        drKardex["galsneto"] = 0;
                        drKardex["Temperatura"] = 0;
                        drKardex["UmTemp"] = "";
                        drKardex["Densidad"] = 0;
                        drKardex["TimeSys"] = DateTime.Now;
                        drKardex["IdUsuario"] = this.Usuario;
                        drKardex["Rec_Costo"] = 0;
                        drKardex["MgenCont"] = 0;
                        drKardex["VrImvCosto"] = 0;
                        drKardex["TarifaIco"] = 0;
                        drKardex["VrImpCon"] = 0;
                        drKardex["CantObseq"] = Convert.ToBoolean(dr.Field<string>("Obsequio")) ? Cantidad : 0;
                        drKardex["VrIvaObseq"] = Convert.ToBoolean(dr.Field<string>("Obsequio")) ? ((((VrPrecio * Cantidad) - (dr.Field<decimal>("VrCostPmd") * Cantidad)) / 100) * TarifaIva) : 0;
                        drKardex["BaseIvaCom"] = 0;
                        drKardex["ImpCarbono"] = 0;
                        drKardex["IngBaseCom"] = 0;
                        //4139
                        drKardex["TarifaStc"] = 0;
                        drKardex["SobtasaCons"] = 0;
                        //4145 agregan estos campos en kardex
                        drKardex["BaseIvp"] = 0;
                        drKardex["TarifaIvp"] = 0;
                        drKardex["IvaIngProd"] = 0;

                        if (dr.Field<string>("TieneImpSal") == "BA")
                        {

                            CultureInfo culture = new CultureInfo("es-ES");

                            VrImpBU += (Convert.ToDecimal(dr.Field<string>("VrImpBu"), culture) * Cantidad);
                            drKardex["TarifaIba"] = Convert.ToDecimal(dr.Field<string>("tarifaba"), culture);
                            drKardex["VrImpuBa"] = Convert.ToDecimal(dr.Field<string>("VrImpBu"), culture);
                            drKardex["CodTarBa"] = " ";
                            drKardex["TarifaCup"] = 0;
                            drKardex["VrImpuCup"] = 0;
                            drKardex["CodTarCup"] = "  ";


                        }
                        else if (dr.Field<string>("TieneImpSal") == "CUP")
                        {
                            CultureInfo culture = new CultureInfo("es-ES");

                            drKardex["TarifaIba"] = 0;
                            drKardex["VrImpuBa"] = 0;
                            drKardex["CodTarBa"] = " ";
                            drKardex["TarifaCup"] = Convert.ToDecimal(dr.Field<string>("tarifaba"), culture);
                            drKardex["VrImpuCup"] = (VrPrecio * Convert.ToDecimal(dr.Field<string>("tarifaba"), culture)) / 100;
                            drKardex["CodTarCup"] = dr.Field<string>("CadTarba");
                            VrImpCup += ((VrPrecio * Convert.ToDecimal(dr.Field<string>("tarifaba"), culture)) / 100) * Cantidad;
                        }
                        else
                        {
                            drKardex["TarifaIba"] = 0;
                            drKardex["VrImpuBa"] = 0;
                            drKardex["CodTarBa"] = " ";
                            drKardex["TarifaCup"] = 0;
                            drKardex["VrImpuCup"] = 0;
                            drKardex["CodTarCup"] = " ";
                        }

                        /*                    if (VrPrecio == 0 && !Convert.ToBoolean(dr.Field<string>("Obsequio")))
                                                this.Errores.Add("El valor del precio del producto " + dr.Field<string>("IdProducto") + " - " + dr.Field<string>("DescripProd") + " es 0. Comuniquese con el Administrador.");
                                            */
                        Kardex.Rows.Add(drKardex);
                    }
                    #endregion

                    
                    /*Se almacena el encabezado del pedido*/
                    #region Encabezado
                    DataRow drPedido = this.OPedido.NewRow();
                    drPedido["TipDoc"] = "PED";
                    drPedido["Pedido"] = Documento;
                    drPedido["IdCia"] = this.IdCia;
                    drPedido["Fecha"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                    drPedido["FechaVence"] = InfCompania.Rows[0].Field<DateTime>("FechaActual").AddDays(InfCliente.Rows[0].Field<Int32>("DiasEntga"));
                    drPedido["IdConcepto"] = this.ObtenerTipoPed(TipoPedido);
                    drPedido["IdCliente"] = this.Cliente.Documento;
                    drPedido["IdAgencia"] = String.IsNullOrEmpty(this.Agencia) ? "0" : this.Agencia;
                    if (string.IsNullOrEmpty(InfCliente.Rows[0].Field<string>("NitFact")))
                    {
                        drPedido["IdClieFact"] = this.Cliente.Documento;
                    }
                    else
                    {
                        drPedido["IdClieFact"] = InfCliente.Rows[0].Field<string>("NitFact");
                    }
                    drPedido["VrSubTotal"] = TotalVrPrecio;
                    drPedido["VrDescuento"] = TotalTarifaDcto;
                    drPedido["VrImpuesto"] = TotalTarifaIva;
                    drPedido["VrFletes"] = 0;
                    drPedido["VrOtros"] = 0;
                    drPedido["VrCargos"] = 0;
                    drPedido["VrOtrDcto"] = 0;
                    drPedido["VrSobretasa"] = 0;
                    drPedido["VrImpGlobal"] = 0;
                    drPedido["VrNeto"] = TotalVrPrecio - TotalTarifaDcto + TotalTarifaIva;
                    drPedido["Cantidad"] = TotalCantidad;
                    drPedido["IdVend"] = this.Usuario;
                    drPedido["TarifaCom"] = TarifaCms;
                    drPedido["CodTarCom"] = CdCms;
                    drPedido["DirEnvio"] = DirEnv;
                    drPedido["IdLocEnv"] = CdLocal;
                    drPedido["LugarEnvio"] = Localidad;
                    drPedido["DiasEntraga"] = InfCliente.Rows[0].Field<Int32>("DiasEntga");
                    drPedido["NitContac"] = NitContac;
                    drPedido["NomContac"] = NomContac;
                    drPedido["TelContac"] = TelContac;
                    drPedido["emlContac"] = emlContac;
                    drPedido["CargoContac"] = CargContac;
                    drPedido["IdForma"] = IdForma;
                    drPedido["DetallePago"] = "";
                    drPedido["MulPlazos"] = false;
                    drPedido["IdPlazo"] = IdPlazo;
                    drPedido["CdMney"] = InfCliente.Rows[0].Field<string>("CdMney");
                    drPedido["NitEmpTrans"] = "0";
                    drPedido["EmpTrans"] = "";
                    drPedido["AsignarVeh"] = false;
                    drPedido["pVehiculo"] = "0";
                    drPedido["CdConductor"] = "0";
                    drPedido["CdRuta"] = InfAgencia.Rows.Count > 0 ? InfAgencia.Rows[0].Field<string>("CodRuta") : InfCliente.Rows[0].Field<string>("IdRuta");
                    drPedido["ListaPrec"] = InfCliente.Rows[0].Field<string>("NumLista");
                    drPedido["RefPedido"] = this.ObtenerTipoPed(this.TipoPedido) == "PED3" ? "DEVOLUCION" : IdPlazo == "0001" ? "CONTADO" : "CREDITO";
                    drPedido["Modalidad"] = "INVENTARIO";
                    drPedido["Vigencia"] = "NORMAL";
                    drPedido["NumAutoriza"] = 0;
                    drPedido["NumAutCupo"] = 0;
                    drPedido["NumAutCheq"] = 0;
                    drPedido["NumAprob"] = this.ObtenerTipoPed(this.TipoPedido) == "PED3" ? 0 : 1999999999;
                    drPedido["IdCiaApr"] = this.ObtenerTipoPed(this.TipoPedido) == "PED3" ? "00" : this.IdCia;
                    if (this.ObtenerTipoPed(this.TipoPedido) != "PED3")
                        drPedido["FecAprob"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                    drPedido["DetalleAprob"] = this.ObtenerTipoPed(this.TipoPedido) == "PED3" ? null : "APROBADO WS";
                    drPedido["CdUsuAprob"] = this.ObtenerTipoPed(this.TipoPedido) == "PED3" ? null : this.Usuario;
                    drPedido["TipFac"] = "0";
                    drPedido["Factura"] = 0;
                    drPedido["IdCiaFac"] = "00";
                    drPedido["FechaFact"] = DBNull.Value;
                    drPedido["TipRem"] = "0";
                    drPedido["Remision"] = 0;
                    drPedido["IdCiaRem"] = "00";
                    drPedido["FechaRem"] = DBNull.Value;
                    drPedido["OrigenAdd"] = "WS";
                    drPedido["ZonaFrontera"] = 0;
                    drPedido["TipoTrans"] = 0;
                    drPedido["TipoOrden"] = this.ObtenerTipoPed(this.TipoPedido) == "PED4" ? "D" : "";
                    drPedido["TipoModifica"] = "";
                    drPedido["Anulado"] = false;
                    drPedido["FecDev"] = DBNull.Value;
                    drPedido["Observacion"] = "Generado interfaz Web Services Incko";
                    drPedido["IdEstado"] = this.ObtenerTipoPed(this.TipoPedido) == "PED3" ? "0002" : "0001";
                    drPedido["TimeSys"] = DateTime.Now;
                    drPedido["FecUpdate"] = DBNull.Value;
                    drPedido["IdCiaCrea"] = this.IdCia;
                    drPedido["IdUsuario"] = this.Usuario;
                    drPedido["NumCotizac"] = 0;
                    drPedido["CdCiaCotizac"] = "00";
                    drPedido["VrImpCarbono"] = 0;
                    drPedido["BaseIvaIgp"] = 0;
                    drPedido["VrIvaIngProd"] = 0;
                    //impusto saludable
                    drPedido["VrImpuBA"] = VrImpBU;
                    drPedido["VrImpuCUP"] = VrImpCup;
                    this.OPedido.Rows.Add(drPedido);
                    #endregion

                    /*Cambios/

                    /*Se almacena la informacion de la aprobacion*/
                    #region Aprobacion
                    /*
                    if (!this.EsDevolucion)
                    {
                    DataRow drAprobacion = this.Aprobacion.NewRow();
                    drAprobacion["TipDoc"] = "APR";
                    drAprobacion["Aprobacion"] = DocAprobacion;
                    drAprobacion["IdCia"] = this.IdCia;
                    drAprobacion["Fecha"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                    drAprobacion["TipoPed"] = "PED";
                    drAprobacion["Pedido"] = Documento;
                    drAprobacion["IdCiaPed"] = this.IdCia;
                    drAprobacion["FecPedido"] = InfCompania.Rows[0].Field<DateTime>("FechaActual");
                    drAprobacion["IdCliente"] = this.Cliente.Documento;
                    drAprobacion["IdAgencia"] = this.Agencia;
                    drAprobacion["pVehiculo"] = "0";
                    drAprobacion["VrCupoCred"] = 0;
                    drAprobacion["VrSaldoAct"] = 0;
                    drAprobacion["ChequesDev"] = false;
                    drAprobacion["ClienteMora"] = false;
                    drAprobacion["OrigenAdd"] = "WS";
                    drAprobacion["Anulado"] = false;
                    drAprobacion["FecDev"] = DBNull.Value;
                    drAprobacion["Observacion"] = "Aprobación WS";
                    drAprobacion["IdEstado"] = "0001";
                    drAprobacion["TimeSys"] = DateTime.Now;
                    drAprobacion["FecUpdate"] = DBNull.Value;
                    drAprobacion["IdCiaCrea"] = this.IdCia;
                    drAprobacion["IdUsuario"] = this.Usuario;
                    this.Aprobacion.Rows.Add(drAprobacion);
                    }
                    */
                    #endregion


                    if (this.Kardex.Rows.Count == 0)
                        this.Errores.Add("No se han encontrado los productos solicitados.");

                    if ((InfCliente.Rows[0].Field<string>("IdEstado") == "1001" || InfCliente.Rows[0].Field<string>("IdEstado") == "9999"))
                        this.Errores.Add("No se pueden generar pedidos a clientes con estado inactivo o radicado, estado actual del cliente: " + InfCliente.Rows[0].Field<string>("IdEstado"));

                    if (this.Errores.Count == 0)
                    {
                        DataTable TableOpedido = this.OPedido;
                        DataTable TableKadex = this.Kardex;
                        
                        ////realizamos la insercion en la base de datos 
                        DataSet TablaInsert = new DataSet();
                        List<SqlParameter> parametro = new List<SqlParameter>();
                        
                        parametro.Add(new SqlParameter("@OpedidoTable", SqlDbType.Structured)
                        {
                            TypeName = "dbo.wcfPedidos_Opedidos", // Nombre del tipo de tabla definido por el usuario
                            Value = TableOpedido
                        });

                        parametro.Add(new SqlParameter("@KadexTable", SqlDbType.Structured)
                        {
                            TypeName = "dbo.wcfPedidos_Kadex", // Nombre del tipo de tabla definido por el usuario
                            Value = TableKadex
                        });

                        

                        Conexion conTD = new Conexion();
                        conTD.setConnection("Syscom");
                        conTD.resetQuery();

                        if (conTD.ejecutarQuery("wcfSycom_AgregarPedido", parametro, out TablaInsert, out string[] nuevoMennsajes, CommandType.StoredProcedure))
                        {
                            if (TablaInsert.Tables[0].Rows[0].Field<int>("Pedido") > 0)
                            {
                                drPedido["Pedido"] = TablaInsert.Tables[0].Rows[0].Field<int>("Pedido");
                                LogErrores.tareas.Add("Pedido No.  PED-" + TablaInsert.Tables[0].Rows[0].Field<int>("Pedido") + "-" + IdCia + "/" + this.ObtenerTipoPed(TipoPedido) + " Del cliente " + this.Cliente.Documento + " / " + (String.IsNullOrEmpty(this.Agencia) ? "0" : this.Agencia));
                                LogErrores.write();
                               
                            }
                            else
                            {
                                //LogErrores.tareas.Add("Err 1");
                                //LogErrores.write();
                                this.Errores.Add("Ha ocurrido un error al intentar generar el pedido");
                               
                            }

                        }
                        else
                        {
                            this.Errores.Add(nuevoMennsajes[1]);
                        }

                        conTD.Close();
                    }
                    else
                    {
                        this.Errores.Add("No ha sido posible guardar la información del pedido. Verifique y corrija los errores.");
                        con.rollback();
                    }
                }
                catch (Exception ex)
                {
                    Errores.Add("Ha ocurrido un error al intentar generar el pedido: " + ex.Message);
                    con.rollback();
                }
            con.Close();
        }

        public List<string> obtenerErrores()
        {
            return this.Errores;
        }

        public DataTable getProducto(string IdProducto, string IdCia, string IdBodega, int numLista, int Cantidad, bool EsObsequio, string IdCliente, Conexion con)
        {
            con.resetQuery();
            con.setCustomQuery(
            @"declare @BAOcup varchar (20),
		@Tarifa decimal(14, 4),
		@Impues decimal(14, 4),
		@CodTar varchar(20),
		@Producto varchar(20),
		@cantidad int,
		@IdCliente varchar(20),
		@calcu decimal(16, 4)
		
		set @Producto = '" + IdProducto + @"'
		set @cantidad = " + Cantidad + @"
        set @Tarifa = 0
		set @Impues = 0
        set @IdCliente = '" + IdCliente + @"'
		set @BaOcup = 'no'
if (not exists(select idClie from terccliente where ExcImpBACU = 1 and IdClie = @IdCliente))
		begin
			select @BaOcup = (select case 
			when ISNULL((select idproducto from ProdMcias where Procedencia = '1' and TipoIBAICU = '1' and idproducto = @Producto), '') != '' then 'BA'
			when ISNULL((select idproducto from ProdMcias where Procedencia = '1' and TipoIBAICU = '2' and idproducto =  @Producto), '') != '' then 'CUP'
			else 'no'
			end);

			if (@BAOcup = 'BA')
			begin
			select @calcu = (select ((CantAzucar * 100) / MedVolm) 'tarifaImpSal' from ProdMcias where Procedencia = '1' and TipoIBAICU = '1' and IdProducto = @Producto) 
			select @tarifa = (select Tarifa from TarifasAzucar where nanno = YEAR(GETDATE()) and  ContAzuDesde <= @calcu and ContAzuHasta >= @calcu)
			select @Impues = (select ((MedVolm / 100)* @Tarifa) 'tarifa' from ProdMcias where Procedencia = '1' and TipoIBAICU = '1' and IdProducto = @Producto) 
			select @CodTar = ''
			end
			if (@BAOcup = 'CUP')
			begin
			select @Tarifa = (select Tarifa from Tablapor where IdTarifa = (select CdTarifIcu from SubGrupos where IdSubgrupo = (select IdSubgrupo from ProdMcias where IdProducto = @Producto))) 
			select @Impues = @Tarifa * @cantidad
			select @CodTar = (select CdTarifIcu from SubGrupos where IdSubgrupo = (select IdSubgrupo from ProdMcias where IdProducto = @Producto))
			end 
		end
if(exists(select IdProducto from prodprecios where IdProducto = '" + IdProducto + @"' and IdCia = '" + IdCia + @"' and VrPrecio" + numLista + @" > 0))
select  @Tarifa 'tarifaba', @BAOcup 'TieneImpSal', @Impues 'VrImpBu', @CodTar 'CadTarba', pm.IdProducto,	pm.DescripProd,	pm.DescripAbrv,	pm.CodBarras,	pm.Referencia,	pm.TipoRef,	l.IdLinea, g.IdGrupo, pm.IdSubgrupo,	pm.IdMarca,	pm.Color,	pm.Tamano,	pm.MedAlto,	pm.MedAncho,	pm.MedLargo,	pm.MedVolm,	pm.UndMed,	pp.IdUnd,	pm.IdUndP,	pm.CdUndS,	pm.CdUndE,	pm.CdUndEb,	pm.IdEmp,	pm.IdNat,	pm.IdMnjo,	pm.IdTmcia,	" + (String.IsNullOrWhiteSpace(IdBodega) ? "pp.IdBodega" : "'" + IdBodega + "' as IdBodega") + ",	pm.IdUbic,	pm.DesUbic,	pm.IdProv,	pm.GarProv,	pm.GarClie,	pm.CdDctCom,	pm.VrCostAnt,	pm.VrCosto,	isnull(pcos.CostoPmd,pm.VrCostPmd) as VrCostPmd,	pp.CdTarIva as IdTarIva, tiva.Tarifa as TarifaIva,	pp.IvaInc,	pm.LtPreDef,	pp.VrPrecio" + numLista + " as VrPrecio, pp.CdMoney" + numLista + @" as CdMoney,	pp.BaseMargen,	pp.CdMargen" + numLista + @" as CdMargen,	pp.CdDcto" + numLista + @" as CdDcto, tdct.Tarifa as TarifaDcto,	pm.CdTarIca,	pm.CdTarRet,	pm.ExtciaMin,	pm.ExtciaMax,	pm.ExtciaAct,	pm.Factor" + numLista + @" as Factor,	pm.Seriales,	pm.Lotes,	pm.Combo,	pm.NoAjustes,	pm.Tanques,	pm.ValesComb,	pm.FecUltcom,	pm.FecUltVta,	pm.CodMcia,	pm.DescripLong,	pm.Cmntarios,	pm.PathFoto,	pm.CantTpv,	pm.IdEstado,	pm.Inactivo,	pm.FechaAdd,	pm.FechaUpdate,	pm.IdUsuario,	pm.TipoZonaFront,	pm.ExcluidoImp, " + numLista + @" as NumLista, " + Cantidad + @" as Cantidad, '" + EsObsequio + @"' as Obsequio
from
ProdMcias pm join
ProdPrecios pp on (pm.IdProducto = pp.IdProducto and pp.IdCia = '" + IdCia + @"') join
SubGrupos sg on (pm.IdSubGrupo = sg.IdSubGrupo) join
Grupos g on (sg.IdGrupo = g.IdGrupo) join
Lineas l on (g.IdLinea = l.IdLinea)
left join TablaPor tiva on (pp.CdTarIva = tiva.IdTarifa and tiva.IdClase = 'IVA') 
left join TablaPor tdct on (pp.CdDcto" + numLista + @" = tdct.IdTarifa and tdct.IdClase = 'DCT')
left join ProdCostos pcos on (pm.IdProducto = pcos.IdProducto and pcos.IdCia = '" + IdCia + @"')
where pm.IdProducto = '" + IdProducto + @"'
else
select  @Tarifa 'tarifaba', @BAOcup 'TieneImpSal', @Impues 'VrImpBu', @CodTar 'CadTarba', pm.IdProducto,	pm.DescripProd,	pm.DescripAbrv,	pm.CodBarras,	pm.Referencia,	pm.TipoRef,	l.IdLinea, g.IdGrupo, pm.IdSubgrupo,	pm.IdMarca,	pm.Color,	pm.Tamano,	pm.MedAlto,	pm.MedAncho,	pm.MedLargo,	pm.MedVolm,	pm.UndMed,	pm.IdUnd,	pm.IdUndP,	pm.CdUndS,	pm.CdUndE,	pm.CdUndEb,	pm.IdEmp,	pm.IdNat,	pm.IdMnjo,	pm.IdTmcia,	" + (String.IsNullOrWhiteSpace(IdBodega) ? "pm.IdBodega" : "'" + IdBodega + "' as IdBodega") + ",	pm.IdUbic,	pm.DesUbic,	pm.IdProv,	pm.GarProv,	pm.GarClie,	pm.CdDctCom,	pm.VrCostAnt,	pm.VrCosto,	isnull(pcos.CostoPmd,pm.VrCostPmd) as VrCostPmd,	pm.IdTarIva, tiva.Tarifa as TarifaIva,	pm.IvaInc,	pm.LtPreDef,	pm.Precio" + numLista + @" as VrPrecio,	pm.CdMon" + numLista + @" as CdMoney,	pm.BaseMgn,	pm.CdMgn" + numLista + @" as CdMargen,	pm.CdDct" + numLista + " as CdDcto,	tdct.Tarifa as TarifaDcto, pm.CdTarIca,	pm.CdTarRet,	pm.ExtciaMin,	pm.ExtciaMax,	pm.ExtciaAct,	pm.Factor" + numLista + @" as Factor,	pm.Seriales,	pm.Lotes,	pm.Combo,	pm.NoAjustes,	pm.Tanques,	pm.ValesComb,	pm.FecUltcom,	pm.FecUltVta,	pm.CodMcia,	pm.DescripLong,	pm.Cmntarios,	pm.PathFoto,	pm.CantTpv,	pm.IdEstado,	pm.Inactivo,	pm.FechaAdd,	pm.FechaUpdate,	pm.IdUsuario,	pm.TipoZonaFront,	pm.ExcluidoImp, " + numLista + " as NumLista, " + Cantidad + @" as Cantidad, '" + EsObsequio + @"' as Obsequio from prodmcias pm join
SubGrupos sg on (pm.IdSubGrupo = sg.IdSubGrupo) join
Grupos g on (sg.IdGrupo = g.IdGrupo) join
Lineas l on (g.IdLinea = l.IdLinea)
left join ProdCostos pcos on (pm.IdProducto = pcos.IdProducto and pcos.IdCia = '" + IdCia + @"')
left join TablaPor tiva on (pm.IdTarIva = tiva.IdTarifa and tiva.IdClase = 'IVA') left join TablaPor tdct on (pm.CdDct" + numLista + @" = tdct.IdTarifa and tdct.IdClase = 'DCT') where pm.IdProducto = '" + IdProducto + @"'");
            con.ejecutarQuery();


            return con.getDataTable();
        }

        public string ObtenerTipoPed(string agTipoPed)
        {
            string resultado = "";
            switch (agTipoPed.ToUpper())
            {
                case "PED3":
                    resultado = "PED3";
                    break;
                case "PED4":
                    resultado = "PED4";
                    break;
                case "PED":
                    resultado = "PED";
                    break;
                case "PED5":
                    resultado = "PED5";
                    break;
                default:
                    throw new Exception("El tipo de pedido no existe.");

            }

            return resultado;
        }

        private Int32 CalcularDv(string nit)
        {
            Int32 x, y, z, i, nit1, dv1;
            if (Int32.TryParse(nit, out nit1))
            {
                Int32[] vpri = new Int32[16];
                x = 0; y = 0; z = nit.Length;
                vpri[1] = 3;
                vpri[2] = 7;
                vpri[3] = 13;
                vpri[4] = 17;
                vpri[5] = 19;
                vpri[6] = 23;
                vpri[7] = 29;
                vpri[8] = 37;
                vpri[9] = 41;
                vpri[10] = 43;
                vpri[11] = 47;
                vpri[12] = 53;
                vpri[13] = 59;
                vpri[14] = 67;
                vpri[15] = 71;
                for (i = 0; i < z; i++)
                {
                    y = Convert.ToInt32(nit.Substring(i, 1));
                    //document.write(y+"x"+ vpri[z-i] +":");
                    x += (y * vpri[z - i]);
                    //document.write(x+"<br>");     
                }
                y = x % 11;
                //document.write(y+"<br>");
                if (y > 1)
                {
                    dv1 = 11 - y;
                }
                else
                {
                    dv1 = y;
                }
                return dv1;
            }
            else
                dv1 = -1;
            return dv1;
        }

    }
}