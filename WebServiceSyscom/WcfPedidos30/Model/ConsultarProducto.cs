using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WcfPedidos30.Model
{
    public class ConsultarProducto
    {
        ConexionBD con = new ConexionBD();
        public RespProducto ConsultarProductos(ProductoRequest producto, UsuariosRequest datosUsuario) 
        {

            RespProducto respuesta = new RespProducto();

            con.setConnection("Prod");
            DataSet TablaProducto = new DataSet();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Usuario", datosUsuario.UserName));
            parametros.Add(new SqlParameter("@SaldosCiaBod", producto.SaldosCiaBod));
            parametros.Add(new SqlParameter("@CodOrDesProd", producto.CodOrDesProd));
            parametros.Add(new SqlParameter("@Subgrupo", producto.Subgrupo));
            parametros.Add(new SqlParameter("@Grupo", producto.Grupo));

            if (con.ejecutarQuery("WSPedidosObtenerProductos", parametros, out TablaProducto, out string[] nuevoMennsaje, CommandType.StoredProcedure))
            {
                if (TablaProducto.Tables[0].Rows.Count > 0)
                {
                    //int total = TablaProducto.Tables[0].AsEnumerable().FirstOrDefault().Field<int>("TotalFilas");
                    //se realiza la organizacion de los datos 
                    // Convierte los datos del DataSet a una lista de objetos Producto
                    List<ProductosResponse> listaProductos = new List<ProductosResponse>();

                    foreach (DataRow row in TablaProducto.Tables[0].Rows)
                    {
                        ProductosResponse productos = new ProductosResponse
                        {
                            CodProducto = row["CodProducto"].ToString(),
                            CodGru = row["CodigoGru"].ToString(),
                            CodSub = row["CodigoSub"].ToString(),
                            Descripcion = row["Descripción"].ToString(),
                            Descuento = Convert.ToInt16(row["Descuento"]),
                            FechaCreacion = row["FechaCreacion"].ToString(),
                            Impuesto = Convert.ToInt16(row["Impuesto"]),
                            Lista1 = Convert.ToInt16(row["Lista1"]),
                            Lista2 = Convert.ToInt16(row["Lista2"]),
                            Lista3 = Convert.ToInt16(row["Lista3"]),
                            NombreGru = row["NombreGru"].ToString(),
                            NombreSub = row["NombreSub"].ToString(),
                            SaldoTotal = Convert.ToInt16(row["SaldoTotal"]),

                        };
                        List<itemCia> ic = new List<itemCia>();
                        /*listaProductos.ForEach(i => ic.Add(new itemCia
                        {
                            CodBodega = row["CodBodega"].ToString(),
                            CodCia = row["CodCia"].ToString(),
                            Saldobodega = Convert.ToInt16(row["SaldoBodega"]),
                            Saldocia = Convert.ToInt16(row["Saldocia"])
                        }));
                        */
                        listaProductos.Add(productos);

                    }
                    respuesta.ListaProductos = listaProductos;
                    /*
                    mensaje = new string[2];
                    mensaje[0] = "012";
                    mensaje[1] = "Se ejecutó correctamente la consulta.";
                    pro.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };
                
    */
                    }
                else
                {
                    /*
                        mensaje = new string[2];
                        mensaje[0] = "013";
                        mensaje[1] = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)total / resPagina);
                        pro.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };
                    */
                }
            }
            else
            {
                /*
                    mensaje = new string[2];
                    mensaje[0] = "014";
                    mensaje[1] = "No se encuentran productos disponibles";
            
                */
            }
            return respuesta;
        }

    }
}