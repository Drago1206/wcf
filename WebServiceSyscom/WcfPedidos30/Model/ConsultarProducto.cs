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
        /// Método para obtener listado de productos
        /// por medio de un procedimiento de almacenado --WSPedidosObtenerProductos--
        /// </summary>
        /// <param name="producto">Propiedades de solicitud del producto</param>
        /// <param name="datosUsuario">Los datos del usuario que realiza la consulta.</param>
        /// <param name="dtProducto">Variable de salida con los datos de respuesta del producto</param>
        /// <returns>La respuesta de la consulta de productos.</returns>
        public RespProducto ConsultarProductos(ProductoRequest producto, UsuariosRequest datosUsuario, out PaginadorProducto<ProductosResponse> dtProducto, out string[] mensaje)
        {
            /// Configuración de la cadena de conexión para determinar a qué base de datos va dirigida la consulta
            con.setConnection("Prod");
            mensaje = null;
            /// Se inicializa un nuevo obteto PaginadorProducto en la variable dtProducto
            dtProducto = new PaginadorProducto<ProductosResponse>();

            /// Se inicializan las variables para la paginación de los productos
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;
            int registros_por_pagina = producto.RegistrosPorPagina;
            int pagina = producto.PaginaActual;
            /// Se inicializa el objeto RespProducto
            RespProducto respuesta = new RespProducto();
            /// Se inicializa la tabla de la respuesta del procedimiento de almacenado
            DataSet TablaProducto = new DataSet();
            /// Se inicializa una lista de parámetros para enviárselos al procedimiento de almacenado
            List<SqlParameter> parametros = new List<SqlParameter>();
            /// Se agregan los parámetros a la lista para enviárselos al procedimiento de almacenado
            parametros.Add(new SqlParameter("@Usuario", datosUsuario.UserName));
            parametros.Add(new SqlParameter("@SaldosCiaBod", producto.SaldosCiaBod));
            parametros.Add(new SqlParameter("@CodOrDesProd", producto.CodOrDesProd));
            parametros.Add(new SqlParameter("@Subgrupo", producto.SubGrupo));
            parametros.Add(new SqlParameter("@Grupo", producto.Grupo));
            
            /// Condición para verificar si el procedimiento de almacenado se ejecuta correctamente
            if (con.ejecutarQuery("WSPedidosObtenerProductos", parametros, out TablaProducto, out string[] mensajeConsulta, CommandType.StoredProcedure))
            {
                /// Se inicializa la tabla contenedora de los resultados del procedimiento de almacenado 
                DataTable dtProductos = TablaProducto.Tables[0];
                
                /// Se inicializa la variable que contendrá la cantidad de registros recibidos del procedimiento de almacenado
                int TotalRegistros = TablaProducto.Tables[0].Rows.Count;
                /// Condición para verificar si la cantidad de registros recibidos son mayor a cero
                if (TotalRegistros > 0)
                {
                    /// Condición para verificar si la pagina recibida no es mayor a la cantidad de registros 
                    if (TotalRegistros > producto.PaginaActual)
                    {
                        /// Se inicializa la lista que contendrá los datos de los productos de la respuesta
                        List<ProductosResponse> listaProductos = new List<ProductosResponse>();

                        /// Se le asigna el valor a la variable dtCliente con la lista que retorna el método DataTableToList
                        dtProducto.Resultado = con.DataTableToList<ProductosResponse>("CodProducto,Descripción,Lista1,Lista2,Lista3,Impuesto,Descuento,CodigoGru,NombreGru,CodigoSub,NombreSub,SaldoTotal,FechaCreacion".Split(','), TablaProducto);

                        /// Se recorre la lista dtCliente
                        dtProducto.Resultado.ForEach(m =>
                        {
                            /// Se inicializa una lista dentro de la lista dtCliente
                            m.ItemCia = new List<itemCia>();
                            /// Se asigna valor a la ItemCia con la lista que retorna el método DataTableToList
                            m.ItemCia = con.DataTableToList<itemCia>(dtProductos.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("CodProducto").Equals(m.CodProducto)).CopyToDataTable().AsDataView().ToTable(true, "CodCia,Saldocia,CodBodega,Saldobodega".Split(',')));
                        });
                        #region paginacion
                        /// Condición que verifica si los registros por página son igual a cero
                        if (producto.RegistrosPorPagina == 0)
                        {
                            /// Si la condición se cumple, se asigna por defecto el valor de diez
                            registros_por_pagina = 10;
                        }
                        else
                        {
                            /// Si la condición no se cumple, se deja el valor que se recibe de la solicitud
                            registros_por_pagina = producto.RegistrosPorPagina;
                        }
                        /// Condición que verifica si la página actual es igual a cero
                        if (producto.PaginaActual == 0)
                        {
                            /// Si la condición se cumple, se asigna por defecto el valor de uno
                            pagina = 1;
                        }
                        else
                        {
                            /// Si la condición no se cumple, se deja como valor el que se recibe de la solicitud
                            pagina = producto.PaginaActual;
                        }

                        /// Se asigna el valor a la variable según el cálculo
                        /// para determinar qué productos se mostrarán según el rango de páginas solicitados
                        /// 
                        listaProductos = dtProducto.Resultado.Skip((pagina - 1) * registros_por_pagina)
                                                         .Take(registros_por_pagina)
                                                         .ToList();
                        /// Se asigna el valor de la variable según la cantidad
                        /// de productos que se hayan determinado con el cálculo
                        _TotalRegistros = listaProductos.Count();

                        /// Se calcula y redondea la cantidad de páginas según los registros totales
                        _TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / registros_por_pagina);
                        #endregion
                        mensaje = new string[2];
                        mensaje[0] = "012";
                        mensaje[1] = "Se ejecutó correctamente la consulta.";

                        /// Se inicializa y declaran las propiedades del objeto PaginadorProducto
                        /// Para la respuesta de la solicitud
                        
                        PaginadorProducto<ProductosResponse> paginadorProducto = new PaginadorProducto<ProductosResponse>
                        {
                            /// Se asigna el valor a las propiedades del objeto
                            PaginaActual = pagina,
                            Resultado = listaProductos,
                            RegistrosPorPagina = registros_por_pagina,
                            TotalRegistros = TotalRegistros,
                            TotalPaginas = _TotalPaginas

                        };
                        /// Se asigna el valor a la variable de salida el objeto que contiene la respuesta de la solicitud
                        dtProducto = paginadorProducto;
                        respuesta.ListaProductos = dtProducto;

                    }
                    else
                    {
                        /// Se inicializa un array de strings para pasar los mensajes de respuesta
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
            /// Se retorna el cúmulo de datos del proceso en la variable respuesta
            return respuesta;
        }
    }
}