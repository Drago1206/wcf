using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Data.SqlClient;


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
        private string DisponibleEnCia { get; set; }

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
        public string pmDisponibleEnConpania { get { return DisponibleEnCia; } set { DisponibleEnCia = value; } }


        public RespProducto GetProducto(ObtProducto pa, string compania, out string[] mensaje)
        {
            mensaje = null;
            RespProducto pro = new RespProducto();
            try
            {
                string cia = compania;
                string bodega = "";
                string producto = "";
                int pagina = 1;
                int inicio = 0;
                int fin = 10;
                int total = 0;
                int resPagina = 10;

                if (!(pa.IdCia == null || String.IsNullOrWhiteSpace(pa.IdCia)))
                {
                    cia = pa.IdCia;
                }
                if (!(pa.IdBodega == null || String.IsNullOrWhiteSpace(pa.IdBodega)))
                {
                    bodega = pa.IdBodega;
                }
                if (!(pa.IdProducto == null || String.IsNullOrWhiteSpace(pa.IdProducto)))
                {
                    producto = pa.IdProducto;
                }
                //parametros para la pagina 
                if (pa.pagina.Pagina > 0)
                {
                    pagina = pa.pagina.Pagina;
                }
                if (pa.pagina.NumResgitroPagina > 0)
                {
                    resPagina = pa.pagina.NumResgitroPagina;
                    fin = pa.pagina.NumResgitroPagina * pagina;
                    inicio = (fin - pa.pagina.NumResgitroPagina) + 1;
                }

                ConexionSQLite conSqlite = new ConexionSQLite("");
                ConexionBD ClassConexion = new ConexionBD();
                string connectionString = conSqlite.obtenerConexionSyscom().ConnectionString;
                ClassConexion.setConnection(conSqlite.obtenerConexionSyscom());
                SqlConnectionStringBuilder infoCon = conSqlite.obtenerConexionSQLServer("dbsyscom");
                DataSet tablaproducto = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Bodega", bodega));
                parametros.Add(new SqlParameter("@Cia", cia));
                parametros.Add(new SqlParameter("@Producto", producto));
                parametros.Add(new SqlParameter("@Inicio", inicio));
                parametros.Add(new SqlParameter("@Fin", fin));
                if (ClassConexion.ejecutarQuery("WSPedido_consObtProductos", parametros, out tablaproducto, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {

                    if (tablaproducto.Tables[0].Rows.Count > 0)
                    {
                        total = tablaproducto.Tables[0].AsEnumerable().FirstOrDefault().Field<int>("TotalFilas");
                        //se realiza la organizacion de los datos 
                        // Convierte los datos del DataSet a una lista de objetos Producto
                        List<Producto> listaProductos = new List<Producto>();

                        foreach (DataRow row in tablaproducto.Tables[1].Rows)
                        {
                            Producto productos = new Producto
                            {
                                IdProducto = row["pmIdProducto"].ToString(),
                                DescripProd = row["pmDescripProd"].ToString(),
                                IvaInc = row["pmIvaInc"].ToString(),
                                LtPreDef = row["pmLtPreDef"].ToString(),

                                VrPrecio1 = Convert.ToDecimal(row["pmVrPrecio1"]),
                                VrPrecio2 = Convert.ToDecimal(row["pmVrPrecio2"]),
                                VrPrecio3 = Convert.ToDecimal(row["pmVrPrecio3"]),
                                VrPrecio4 = Convert.ToDecimal(row["pmVrPrecio4"]),
                                VrPrecio5 = Convert.ToDecimal(row["pmVrPrecio5"]),
                                TarifaIva = Convert.ToDecimal(row["pmTarifaIva"]),
                                ExcluidoImp = Convert.ToBoolean(row["pmExcluidoImp"]),
                                Cantidad = Convert.ToInt32(row["pmCantidad"]),
                                DisponibleEnCia = row["pmDisponibleEnConpania"].ToString()
                            };

                            listaProductos.Add(productos);
                        }
                        pro.Productos = listaProductos;
                        mensaje = new string[2];
                        mensaje[0] = "012";
                        mensaje[1] = "Se ejecutó correctamente la consulta.";
                        pro.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };
                    }
                    else
                    {
                        mensaje = new string[2];
                        mensaje[0] = "013";
                        mensaje[1] = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)total / resPagina);
                        pro.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };
                    }
                }
                else
                {
                    mensaje = new string[2];
                    mensaje[0] = "014";
                    mensaje[1] = "No se encuentran Clientes disponibles";
                }
            }
            catch (Exception ex)
            {
                mensaje = new string[2];
                mensaje[0] = "014";
                mensaje[1] = ex.Message;
            }   
            return pro;
        }

    }



}