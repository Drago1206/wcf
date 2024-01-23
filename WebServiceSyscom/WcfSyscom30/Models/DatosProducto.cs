using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfSyscom30.Models
{
    public class DatosProducto
    {
        public string CodProducto { get; set; }
        public string CodConcepto { get; set; }
        public int CantidadProducto { get; set; }
        public int TotalProducto { get; set; }
        public int Iva { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
    }
}