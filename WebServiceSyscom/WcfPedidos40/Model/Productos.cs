using connect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;

namespace WcfPedidos40.Model
{
    public class ProductosResponse
    {
        [DataMember]
        public string IdProducto { get; set; }
        [DataMember]
        public string DescripProd { get; set; }
        [DataMember]
        public string IvaInc { get; set; }
        [DataMember]
        public string LtPreDef { get; set; }
        [DataMember]
        public decimal VrPrecio1 { get; set; }
        [DataMember]
        public decimal VrPrecio2 { get; set; }
        [DataMember]
        public decimal VrPrecio3 { get; set; }
        [DataMember]
        public decimal VrPrecio4 { get; set; }
        [DataMember]
        public decimal VrPrecio5 { get; set; }
        [DataMember]
        public decimal TarifaIva { get; set; }
        [DataMember]
        public bool ExcluidoImp { get; set; }
        [DataMember]
        public Int32 Cantidad { get; set; }
        [DataMember]
        public bool EsObsequio { get; set; }
        [DataMember]
        public List<string> DisponibleEnCia { get; set; }

        public RespProducto GetProducto(ProductoReq pa)
        {
            RespProducto pro = new RespProducto();
            DataSet TablaProductos = new DataSet();
            try
            {
                Conexion con = new Conexion();
                con.setConnection("Syscom");
                /*Se importan los campos necesarios para grabar el pedido partiendo de las tablas OPedido y Kardex*/
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@IdProducto", pa.IdProducto));
                if (con.ejecutarQuery("WSPedidos40ConsProducto", parametros, out TablaProductos, out string[] mensajeProductos, CommandType.StoredProcedure))
                {

                    if (TablaProductos != null && TablaProductos.Tables.Count > 0)
                    {
                        pro = con.ConvertToModel<RespProducto>(TablaProductos);
                        //Verificar saldos por bodega y Cia
                        if (pro != null && TablaProductos.Tables[1].Rows.Count > 0)
                        {
                            IEnumerable<DataRow> dt = TablaProductos.Tables[1].Rows.Cast<DataRow>();
                            pro.Saldo = !string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega)).Sum(s => s.Field<decimal>("SaldoActual")) : string.IsNullOrWhiteSpace(pa.IdBodega) && !string.IsNullOrWhiteSpace(pa.IdCia) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).Sum(s => s.Field<decimal>("SaldoActual")) : string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega)).Sum(s => s.Field<decimal>("SaldoActual")) : dt.Sum(s => s.Field<decimal>("SaldoActual"));
                            pro.Bodega = pa.IdBodega;
                            pro.Compania = pa.IdCia;
                        }
                        //Verificar precios por bodega y Cia
                        //Deyci indica que para el precio no se debe tener en cuenta la bodega
                        //if (pro != null && TablaProductos.Tables[2].Rows.Count > 0 && (!string.IsNullOrWhiteSpace(pa.IdCia) || !string.IsNullOrWhiteSpace(pa.IdBodega)))
                        //{
                        if (pro != null && TablaProductos.Tables[2].Rows.Count > 0 && !string.IsNullOrWhiteSpace(pa.IdCia))
                        {
                            IEnumerable<DataRow> dt = TablaProductos.Tables[2].Rows.Cast<DataRow>();

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
            }
            catch (Exception ex)
            {
                LogErrores.tareas.Add("Ha ocurrido un error al intentar consultar: " + ex.Message);
                LogErrores.write();
            }
            return pro;
        }
        public RespProducto GetProductoTP(ProductoTPReq pa)
        {
            RespProducto pro = new RespProducto();
            try
            {
                Conexion con = new Conexion();
                con.setConnection("Syscom");
                string IdCia = ConfigurationManager.AppSettings["ciapre"] ?? null;
                DataSet TablaProductosTP = new DataSet(); //TablaProductosTP
                List<SqlParameter> parametros = new List<SqlParameter>();
                if (string.IsNullOrWhiteSpace(IdCia))
                {

                    IdCia = "01";
                }
                parametros.Add(new SqlParameter("@IdCia", IdCia));
                parametros.Add(new SqlParameter("@IdProducto", pa.IdProducto));
                /*Se importan los campos necesarios para grabar el pedido partiendo de las tablas OPedido y Kardex*/
                if (con.ejecutarQuery("WSPedidos40ProductosTP", parametros, out TablaProductosTP, out string[] mensajeProductoTP, CommandType.StoredProcedure))
                {
                    if (TablaProductosTP != null && TablaProductosTP.Tables.Count > 0)
                    {
                        pro = con.ConvertToModel<RespProducto>(TablaProductosTP);
                        //Verificar saldos por bodega y Cia
                        if (pro != null && TablaProductosTP.Tables[1].Rows.Count > 0)
                        {
                            IEnumerable<DataRow> dt = TablaProductosTP.Tables[1].Rows.Cast<DataRow>();
                            pro.Saldo = dt.Where(s => s.Field<string>("IdCia").Equals(IdCia)).Sum(s => s.Field<decimal>("SaldoActual"));
                            //!string.IsNullOrWhiteSpace(IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia) && s.Field<string>("IdBodega").Equals(pa.IdBodega)).Sum(s => s.Field<decimal>("SaldoActual")) : string.IsNullOrWhiteSpace(pa.IdBodega) && !string.IsNullOrWhiteSpace(pa.IdCia) ? dt.Where(s => s.Field<string>("IdCia").Equals(pa.IdCia)).Sum(s => s.Field<decimal>("SaldoActual")) : string.IsNullOrWhiteSpace(pa.IdCia) && !string.IsNullOrWhiteSpace(pa.IdBodega) ? dt.Where(s => s.Field<string>("IdBodega").Equals(pa.IdBodega)).Sum(s => s.Field<decimal>("SaldoActual")) : dt.Sum(s => s.Field<decimal>("SaldoActual"));
                            //pro.Bodega = pa.IdBodega;
                            pro.Compania = IdCia;
                        }
                        //Verificar precios por bodega y Cia
                        //Deyci indica que para el precio no se debe tener en cuenta la bodega
                        //if (pro != null && TablaProductosTP.Tables[2].Rows.Count > 0 && (!string.IsNullOrWhiteSpace(pa.IdCia) || !string.IsNullOrWhiteSpace(pa.IdBodega)))
                        ////{
                        //if (pro != null && TablaProductosTP.Tables[1].Rows.Count > 0 && !string.IsNullOrWhiteSpace(IdCia))
                        ////{
                        //    IEnumerable<DataRow> dt = TablaProductosTP.Tables[1].Rows.Cast<DataRow>();

                        //    pro.PrecioL1 = dt.Any(s => s.Field<string>("IdCia").Equals(IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(IdCia)).FirstOrDefault().Field<decimal>("PrecioL1") : pro.PrecioL1;
                        //    pro.PrecioL2 = dt.Any(s => s.Field<string>("IdCia").Equals(IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(IdCia)).FirstOrDefault().Field<decimal>("PrecioL2") : pro.PrecioL2;
                        //    pro.PrecioL3 = dt.Any(s => s.Field<string>("IdCia").Equals(IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(IdCia)).FirstOrDefault().Field<decimal>("PrecioL3") : pro.PrecioL3;
                        //    pro.PrecioL4 = dt.Any(s => s.Field<string>("IdCia").Equals(IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(IdCia)).FirstOrDefault().Field<decimal>("PrecioL4") : pro.PrecioL4;
                        //    pro.PrecioL5 = dt.Any(s => s.Field<string>("IdCia").Equals(IdCia)) ? dt.Where(s => s.Field<string>("IdCia").Equals(IdCia)).FirstOrDefault().Field<decimal>("PrecioL5") : pro.PrecioL5;
                        //    //Deyci dice q si está en cías y va en ceros se debe dejar en ceros
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
                        //}

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