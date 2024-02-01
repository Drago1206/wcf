using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace WcfPedidos30.Model
{
    public class ConsultarProducto
    {
        ConexionBD con = new ConexionBD();

        /// <summary>
        /// Método para obtener listado de productos.
        /// </summary>
        /// <param name="producto">El producto solicitado.</param>
        /// <param name="datosUsuario">Los datos del usuario que realiza la consulta.</param>
        /// <param name="dtProducto">El paginador de los productos consultados.</param>
        /// <returns>La respuesta de la consulta de productos.</returns>
        public RespProducto ConsultarProductos(ProductoRequest producto, UsuariosRequest datosUsuario, out PaginadorProducto<ProductosResponse> dtProducto)
        {
            #region variables
            con.setConnection("Prod");
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            int registros_por_pagina = producto.RegistrosPorPagina;
            int pagina = producto.PaginaActual;
            dtProducto = new PaginadorProducto<ProductosResponse>();

            RespProducto respuesta = new RespProducto();
            DataSet TablaProducto = new DataSet();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Usuario", datosUsuario.UserName));
            parametros.Add(new SqlParameter("@SaldosCiaBod", producto.SaldosCiaBod));
            parametros.Add(new SqlParameter("@CodOrDesProd", producto.CodOrDesProd));
            parametros.Add(new SqlParameter("@Subgrupo", producto.SubGrupo));
            parametros.Add(new SqlParameter("@Grupo", producto.Grupo));
            #endregion
            if (con.ejecutarQuery("WSPedidosObtenerProductos", parametros, out TablaProducto, out string[] mensaje, CommandType.StoredProcedure))
            {
                DataTable dtProductos = TablaProducto.Tables[0];
                int TotalRegistros = TablaProducto.Tables[0].Rows.Count;
                if (TotalRegistros > 0)
                {
                    if (TotalRegistros > pagina)
                    {
                        List<ProductosResponse> listaProductos = new List<ProductosResponse>();

                        dtProducto.Resultado = con.DataTableToList<ProductosResponse>("CodProducto,Descripción,Lista1,Lista2,Lista3,Impuesto,Descuento,CodigoGru,NombreGru,CodigoSub,NombreSub,SaldoTotal,FechaCreacion".Split(','), TablaProducto);
                        dtProducto.Resultado.ForEach(m =>
                        {
                            m.ItemCia = new List<itemCia>();
                            m.ItemCia = con.DataTableToList<itemCia>(dtProductos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("CodProducto").Equals(m.CodProducto)).CopyToDataTable().AsDataView().ToTable(true, "CodCia,Saldocia,CodBodega,Saldobodega".Split(',')));
                        });
                        #region paginacion
                        if (producto.RegistrosPorPagina == 0)
                        {
                            registros_por_pagina = 10;
                        }
                        else
                        {
                            registros_por_pagina = producto.RegistrosPorPagina;
                        }

                        if (producto.PaginaActual == 0)
                        {
                            pagina = 1;
                        }
                        else
                        {
                            pagina = producto.PaginaActual;
                        }
                        listaProductos = dtProducto.Resultado.Skip((pagina - 1) * registros_por_pagina)
                                                         .Take(registros_por_pagina)
                                                         .ToList();
                        _TotalRegistros = listaProductos.Count();
                        _TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / registros_por_pagina);
                        #endregion
                        mensaje = new string[2];
                        mensaje[0] = "012";
                        mensaje[1] = "Se ejecutó correctamente la consulta.";

                        PaginadorProducto<ProductosResponse> paginadorProducto = new PaginadorProducto<ProductosResponse>
                        {
                            PaginaActual = pagina,
                            Resultado = listaProductos,
                            RegistrosPorPagina = registros_por_pagina,
                            TotalRegistros = TotalRegistros,
                            TotalPaginas = _TotalPaginas

                        };
                        dtProducto = paginadorProducto;

                    }
                    else
                    {
                        mensaje = new string[2];
                        mensaje[0] = "013";
                        mensaje[1] = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)TotalRegistros / registros_por_pagina);
                        respuesta.ListaProductos = dtProducto;

                    }
                }
            }
            else
            {
                mensaje = new string[2];
                mensaje[0] = "014";
                mensaje[1] = "No se encuentran productos disponibles";
            }

            return respuesta;
        }
    }
}