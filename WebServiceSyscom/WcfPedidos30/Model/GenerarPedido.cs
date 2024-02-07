using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace WcfPedidos30.Model
{
    public class GenerarPedido
    {
        public Log GenerarPedidos(DtPedido pedido, out List<PedidoResponse> datPedido)
        {
            ConexionBD con = new ConexionBD();
            datPedido = new List<PedidoResponse>();
            Log _error = new Log { Codigo = "error", Descripcion = "error" };
            List<SqlParameter> lsp = new List<SqlParameter>();
            con.setConnection("WcfPedidos30_SelectPedidos");
            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                List<ProductosPed> distinctprod = pedido.Pedido.ListaProductos.GroupBy(p => new { p.IdProducto }).Select(g => g.First()).ToList();
                string idproducto = String.Join(",", distinctprod.Select(g => "" + g.IdProducto + ""));
                /// Se agrega el parámetro necesario para ejecutar el procedimiento de almacenado, con el dato del cliente
                parametros.Add(new SqlParameter("@UserName", pedido.Usuarios.UserName));
                parametros.Add(new SqlParameter("@IdCliente", pedido.Pedido.IdCliente));
                parametros.Add(new SqlParameter("@IdAgencia", pedido.Pedido.IdAgencia));
                parametros.Add(new SqlParameter("@IdVendedor", pedido.Pedido.IdVendedor));
                parametros.Add(new SqlParameter("@IdProducto", idproducto));
                parametros.Add(new SqlParameter("@CodConcepto", pedido.Pedido.CodConcepto));

                /// Se inicializa el dataset para capturar la tabla del resultado del procedimiento de almacenado
                DataSet TablaPedidos = new DataSet();

                ///Condición que verifica si la consulta por medio del procedimiento de almacenado se efectuó correctamente
                if (con.ejecutarQuery("WSPedidosSelectPedidos", parametros, out TablaPedidos, out string[] mensaje, CommandType.StoredProcedure))
                {
                    List<PedidoResponse> list = new List<PedidoResponse>();
                    if (TablaPedidos != null)
                    {
                        List<DataRow> _productos = TablaPedidos.Tables[6].AsEnumerable().ToList();
                        DataRow usuario = TablaPedidos.Tables[1].Rows.Count > 0 ? TablaPedidos.Tables[1].Rows[0] : null;
                        DataRow cliente = TablaPedidos.Tables[2].Rows.Count > 0 ? TablaPedidos.Tables[2].Rows[0] : null;
                        DataRow agenciaCliente = TablaPedidos.Tables[3].Rows.Count > 0 ? TablaPedidos.Tables[3].Rows[0] : null;
                        DataRow vendedor = TablaPedidos.Tables[4].Rows.Count > 0 ? TablaPedidos.Tables[4].Rows[0] : null;
                        DataRow concepto = TablaPedidos.Tables[10].Rows.Count > 0 ? TablaPedidos.Tables[10].Rows[0] : null;
                        int suma = Convert.ToInt32(pedido.Pedido.ListaProductos.Sum(item => item.Cantidad));
                        int Item = 1;

                        //validaciones 
                        if (usuario == null)
                            _error = new Log { Codigo = "USER_001", Descripcion = "Usuario no encontrado" };
                        
                        else if (concepto == null)
                        {
                            //El Codigo del concepto digitado no existe o no corresponde a pedidos
                            _error = new Log { Codigo = "USER_005", Descripcion = "¡El código de concepto no existe!" };
                        }
                        else if (Convert.ToInt32(usuario["PER_AGR"]) == 0)
                        {
                            //Usuario sin permisos USER_005
                            _error = new Log { Codigo = "USER_005", Descripcion = "¡Usuario sin permisos!" };
                        }
                        else if (cliente == null || Convert.ToInt32(cliente["NIT_BLO"]) == 1)
                        {
                            //Cliente inactivo
                            _error = new Log { Codigo = "GPED_002", Descripcion = "¡El cliente esta inactivo o no existe!" };
                        }
                        else if (Convert.ToInt32(cliente["NIT_CLI"]) == 0)
                        {
                            //Tercero no marcado como cliente 
                            _error = new Log { Codigo = "GPED _010", Descripcion = "¡El tercero no está marcado como cliente!" };
                        }
                        else if (TablaPedidos.Tables[2] == null)
                        {
                            //Documento incorrecto de cliente.
                            _error = new Log { Codigo = "GPED _007", Descripcion = "¡Documento incorrecto de cliente!" };
                        }
                        else if (vendedor == null)
                        {
                            //Documento incorrecto de vendedor.
                            _error = new Log { Codigo = "GPED _008", Descripcion = "¡Documento incorrecto de vendedor!" };
                        }
                        else if (vendedor != null && Convert.ToInt32(vendedor["NIT_VEN"]) == 0)
                        {
                            //El tercero no esta marcado como vendedor 
                            _error = new Log { Codigo = "GPED _011", Descripcion = "¡El tercero no está marcado como vendedor!" };
                        }
                        else if (vendedor != null && Convert.ToInt32(vendedor["NIT_BLO"]) == 1)
                        {
                            //El tercero no esta marcado como vendedor 
                            _error = new Log { Codigo = "GPED _013", Descripcion = "¡El vendedor esta inactivo o no existe!" };
                        }
                        else if (TablaPedidos.Tables[6].Rows.Count <= 0)
                        {
                            //no existen productos 
                            _error = new Log { Codigo = "GPED_003", Descripcion = "¡No existen ningún producto para generar el pedido!" };
                        }
                        else if (suma == 0)
                        {
                            // la cantidad de productos debe ser mayor a cero 
                            _error = new Log { Codigo = "GPED_004", Descripcion = "¡Se debe solicitar 1 o más productos para generar pedido!" };
                        }
                        else if (!TablaPedidos.Tables[6].AsEnumerable().Any(p => !distinctprod.Any(lp => lp.IdProducto != p.Field<string>("REF_COD"))))
                            _error = new Log { Codigo = "GPED _012", Descripcion = "¡El código de producto '" + distinctprod.FirstOrDefault(p => !TablaPedidos.Tables[6].AsEnumerable().Any(r => r.Field<string>("REF_COD") == p.IdProducto)).IdProducto + "', NO existe!" };

                        else
                        {
                            #region Kardex
                            foreach (ProductosPed p in pedido.Pedido.ListaProductos)
                            {
                                DataRow DrKardex = TablaPedidos.Tables[5].NewRow();
                                DrKardex["KAR_TIP"] = "PED";
                                DrKardex["KAR_NUM"] = (int)0;
                                DrKardex["KAR_CIA"] = usuario["USU_CIA"];
                                DrKardex["KAR_ITM"] = Item;
                                DrKardex["KAR_DOC"] = (int)0;
                                DrKardex["KAR_CDC"] = usuario["USU_CIA"];
                                DrKardex["KAR_FEC"] = usuario["CIA_FEC"];
                                DrKardex["KAR_COD"] = p.IdProducto;
                                DrKardex["KAR_SEC"] = usuario["USU_SEC"];
                                DrKardex["KAR_ENT"] = Convert.ToDecimal(p.Cantidad);
                                DrKardex["KAR_SAL"] = 0.0000M;
                                DrKardex["KAR_VAL"] = (int)0;
                                DrKardex["KAR_PRE"] = _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("PRECIO");
                                DrKardex["KAR_IVA"] = _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("POR_VAL");
                                DrKardex["KAR_VIE"] = ((_productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("PRECIO") - (((_productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("PRECIO") * p.Cantidad) * Convert.ToDecimal(Convert.ToString(cliente["NIT_DCT"]) == "" ? _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("REF_DCT") : cliente["NIT_DCTO"]) / 100))) * (_productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("POR_VAL") / 100));
                                DrKardex["KAR_VIS"] = (int)0;
                                DrKardex["KAR_DCT"] = Convert.ToString(cliente["NIT_DCT"]) == "" ? _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("REF_DCT") : cliente["NIT_DCTO"];
                                DrKardex["KAR_VDE"] = ((_productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("PRECIO") * p.Cantidad) * Convert.ToDecimal(Convert.ToString(cliente["NIT_DCT"]) == "" ? _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("REF_DCT") : cliente["NIT_DCTO"]) / 100);//((kar_pre* kar_ent) *kar_dct)
                                DrKardex["KAR_VDS"] = (int)0;
                                DrKardex["KAR_CON"] = "PED";
                                DrKardex["KAR_CLI"] = pedido.Pedido.IdCliente;
                                DrKardex["KAR_VEN"] = pedido.Pedido.IdVendedor;
                                DrKardex["KAR_CMS"] = 0.0000M;
                                DrKardex["KAR_CMC"] = 0.0000M;
                                DrKardex["KAR_TAN"] = "";
                                DrKardex["KAR_LOT"] = "";
                                DrKardex["KAR_REM"] = "0";
                                DrKardex["KAR_PED"] = (int)0;
                                DrKardex["KAR_COM"] = false;
                                DrKardex["KAR_SUR"] = "";
                                DrKardex["KAR_INI"] = 0.0000M;
                                DrKardex["KAR_FIN"] = 0.0000M;
                                DrKardex["KAR_TAS"] = 0.0000M;
                                DrKardex["KAR_NAC"] = 0.0000M;
                                DrKardex["KAR_DEP"] = 0.0000M;
                                DrKardex["KAR_MUN"] = 0.0000M;
                                DrKardex["KAR_SOL"] = 0.0000M;
                                DrKardex["KAR_UNI"] = 0.0000M;
                                DrKardex["KAR_IMP"] = false;
                                DrKardex["KAR_DCO"] = Convert.ToString(cliente["NIT_DCT"]) == "" ? _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<string>("GRU_DCT") : cliente["NIT_DCT"];
                                DrKardex["KAR_REF"] = "WS";
                                DrKardex["KAR_CIU"] = string.IsNullOrWhiteSpace(pedido.Pedido.IdAgencia) ? cliente["NIT_CIU"] : agenciaCliente["AGE_CIU"];
                                DrKardex["KAR_CEN"] = (int)0;
                                DrKardex["KAR_CSA"] = (int)0;
                                DrKardex["KAR_PRT"] = 0.0000M;
                                DrKardex["KAR_RET"] = (int)0;
                                DrKardex["KAR_RSA"] = (int)0;
                                DrKardex["KAR_BAS"] = _productos.Where(pi => pi.Field<string>("REF_COD").Equals(p.IdProducto)).SingleOrDefault().Field<decimal>("PRECIO");
                                DrKardex["KAR_USU"] = pedido.Usuarios.UserName;
                                DrKardex["KAR_OPE"] = "";
                                DrKardex["KAR_CRF"] = "";
                                DrKardex["KAR_CIC"] = "";
                                DrKardex["KAR_ICA"] = (int)0;
                                DrKardex["KAR_PIC"] = 0.0000M;
                                DrKardex["KAR_GUI"] = (int)0;
                                DrKardex["KAR_CGU"] = DBNull.Value;
                                DrKardex["KAR_ORD"] = (int)0;
                                DrKardex["KAR_COC"] = DBNull.Value;
                                DrKardex["KAR_NCO"] = DBNull.Value;
                                DrKardex["KAR_FOC"] = DBNull.Value;
                                DrKardex["KAR_AGE"] = "N";
                                DrKardex["KAR_ICO"] = 0.0000M;
                                DrKardex["KAR_TIC"] = DBNull.Value;
                                DrKardex["KAR_CUS"] = (int)0;
                                DrKardex["KAR_TCV"] = DBNull.Value;
                                DrKardex["KAR_TCC"] = DBNull.Value;
                                DrKardex["KAR_DIV"] = (int)0;
                                DrKardex["KAR_CC"] = "";
                                DrKardex["KAR_TIM"] = 0.0000M;
                                DrKardex["KAR_IMV"] = 0.0000M;
                                DrKardex["KAR_COB"] = 0.0000M;
                                DrKardex["KAR_IOB"] = 0.0000M;
                                DrKardex["KAR_RCO"] = false;
                                /*
                                DrKardex["KAR_CTB"] = "";
                                DrKardex["KAR_TIB"] = 0.0000M;
                                DrKardex["KAR_IBA"] = (int)0;
                                DrKardex["KAR_CTU"] = "";
                                DrKardex["KAR_TIU"] = 0.0000M;
                                DrKardex["KAR_ICU"] = (int)0;
                                */
                                TablaPedidos.Tables[5].Rows.Add(DrKardex);
                                Item++;
                            }
                            lsp.Add(new SqlParameter("@dataTypeKARDEX", TablaPedidos.Tables[5]));
                            #endregion Kardex

                            #region Pedido
                            DataRow DrPedido = TablaPedidos.Tables[0].NewRow();
                            DrPedido["PED_NUM"] = (int)0;
                            DrPedido["PED_TIP"] = "PED";
                            DrPedido["PED_CIA"] = TablaPedidos.Tables[1].Rows[0]["USU_CIA"];
                            DrPedido["PED_FEC"] = TablaPedidos.Tables[1].Rows[0]["CIA_FEC"];
                            DrPedido["PED_CON"] = "PED";
                            DrPedido["PED_CLI"] = pedido.Pedido.IdCliente.ToString();
                            DrPedido["PED_AGE"] = string.IsNullOrWhiteSpace(pedido.Pedido.IdAgencia) ? "N" : pedido.Pedido.IdAgencia;
                            DrPedido["PED_FDE"] = DateTime.Now.Date;
                            DrPedido["PED_VCE"] = DateTime.Now.Date;
                            DrPedido["PED_HOR"] = DateTime.Now.ToString("hh:mm:ss tt");
                            DrPedido["PED_VAL"] = TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_PRE") * vn.Field<decimal>("KAR_ENT"));//sumatoria todos los item (kar_pre*kar_ent)
                            DrPedido["PED_IVA"] = Decimal.Round(TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_VIE")));//sum todos item valor iva 
                            DrPedido["PED_DCT"] = TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_VDE"));//sum todos item kardex. Kar_vde 
                            DrPedido["PED_FLE"] = pedido.Pedido.VrFlete;
                            DrPedido["PED_OTR"] = (int)0;
                            DrPedido["PED_PAG"] = (int)0;
                            DrPedido["PED_IMP"] = (int)0;
                            DrPedido["PED_NET"] = Decimal.Round((TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_PRE") * vn.Field<decimal>("KAR_ENT")) - TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_VDE"))) + TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_VIE")) + pedido.Pedido.VrFlete);//ped_val-ped_dct+ped_iva+ped_fle
                            DrPedido["PED_VEN"] = pedido.Pedido.IdVendedor.ToString();
                            DrPedido["PED_CDT"] = "";
                            DrPedido["PED_SOL"] = "";
                            DrPedido["PED_DIR"] = cliente["NIT_DIR"];
                            DrPedido["PED_CIU"] = cliente["NIT_CIU"];
                            DrPedido["PED_LUG"] = "";
                            DrPedido["PED_FOR"] = "";
                            DrPedido["PED_PES"] = "";
                            DrPedido["PED_VOL"] = "";
                            DrPedido["PED_TRA"] = "";
                            DrPedido["PED_EMP"] = "";
                            DrPedido["PED_PLA"] = "";
                            DrPedido["PED_APL"] = false;
                            DrPedido["PED_DOC"] = (int)0;
                            DrPedido["PED_CDC"] = DBNull.Value;
                            DrPedido["PED_REM"] = 0;
                            DrPedido["PED_CRM"] = TablaPedidos.Tables[1].Rows[0]["USU_CIA"];
                            DrPedido["PED_APR"] = -1;
                            DrPedido["PED_CPR"] = TablaPedidos.Tables[1].Rows[0]["USU_CIA"];
                            DrPedido["PED_MAY"] = false;
                            DrPedido["PED_SOB"] = false;
                            DrPedido["PED_CAR"] = (int)0;
                            DrPedido["PED_ORI"] = "";
                            DrPedido["PED_IMV"] = (int)0;
                            TablaPedidos.Tables[0].Rows.Add(DrPedido);
                            lsp.Add(new SqlParameter("@dataTypePEDIDOS", TablaPedidos.Tables[0]));
                            #endregion pedido

                            #region INFDOC
                            DataRow DrINFDOC = TablaPedidos.Tables[7].NewRow();
                            DrINFDOC["DOC_TIP"] = "PED";
                            DrINFDOC["DOC_NUM"] = "0";
                            DrINFDOC["DOC_CIA"] = TablaPedidos.Tables[1].Rows[0]["USU_CIA"];
                            DrINFDOC["DOC_CMP"] = (int)0;
                            DrINFDOC["DOC_NCP"] = (int)0;
                            DrINFDOC["DOC_DEV"] = (int)0;
                            DrINFDOC["DOC_NDV"] = (int)0;
                            DrINFDOC["DOC_FDV"] = DBNull.Value;
                            DrINFDOC["DOC_MUL"] = (int)0;
                            DrINFDOC["DOC_PAG"] = cliente["NIT_CDP"];
                            DrINFDOC["DOC_OBS"] = pedido.Pedido.Observación;
                            DrINFDOC["DOC_BAS"] = (int)0;
                            DrINFDOC["DOC_BA1"] = TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_PRE")) - TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_VDE"));
                            DrINFDOC["DOC_BA2"] = (int)0;
                            DrINFDOC["DOC_POR"] = 0.0000M;
                            DrINFDOC["DOC_PO1"] = 0.0000M;
                            DrINFDOC["DOC_PO2"] = 0.0000M;
                            DrINFDOC["DOC_COP"] = "";
                            DrINFDOC["DOC_CO1"] = "";
                            DrINFDOC["DOC_CO2"] = "";
                            DrINFDOC["DOC_CUM"] = (int)0;
                            DrINFDOC["DOC_FCU"] = DBNull.Value;
                            DrINFDOC["DOC_EST"] = "";
                            DrINFDOC["DOC_FEC"] = TablaPedidos.Tables[1].Rows[0]["CIA_FEC"];
                            DrINFDOC["DOC_CAN"] = TablaPedidos.Tables[5].AsEnumerable().Sum(vn => vn.Field<decimal>("KAR_ENT"));
                            DrINFDOC["DOC_USU"] = pedido.Usuarios.UserName;
                            DrINFDOC["DOC_KLR"] = 0.0000M;
                            DrINFDOC["DOC_TCU"] = "";
                            DrINFDOC["DOC_SUS"] = "";
                            DrINFDOC["DOC_CSU"] = "";
                            DrINFDOC["DOC_DCU"] = DBNull.Value;
                            DrINFDOC["DOC_CBO"] = "";
                            DrINFDOC["DOC_CVD"] = (int)0;
                            TablaPedidos.Tables[7].Rows.Add(DrINFDOC);
                            lsp.Add(new SqlParameter("@dataTypeINFDOC", TablaPedidos.Tables[7]));
                            #endregion INFDOC

                            #region LOGSYS
                            DataRow DrLOGSYS = TablaPedidos.Tables[8].NewRow();
                            DrLOGSYS["LOG_USU"] = pedido.Usuarios.UserName;
                            DrLOGSYS["LOG_CIA"] = TablaPedidos.Tables[1].Rows[0]["USU_CIA"];
                            DrLOGSYS["LOG_TIP"] = "PED";
                            DrLOGSYS["LOG_ID"] = (int)0;
                            DrLOGSYS["LOG_NUM"] = "";
                            DrLOGSYS["LOG_FEC"] = DateTime.Now;
                            DrLOGSYS["LOG_FRE"] = TablaPedidos.Tables[1].Rows[0]["CIA_FEC"];
                            DrLOGSYS["LOG_CMP"] = false;
                            DrLOGSYS["LOG_ELI"] = false;
                            DrLOGSYS["LOG_PRE"] = false;
                            DrLOGSYS["LOG_DOC"] = "Pedido creado por WS";
                            TablaPedidos.Tables[8].Rows.Add(DrLOGSYS);
                            lsp.Add(new SqlParameter("@dataTypeLOGSYS", TablaPedidos.Tables[8]));
                            #endregion INFDOC

                            #region Documentos
                            DataRow DrDocumentos = TablaPedidos.Tables[9].NewRow();
                            DrDocumentos["DOC_TIP"] = "PED";
                            DrDocumentos["DOC_NUM"] = (int)0; ;
                            DrDocumentos["DOC_CIA"] = TablaPedidos.Tables[1].Rows[0]["USU_CIA"];
                            DrDocumentos["DOC_FEC"] = TablaPedidos.Tables[1].Rows[0]["CIA_FEC"];
                            DrDocumentos["DOC_FAC"] = "0";
                            DrDocumentos["DOC_CLI"] = pedido.Pedido.IdCliente;
                            DrDocumentos["DOC_ELI"] = false;
                            DrDocumentos["DOC_ANU"] = false;
                            DrDocumentos["DOC_DEV"] = false;
                            DrDocumentos["DOC_NDV"] = -1;
                            DrDocumentos["DOC_FDV"] = DBNull.Value;
                            DrDocumentos["DOC_CMP"] = "";
                            DrDocumentos["DOC_NCP"] = -1;
                            TablaPedidos.Tables[9].Rows.Add(DrDocumentos);
                            lsp.Add(new SqlParameter("@dataTypeDOCUMENTOS", TablaPedidos.Tables[9]));
                            #endregion Documentos

                            //con.addParametersProc(lsp);
                            //con.ejecutarProcedimiento("paSwInsPedido");
                            DataSet InsPedido= new DataSet();
                            con.setConnection("DBMOV");
                            if(con.ejecutarQuery("paSwInsPedido", lsp, out InsPedido, out string[] mensajeError , CommandType.StoredProcedure))
                            {
                                //DataTable dtDatos = con.getDataTable();
                                datPedido = con.DataTableToList<PedidoResponse>("TipoDoc,IdCia,Fecha,TotalProductos,SubTotal,Descuento,Iva".Split(','),InsPedido);

                            }
                            /*
                            if (con.getDataTable() != null && con.getDataTable().Rows.Count > 0)
                            {
                                DataTable dtDatos = con.getDataTable();
                                datPedido = con.DataTableToList<PedidoResponse>();
                            }*/
                        }

                    }
                }

                #region
                //#region KARDEX (dataTypeKARDEX)
                //Dictionary<string, List<Dictionary<string, object>>> LstKARDEXListas = new Dictionary<string, List<Dictionary<string, object>>>();
                //List<Dictionary<string, object>> ListaKARDEX = new List<Dictionary<string, object>>();
                //Dictionary<string, object> FilaKARDEX = new Dictionary<string, object>();
                //Decimal PED_VAL = Convert.ToDecimal(0);

                //var i = 1;
                //foreach (var val in pedido.ListaProductos)
                //{
                //    FilaKARDEX = new Dictionary<string, object>();
                //    FilaKARDEX.Add("KAR_TIP", "PED");
                //    FilaKARDEX.Add("KAR_NUM", "");
                //    FilaKARDEX.Add("KAR_CIA", "");
                //    FilaKARDEX.Add("KAR_ITM", "");
                //    FilaKARDEX.Add("KAR_DOC", "0");
                //    FilaKARDEX.Add("KAR_CDC", "");
                //    FilaKARDEX.Add("KAR_FEC", "");
                //    FilaKARDEX.Add("KAR_COD", "");
                //    FilaKARDEX.Add("KAR_SEC", "");
                //    FilaKARDEX.Add("KAR_ENT", "");
                //    FilaKARDEX.Add("KAR_SAL", "0.0000");
                //    FilaKARDEX.Add("KAR_VAL", "0");
                //    FilaKARDEX.Add("KAR_PRE", "");
                //    FilaKARDEX.Add("KAR_IVA", "");
                //    FilaKARDEX.Add("KAR_VIE", "");
                //    FilaKARDEX.Add("KAR_VIS", "0");
                //    FilaKARDEX.Add("KAR_DCT", "");
                //    FilaKARDEX.Add("KAR_VDE", "");
                //    FilaKARDEX.Add("KAR_VTablaPedidos", "0");
                //    FilaKARDEX.Add("KAR_CON", "PED");
                //    FilaKARDEX.Add("KAR_CLI", "");
                //    FilaKARDEX.Add("KAR_VEN", "");
                //    FilaKARDEX.Add("KAR_CMS", "0.0000");
                //    FilaKARDEX.Add("KAR_CMC", "0.0000");
                //    FilaKARDEX.Add("KAR_TAN", "");
                //    FilaKARDEX.Add("KAR_LOT", "");
                //    FilaKARDEX.Add("KAR_REM", "0");
                //    FilaKARDEX.Add("KAR_PED", "0");
                //    FilaKARDEX.Add("KAR_COM", "0");
                //    FilaKARDEX.Add("KAR_SUR", "");
                //    FilaKARDEX.Add("KAR_INI", "0.0000");
                //    FilaKARDEX.Add("KAR_FIN", "0.0000");
                //    FilaKARDEX.Add("KAR_TAS", "0.0000");
                //    FilaKARDEX.Add("KAR_NAC", "0.0000");
                //    FilaKARDEX.Add("KAR_DEP", "0.0000");
                //    FilaKARDEX.Add("KAR_MUN", "0.0000");
                //    FilaKARDEX.Add("KAR_SOL", "0.0000");
                //    FilaKARDEX.Add("KAR_UNI", "0.0000");
                //    FilaKARDEX.Add("KAR_IMP", "0");
                //    FilaKARDEX.Add("KAR_DCO", "");
                //    FilaKARDEX.Add("KAR_REF", "WS");
                //    FilaKARDEX.Add("KAR_CIU", "");
                //    FilaKARDEX.Add("KAR_CEN", "0");
                //    FilaKARDEX.Add("KAR_CSA", "0");
                //    FilaKARDEX.Add("KAR_PRT", "0.0000");
                //    FilaKARDEX.Add("KAR_RET", "0");
                //    FilaKARDEX.Add("KAR_RSA", "0");
                //    FilaKARDEX.Add("KAR_BAS", "");
                //    FilaKARDEX.Add("KAR_USU", "");
                //    FilaKARDEX.Add("KAR_OPE", "");
                //    FilaKARDEX.Add("KAR_CRF", "");
                //    FilaKARDEX.Add("KAR_CIC", "");
                //    FilaKARDEX.Add("KAR_ICA", "");
                //    FilaKARDEX.Add("KAR_PIC", "0.0000");
                //    FilaKARDEX.Add("KAR_GUI", "");
                //    FilaKARDEX.Add("KAR_CGU", "NULL");
                //    FilaKARDEX.Add("KAR_ORD", "");
                //    FilaKARDEX.Add("KAR_COC", "NULL");
                //    FilaKARDEX.Add("KAR_NCO", "NULL");
                //    FilaKARDEX.Add("KAR_FOC", "NULL");
                //    FilaKARDEX.Add("KAR_AGE", "N");
                //    FilaKARDEX.Add("KAR_ICO", "0.0000");
                //    FilaKARDEX.Add("KAR_TIC", "NULL");
                //    FilaKARDEX.Add("KAR_CUS", "0");
                //    FilaKARDEX.Add("KAR_TCV", "NULL");
                //    FilaKARDEX.Add("KAR_TCC", "NULL");
                //    FilaKARDEX.Add("KAR_DIV", "0");
                //    FilaKARDEX.Add("KAR_CC", "");
                //    FilaKARDEX.Add("KAR_TIM", "0.0000");
                //    FilaKARDEX.Add("KAR_IMV", "0.0000");
                //    FilaKARDEX.Add("KAR_COB", "0.0000");
                //    FilaKARDEX.Add("KAR_IOB", "0.0000");

                //    ListaKARDEX.Add(FilaKARDEX);
                //    i++;
                //}



                //ListaKARDEX.Add(FilaKARDEX);
                //LstKARDEXListas.Add("dataTypeKARDEX", ListaKARDEX);
                //Listas = new List<Dictionary<string, List<Dictionary<string, object>>>>();
                //Listas.Add(LstKARDEXListas);
                //lsp.Add(new SqlParameter("@dataTypeKARDEX", tr.transformar(Listas)));
                //#endregion

                //#region Pedidos (dataTypePEDIDOS)
                //Dictionary<string, List<Dictionary<string, object>>> LstPedidosListas = new Dictionary<string, List<Dictionary<string, object>>>();
                //List<Dictionary<string, object>> ListaPedidos = new List<Dictionary<string, object>>();
                //Dictionary<string, object> FilaPedidos = new Dictionary<string, object>();

                //FilaPedidos.Add("PED_NUM", "");
                //FilaPedidos.Add("PED_TIP", "PED");
                //FilaPedidos.Add("PED_CIA", "");
                //FilaPedidos.Add("PED_FEC", "");
                //FilaPedidos.Add("PED_CON", "PED");
                //FilaPedidos.Add("PED_CLI", pedido.IdCliente);
                //FilaPedidos.Add("PED_AGE", "");
                //FilaPedidos.Add("PED_FDE", DateTime.Now);
                //FilaPedidos.Add("PED_VCE", DateTime.Now);
                //FilaPedidos.Add("PED_HOR", DateTime.Now);
                //FilaPedidos.Add("PED_VAL", "");
                //FilaPedidos.Add("PED_IVA", "");
                //FilaPedidos.Add("PED_DCT", "");
                //FilaPedidos.Add("PED_FLE", "0");
                //FilaPedidos.Add("PED_OTR", "0");
                //FilaPedidos.Add("PED_PAG", "0");
                //FilaPedidos.Add("PED_IMP", "0");
                //FilaPedidos.Add("PED_NET", "");
                //FilaPedidos.Add("PED_VEN", "");
                //FilaPedidos.Add("PED_CDT", "");
                //FilaPedidos.Add("PED_SOL", "");
                //FilaPedidos.Add("PED_DIR", "");
                //FilaPedidos.Add("PED_CIU", "");
                //FilaPedidos.Add("PED_LUG", "");
                //FilaPedidos.Add("PED_FOR", "");
                //FilaPedidos.Add("PED_PES", "");
                //FilaPedidos.Add("PED_VOL", "");
                //FilaPedidos.Add("PED_TRA", "");
                //FilaPedidos.Add("PED_EMP", "");
                //FilaPedidos.Add("PED_PLA", "");
                //FilaPedidos.Add("PED_APL", "0");
                //FilaPedidos.Add("PED_DOC", "0");
                //FilaPedidos.Add("PED_CDC", "NULL");
                //FilaPedidos.Add("PED_REM", "0");
                //FilaPedidos.Add("PED_CRM", "2");
                //FilaPedidos.Add("PED_APR", "-1");
                //FilaPedidos.Add("PED_CPR", "1");
                //FilaPedidos.Add("PED_MAY", "0");
                //FilaPedidos.Add("PED_SOB", "0");
                //FilaPedidos.Add("PED_CAR", "0");
                //FilaPedidos.Add("PED_ORI", "");
                //FilaPedidos.Add("PED_IMV", "0");

                //ListaPedidos.Add(FilaPedidos);
                //LstPedidosListas.Add("dataTypePEDIDOS", ListaPedidos);
                //Listas = new List<Dictionary<string, List<Dictionary<string, object>>>>();
                //Listas.Add(LstPedidosListas);
                //lsp.Add(new SqlParameter("@dataTypePEDIDOS", tr.transformar(Listas)));
                //#endregion

                //#region INFDOC (dataTypeINFDOC)
                //Dictionary<string, List<Dictionary<string, object>>> LstINFDOCListas = new Dictionary<string, List<Dictionary<string, object>>>();
                //List<Dictionary<string, object>> ListaINFDOC = new List<Dictionary<string, object>>();
                //Dictionary<string, object> FilaINFDOC = new Dictionary<string, object>();

                //FilaINFDOC.Add("DOC_TIP", "PED");
                //FilaINFDOC.Add("DOC_NUM", "");
                //FilaINFDOC.Add("DOC_CIA", "");
                //FilaINFDOC.Add("DOC_CMP", "");
                //FilaINFDOC.Add("DOC_NCP", "0");
                //FilaINFDOC.Add("DOC_DEV", "0");
                //FilaINFDOC.Add("DOC_NDV", "0");
                //FilaINFDOC.Add("DOC_FDV", "NULL");
                //FilaINFDOC.Add("DOC_MUL", "0");
                //FilaINFDOC.Add("DOC_PAG", "");
                //FilaINFDOC.Add("DOC_OBS", "");
                //FilaINFDOC.Add("DOC_BAS", "0");
                //FilaINFDOC.Add("DOC_BA1", "");
                //FilaINFDOC.Add("DOC_BA2", "0");
                //FilaINFDOC.Add("DOC_POR", "0.0000");
                //FilaINFDOC.Add("DOC_PO1", "0.0000");
                //FilaINFDOC.Add("DOC_PO2", "0.0000");
                //FilaINFDOC.Add("DOC_COP", "");
                //FilaINFDOC.Add("DOC_CO1", "");
                //FilaINFDOC.Add("DOC_CO2", "");
                //FilaINFDOC.Add("DOC_CUM", "0");
                //FilaINFDOC.Add("DOC_FCU", "NULL");
                //FilaINFDOC.Add("DOC_EST", "");
                //FilaINFDOC.Add("DOC_FEC", "");
                //FilaINFDOC.Add("DOC_CAN", "");
                //FilaINFDOC.Add("DOC_USU", "");
                //FilaINFDOC.Add("DOC_KLR", "0.0000");
                //FilaINFDOC.Add("DOC_TCU", "");
                //FilaINFDOC.Add("DOC_SUS", "");
                //FilaINFDOC.Add("DOC_CSU", "");
                //FilaINFDOC.Add("DOC_DCU", "NULL");
                //FilaINFDOC.Add("DOC_CBO", "");
                //FilaINFDOC.Add("DOC_CVD", "0");

                //ListaINFDOC.Add(FilaINFDOC);
                //LstINFDOCListas.Add("dataTypeINFDOC", ListaINFDOC);
                //Listas = new List<Dictionary<string, List<Dictionary<string, object>>>>();
                //Listas.Add(LstINFDOCListas);
                //lsp.Add(new SqlParameter("@dataTypeINFDOC", tr.transformar(Listas)));
                //#endregion

                //#region LogSys (dataTypeLOGSYS)
                //Dictionary<string, List<Dictionary<string, object>>> LstLogSysListas = new Dictionary<string, List<Dictionary<string, object>>>();
                //List<Dictionary<string, object>> ListaLogSys = new List<Dictionary<string, object>>();
                //Dictionary<string, object> FilaLogSys = new Dictionary<string, object>();

                //FilaLogSys.Add("LOG_USU", pusuario.UserName);
                //FilaLogSys.Add("LOG_CIA", "");
                //FilaLogSys.Add("LOG_TIP", "");
                //FilaLogSys.Add("LOG_ID", "");
                //FilaLogSys.Add("LOG_NUM", "");
                //FilaLogSys.Add("LOG_FEC", "");
                //FilaLogSys.Add("LOG_FRE", "");
                //FilaLogSys.Add("LOG_CMP", "");
                //FilaLogSys.Add("LOG_ELI", "");
                //FilaLogSys.Add("LOG_PRE", "");
                //FilaLogSys.Add("LOG_DOC", "");

                //ListaLogSys.Add(FilaLogSys);
                //LstLogSysListas.Add("dataTypeLOGSYS", ListaLogSys);
                //Listas = new List<Dictionary<string, List<Dictionary<string, object>>>>();
                //Listas.Add(LstLogSysListas);
                //lsp.Add(new SqlParameter("@dataTypeLOGSYS", tr.transformar(Listas)));
                //#endregion

                //#region Documentos (dataTypeDOCUMENTOS)
                //Dictionary<string, List<Dictionary<string, object>>> LstDocumentosListas = new Dictionary<string, List<Dictionary<string, object>>>();
                //List<Dictionary<string, object>> ListaDocumentos = new List<Dictionary<string, object>>();
                //Dictionary<string, object> FilaDocumentos = new Dictionary<string, object>();

                //FilaDocumentos.Add("DOC_TIP", "PED");
                //FilaDocumentos.Add("DOC_NUM", "");
                //FilaDocumentos.Add("DOC_CIA", "");
                //FilaDocumentos.Add("DOC_FEC", "");
                //FilaDocumentos.Add("DOC_FAC", "0");
                //FilaDocumentos.Add("DOC_CLI", "");
                //FilaDocumentos.Add("DOC_ELI", "0");
                //FilaDocumentos.Add("DOC_ANU", "0");
                //FilaDocumentos.Add("DOC_DEV", "0");
                //FilaDocumentos.Add("DOC_NDV", "-1");
                //FilaDocumentos.Add("DOC_FDV", "NULL");
                //FilaDocumentos.Add("DOC_CMP", "");
                //FilaDocumentos.Add("DOC_NCP", "-1");

                //ListaDocumentos.Add(FilaDocumentos);
                //LstDocumentosListas.Add("dataTypeDOCUMENTOS", ListaDocumentos);
                //Listas = new List<Dictionary<string, List<Dictionary<string, object>>>>();
                //Listas.Add(LstDocumentosListas);
                //lsp.Add(new SqlParameter("@dataTypeDOCUMENTOS", tr.transformar(Listas)));
                //#endregion

                #endregion


            }
            catch (Exception ex)
            {
                _error = new Log
                {
                    Descripcion = ex.Message
                };
            }
            return _error;
        }

    }
}