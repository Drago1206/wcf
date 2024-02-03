using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wcfSyscom30.Models
{
    public class Clientes
    {
        public string NitCliente { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
    }
}