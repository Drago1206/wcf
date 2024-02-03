using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wcfSyscom30.Models
{
    public class DatosProducto
    {
        public string CodOrDesProd { get; set; }
        public string Subgrupo { get; set; }
        public string Grupo { get; set; }
        public bool SaldosCiaBod { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
    }
}