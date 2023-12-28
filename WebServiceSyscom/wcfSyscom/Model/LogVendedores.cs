using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wcfSyscom.Model
{
    public class LogVendedores
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string ClaveReg { get; set; }
        public string TipoProc { get; set; }
        public string IdCliente { get; set; }
        public string CdAgencia { get; set; }
        public string CdVendAnt { get; set; }
        public string IdVend { get; set; }
        public DateTime FechaCrea { get; set; }
        public string IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string NomCliente { get; set; }
        public string NomAgencia { get; set; }
        public string NomVendedor { get; set; }
        public string NomVendAnt { get; set; }
    }
}