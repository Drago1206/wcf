﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfPedidos30.Model
{
    public class PaginadorCliente<T> where T : class
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
        public IEnumerable<T> Resultado { get; set; }

        public static implicit operator PaginadorCliente<T>(ClienteResponse v)
        {
            throw new NotImplementedException();
        }
    }
}