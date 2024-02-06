using System.Collections.Generic;

namespace WcfPedidos30.Model
{
    public class PaginadorProducto<T> where T : class
    {
        /// <summary>
        /// Página devuelta por la consulta actual.
        /// </summary>
        public int PaginaActual { get; set; }
        /// <summary>
        /// Número de registros de la página devuelta.
        /// </summary>
        public int RegistrosPorPagina { get; set; }
        /// <summary>
        /// Total de registros de consulta.
        /// </summary>
        public int TotalRegistros { get; set; }
        /// <summary>
        /// Total de páginas de la consulta.
        /// </summary>
        public int TotalPaginas { get; set; }
        /// <summary>
        /// Texto de búsqueda de la consuta actual.
        /// </summary>
        //public IEnumerable<T> Resultado { get; set; }

        public List<ProductosResponse> Resultado { get; set; }
    }
}