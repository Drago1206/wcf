using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfSyscom30.Models
{
    public class ItemCartera
    {
        public string TipoDocumento { get; set; }
        public int Documento { get; set; }
        public string Compañia { get; set; }
        public int Vencimiento { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int ValorTotal { get; set; }
        public int Abono { get; set; }
        public int Saldo { get; set; }
    }
}