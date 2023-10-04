using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace WcfPedidos.Model
{
    public class Producto
    {
        private string IdProducto { get; set; }
        private string DescripProd { get; set; }
        private string IvaInc { get; set; }
        private string LtPreDef { get; set; }
        private decimal VrPrecio1 { get; set; }
        private decimal VrPrecio2 { get; set; }
        private decimal VrPrecio3 { get; set; }
        private decimal VrPrecio4 { get; set; }
        private decimal VrPrecio5 { get; set; }
        private decimal TarifaIva { get; set; }
        private bool ExcluidoImp { get; set; }
        private Int32 Cantidad { get; set; }
        private bool EsObsequio { get; set; }
        private List<string> DisponibleEnCia { get; set; }

        [DataMember]
        public string pmIdProducto { get { return IdProducto; } set { IdProducto = value; } }

        [DataMember]
        public string pmDescripProd { get { return DescripProd; } set { DescripProd = value; } }

        [DataMember]
        public string pmIvaInc { get { return IvaInc; } set { IvaInc = value; } }

        [DataMember]
        public string pmLtPreDef { get { return LtPreDef; } set { LtPreDef = value; } }

        [DataMember]
        public decimal pmVrPrecio1 { get { return VrPrecio1; } set { VrPrecio1 = value; } }

        [DataMember]
        public decimal pmVrPrecio2 { get { return VrPrecio2; } set { VrPrecio2 = value; } }

        [DataMember]
        public decimal pmVrPrecio3 { get { return VrPrecio3; } set { VrPrecio3 = value; } }

        [DataMember]
        public decimal pmVrPrecio4 { get { return VrPrecio4; } set { VrPrecio4 = value; } }

        [DataMember]
        public decimal pmVrPrecio5 { get { return VrPrecio5; } set { VrPrecio5 = value; } }

        [DataMember]
        public decimal pmTarifaIva { get { return TarifaIva; } set { TarifaIva = value; } }

        [DataMember]
        public bool pmExcluidoImp { get { return ExcluidoImp; } set { ExcluidoImp = value; } }

        [DataMember]
        public Int32 pmCantidad { get { return Cantidad; } set { Cantidad = value; } }

        [DataMember]
        public bool pmEsObsequio { get { return EsObsequio; } set { EsObsequio = value; } }
        [DataMember]
        public List<string> pmDisponibleEnConpania { get { return DisponibleEnCia; } set { DisponibleEnCia = value; } }
        /*
        public string IdProducto { get; set; }

        public string Descripcion { get; set; }

        public string DesCorta { get; set; }

        public string Compania { get; set; }

        public string Bodega { get; set; }

        public decimal Saldo { get; set; }

        public decimal PrecioL1 { get; set; }

        public decimal PrecioL2 { get; set; }

        public decimal PrecioL3 { get; set; }

        public decimal PrecioL4 { get; set; }

        public decimal PrecioL5 { get; set; }

        public decimal TarifaIva { get; set; }

        public int cantidad { get; set; }
        */

        public RespProducto GetProducto(ObtProducto pa, out string[] mensaje)
        {
            mensaje = null;
            RespProducto pro = new RespProducto();
          
            DataSet dsCons = new DataSet();
            try
            {
                ConexionBD con = new ConexionBD();
                ConexionSQLite conSQLite = new ConexionSQLite("");
                string connectionString = conSQLite.obtenerConexionSyscom().ConnectionString;
                con.setConnection(conSQLite.obtenerConexionSyscom());
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
                    
                    if (pro != null && dsCons.Tables[2].Rows.Count > 0 && !string.IsNullOrWhiteSpace(pa.IdCia))
                    {
                        IEnumerable<DataRow> dt = dsCons.Tables[2].Rows.Cast<DataRow>();

                        pro.PrecioL1 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL1") : pro.PrecioL1;
                        pro.PrecioL2 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL2") : pro.PrecioL2;
                        pro.PrecioL3 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL3") : pro.PrecioL3;
                        pro.PrecioL4 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL4") : pro.PrecioL4;
                        pro.PrecioL5 = dt.Any(s => s.Field<string>("IdCia").Equals(pa.IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).FirstOrDefault().Field<decimal>("PrecioL5") : pro.PrecioL5;
                      
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
                mensaje = new string[2];
                mensaje[0] = "ee";
                mensaje[1] = ex.Message;
            }
            if (mensaje == null)
            {
                mensaje = new string[2];
                mensaje[0] = "dasd";
                mensaje[1] = "comprobacion";
            }
            return pro;
        
        }

    }



}