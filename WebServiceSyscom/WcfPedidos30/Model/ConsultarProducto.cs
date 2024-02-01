﻿using System;
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
        public RespProducto ConsultarProductos(ProductoRequest producto, UsuariosRequest datosUsuario, out PaginadorProducto<ProductosResponse> dtProducto /*PaginadorProducto<ProductosResponse> dtProducto*/ ) 
        {
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
            parametros.Add(new SqlParameter("@Subgrupo", producto.Subgrupo));
            parametros.Add(new SqlParameter("@Grupo", producto.Grupo));

            if (con.ejecutarQuery("WSPedidosObtenerProductos", parametros, out TablaProducto, out string[] mensaje, CommandType.StoredProcedure))
            {
                DataTable dtProductos = TablaProducto.Tables[0];
                int TotalRegistros = TablaProducto.Tables[0].Rows.Count;
                if (TotalRegistros > 0)
                {
                    if(TotalRegistros > pagina)
                    {
                        List<ProductosResponse> listaProductos = new List<ProductosResponse>();

                        dtProducto.Resultado = con.DataTableToList<ProductosResponse>("CodProducto,Descripción,Lista1,Lista2,Lista3,Impuesto,Descuento,CodigoGru,NombreGru,CodigoSub,NombreSub,SaldoTotal,FechaCreacion".Split(','), TablaProducto);
                        dtProducto.Resultado.ForEach(m =>
                        {
                            m.itemCia = new List<itemCia>();
                            m.itemCia = con.DataTableToList<itemCia>(dtProductos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("CodProducto").Equals(m.CodProducto)).CopyToDataTable().AsDataView().ToTable(true, "CodCia,Saldocia,CodBodega,Saldobodega".Split(',')));
                        });
                        /*
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
                                SaldoTotal = Convert.ToInt16(row["SaldoTotal"])
                            };

                            productos.itemCia = TablaProducto.Tables[0].Rows.Cast<DataRow>().Where(r => r.Field<string>("CodProducto").Equals(productos.CodProducto)).Select(r => new itemCia
                            {
                                CodCia = r["CodCia"] != DBNull.Value ? r["CodCia"].ToString() : "",
                                Saldocia = r["Saldocia"] != DBNull.Value ? Convert.ToInt16(r["Saldocia"]) : 0,
                                CodBodega = r["CodBodega"] != DBNull.Value ? r["CodBodega"].ToString() : "",
                                Saldobodega = r["SaldoBodega"] != DBNull.Value ? Convert.ToInt16(r["SaldoBodega"]) : 0
                            }).ToList();
                            listaProductos.Add(productos);
                        }
                        */
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
                        listaProductos = listaProductos.Skip((pagina - 1) * registros_por_pagina)
                                                         .Take(registros_por_pagina)
                                                         .ToList();
                        _TotalRegistros = listaProductos.Count();
                        _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / registros_por_pagina);

                        mensaje = new string[2];
                        mensaje[0] = "012";
                        mensaje[1] = "Se ejecutó correctamente la consulta.";
                        
                        PaginadorProducto<ProductosResponse> paginadorProducto = new PaginadorProducto<ProductosResponse>
                        {
                            PaginaActual = pagina,
                            Resultado = dtProducto.Resultado,
                            RegistrosPorPagina = registros_por_pagina,
                            TotalRegistros = _TotalRegistros,
                           TotalPaginas = _TotalPaginas
                      
                        };
                        dtProducto = paginadorProducto;
                        
                    }
                    else
                    {
                            mensaje = new string[2];
                            mensaje[0] = "013";
                            mensaje[1] = "La Página que deseas acceder no está disponible porque solo cuentan con " + (int)Math.Ceiling((double)TotalRegistros / registros_por_pagina);
                            //respuesta.paginas = new OrganizadorPagina { NumeroDePaginas = (int)Math.Ceiling((double)total / resPagina), RegistroTotal = total, RegistroPorPagina = resPagina, PaginaActual = pagina };
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