using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace WcfPedidos40.Model
{
    public class ProductosResponse
    {
        [DataMember]
        private string IdProducto { get; set; }
        [DataMember]
        private string DescripProd { get; set; }
        [DataMember]
        private string IvaInc { get; set; }
        [DataMember]
        private string LtPreDef { get; set; }
        [DataMember]
        private decimal VrPrecio1 { get; set; }
        [DataMember]
        private decimal VrPrecio2 { get; set; }
        [DataMember]
        private decimal VrPrecio3 { get; set; }
        [DataMember]
        private decimal VrPrecio4 { get; set; }
        [DataMember]
        private decimal VrPrecio5 { get; set; }
        [DataMember]
        private decimal TarifaIva { get; set; }
        [DataMember]
        private bool ExcluidoImp { get; set; }
        [DataMember]
        private Int32 Cantidad { get; set; }
        [DataMember]
        private bool EsObsequio { get; set; }
        [DataMember]
        private List<string> DisponibleEnCia { get; set; }

        public RespProducto GetProducto(ProductoReq pa)
        {
            RespProducto pro = new RespProducto();
            DataSet dsCons = new DataSet();
            try
            {
                Conexion con = new Conexion();
                con.setConnection("Syscom");
                /*Se importan los campos necesarios para grabar el pedido partiendo de las tablas OPedido y Kardex*/
                con.resetQuery();
                con.setCustomQuery(@"Select IdProducto AS Codigo, DescripProd AS Descripcion, DescripAbrv AS DesCorta, CAST((Precio1 - (Precio1 * isnull(TD1.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('1', IvaInc) <= 0) THEN ((Precio1 - (Precio1 * isnull(TD1.Tarifa, 0) / 100)) * (TB.Tarifa / 100))  ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL1, CAST((Precio2 - (Precio2 * isnull(TD2.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('2', IvaInc) <= 0) THEN ((Precio2 - (Precio2 * isnull(TD2.Tarifa, 0) / 100)) * (TB.Tarifa / 100)) ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL2, CAST((Precio3 - (Precio3 * isnull(TD3.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('3', IvaInc) <= 0) THEN ((Precio3 - (Precio3 * isnull(TD3.Tarifa, 0) / 100)) * (TB.Tarifa / 100)) ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL3, CAST((Precio4 - (Precio4 * isnull(TD4.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('4', IvaInc) <= 0) THEN ((Precio4 - (Precio4 * isnull(TD4.Tarifa, 0) / 100)) * (TB.Tarifa / 100)) ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL4, CAST((Precio5 - (Precio5 * isnull(TD5.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('5', IvaInc) <= 0) THEN ((Precio5 - (Precio5 * isnull(TD5.Tarifa, 0) / 100)) * (TB.Tarifa / 100)) ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL5 from ProdMcias AS P
                LEFT JOIN TablaPor AS TB ON TB.IdTarifa = P.IdTarIva
		        LEFT JOIN TablaPor AS TD1 ON TD1.IdTarifa = P.CdDct1
				LEFT JOIN TablaPor AS TD2 ON TD2.IdTarifa = P.CdDct2
				LEFT JOIN TablaPor AS TD3 ON TD3.IdTarifa = P.CdDct3
				LEFT JOIN TablaPor AS TD4 ON TD4.IdTarifa = P.CdDct4
				LEFT JOIN TablaPor AS TD5 ON TD5.IdTarifa = P.CdDct5
                Where IdProducto ='" + pa.IdProducto + @"';
                Select  IdProducto, IdBodega, IdCia, SaldoActual from ProdSaldos where IdProducto ='" + pa.IdProducto + @"';
                Select IdProducto, IdCia, IdBodega, CAST((VrPrecio1 - (VrPrecio1 * isnull(TD1.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('1', IvaInc) <= 0) THEN ((VrPrecio1  - (VrPrecio1   * isnull(TD1.Tarifa, 0) / 100)) * (TB.Tarifa / 100))  ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL1, CAST((VrPrecio2 - (VrPrecio2 * isnull(TD2.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('2', IvaInc) <= 0)  THEN ((VrPrecio2 - (VrPrecio2 * isnull(TD2.Tarifa, 0) / 100)) * (TB.Tarifa / 100))  ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL2, CAST((VrPrecio3 - (VrPrecio3 * isnull(TD3.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('3', IvaInc) <= 0)  THEN ((VrPrecio3  - (VrPrecio3 * isnull(TD3.Tarifa, 0) / 100)) * (TB.Tarifa / 100))  ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL3, CAST((VrPrecio4 - (VrPrecio4 * isnull(TD4.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('4', IvaInc) <= 0)  THEN ((VrPrecio4 - (VrPrecio4 * isnull(TD4.Tarifa, 0) / 100)) * (TB.Tarifa / 100))  ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL4, CAST((VrPrecio5 - (VrPrecio5 * isnull(TD5.Tarifa, 0) / 100)) + (CASE WHEN TB.Tarifa > 0 AND  (IvaInc = NULL OR IvaInc = '' OR CHARINDEX('5', IvaInc) <= 0)  THEN ((VrPrecio5  - (VrPrecio5 * isnull(TD5.Tarifa, 0) / 100)) * (TB.Tarifa / 100))  ELSE 0 END) AS DECIMAL(14, 4)) AS PrecioL5 from prodprecios AS PP 
				LEFT JOIN TablaPor AS TB ON TB.IdTarifa = PP.CdTarIva
	            LEFT JOIN TablaPor AS TD1 ON TD1.IdTarifa = PP.CdDcto1
				LEFT JOIN TablaPor AS TD2 ON TD2.IdTarifa = PP.CdDcto2
				LEFT JOIN TablaPor AS TD3 ON TD3.IdTarifa = PP.CdDcto3
				LEFT JOIN TablaPor AS TD4 ON TD4.IdTarifa = PP.CdDcto4
				LEFT JOIN TablaPor AS TD5 ON TD5.IdTarifa = PP.CdDcto5
				Where IdProducto ='" + pa.IdProducto + "'");
                con.ejecutarQuery();
                dsCons = con.getDataSet();
                if (dsCons != null && dsCons.Tables.Count > 0)
                {
                    pro = con.ConvertToModel<RespProducto>();
                    //Verificar saldos por bodega y Cia
                    if (pro != null && dsCons.Tables[1].Rows.Count > 0)
                    {
                        IEnumerable<DataRow> dt = dsCons.Tables[1].Rows.Cast<DataRow>();
                        pro.Saldo = !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega)).Sum(s => s.Field<decimal>("SaldoActual")) : string.IsNullOrWhiteSpace(pa.IdBodega) && !string.IsNullOrWhiteSpace(pa.IdCia) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).Sum(s => s.Field<decimal>("SaldoActual")) : string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega)).Sum(s => s.Field<decimal>("SaldoActual")) : dt.Sum(s => s.Field<decimal>("SaldoActual"));
                        pro.Bodega = pa.IdBodega;
                        pro.Compania = pa.IdCia;
                    }
                    //Verificar precios por bodega y Cia
                    //Deyci indica que para el precio no se debe tener en cuenta la bodega
                    //if (pro != null && dsCons.Tables[2].Rows.Count > 0 && (!string.IsNullOrWhiteSpace(pa.IdCia) || !string.IsNullOrWhiteSpace(pa.IdBodega)))
                    //{
                    if (pro != null && dsCons.Tables[2].Rows.Count > 0 && !string.IsNullOrWhiteSpace(pa.IdCia))
                    {
                        IEnumerable<DataRow> dt = dsCons.Tables[2].Rows.Cast<DataRow>();

                        pro.PrecioL1 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL1") : pro.PrecioL1;
                        pro.PrecioL2 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL2") : pro.PrecioL2;
                        pro.PrecioL3 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL3") : pro.PrecioL3;
                        pro.PrecioL4 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL4") : pro.PrecioL4;
                        pro.PrecioL5 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL5") : pro.PrecioL5;
                        //Deyci dice q si está en cías y va en ceros se debe dejar en ceros
                        //pro.PrecioL1 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL1") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL1") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL1") : pro.PrecioL1;
                        //pro.PrecioL2 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL2") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL2") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL2") : pro.PrecioL2;
                        //pro.PrecioL3 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL3") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL3") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL3") : pro.PrecioL3;
                        //pro.PrecioL4 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL4") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL4") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL4") : pro.PrecioL4;
                        //pro.PrecioL5 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL5") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL5") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL5") : pro.PrecioL5;
                        //Deyci indica que no se debe tener en cuenta la boidaga para los precios
                        //pro.PrecioL1 =  !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL1") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL1") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL1") : 
                        //!string.IsNullOrWhiteSpace(pa.IdCia) && string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL1") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL1") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL1") :
                        //string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL1") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL1") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL1") :
                        //pro.PrecioL1;

                        //pro.PrecioL2 = !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL2") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL2") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL2") :
                        //!string.IsNullOrWhiteSpace(pa.IdCia) && string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL2") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL2") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL2") :
                        //string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL2") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL2") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL2") :
                        //pro.PrecioL2;


                        //pro.PrecioL3 = !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL3") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL3") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL3") :
                        //!string.IsNullOrWhiteSpace(pa.IdCia) && string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL3") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL3") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL3") :
                        //string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL3") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL3") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL3") :
                        //pro.PrecioL3;

                        //pro.PrecioL4 = !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL4") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL4") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL4") :
                        //!string.IsNullOrWhiteSpace(pa.IdCia) && string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL4") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL4") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL4") :
                        //string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL4") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL4") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL4") :
                        //pro.PrecioL4;

                        //pro.PrecioL5 = !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL5") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL5") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL5") :
                        //!string.IsNullOrWhiteSpace(pa.IdCia) && string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL5") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<decimal>("PrecioL5") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL5") :
                        //string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) && dt.Any(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL5") > decimal.Zero) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega) && s.Field<decimal>("PrecioL5") > decimal.Zero).FirstOrDefault().Field<decimal>("PrecioL5") :
                        //pro.PrecioL5;
                    }

                    if (pro != null && pro.Codigo != null)
                    {
                        switch (pa.NumListaPrecio)
                        {
                            case 1:
                                pro.PrecioL2 = 0.00M;
                                pro.PrecioL3 = 0.00M;
                                pro.PrecioL4 = 0.00M;
                                pro.PrecioL5 = 0.00M;
                                break;
                            case 2:
                                pro.PrecioL1 = 0.00M;
                                pro.PrecioL3 = 0.00M;
                                pro.PrecioL4 = 0.00M;
                                pro.PrecioL5 = 0.00M;
                                break;
                            case 3:
                                pro.PrecioL1 = 0.00M;
                                pro.PrecioL2 = 0.00M;
                                pro.PrecioL4 = 0.00M;
                                pro.PrecioL5 = 0.00M;
                                break;
                            case 4:
                                pro.PrecioL1 = 0.00M;
                                pro.PrecioL2 = 0.00M;
                                pro.PrecioL3 = 0.00M;
                                pro.PrecioL5 = 0.00M;
                                break;
                            case 5:
                                pro.PrecioL1 = 0.00M;
                                pro.PrecioL2 = 0.00M;
                                pro.PrecioL3 = 0.00M;
                                pro.PrecioL4 = 0.00M;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrores.tareas.Add("Ha ocurrido un error al intentar consultar: " + ex.Message);
                LogErrores.write();
            }
            return pro;
        }
    }
}