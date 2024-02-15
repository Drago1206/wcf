using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class Conceptos
    {
        [DataMember(IsRequired = false)]
        public string CodigoConcepto { get; set; }
        [DataMember(IsRequired = false)]
        public string Descripcion { get; set; }
        [DataMember(IsRequired = false)]
        public int Cantidad { get; set; }
        [DataMember(IsRequired = false)]
        public decimal VrUnitario { get; set; }
        [DataMember(IsRequired = false)]
        public string Rubro { get; set; }
        [DataMember(IsRequired = false)]
        public string TipoConcepto { get; set; }
        [DataMember(IsRequired = false)]
        public string Nit { get; set; }
        [DataMember(IsRequired = false)]
        public string TipoEscolta { get; set; }
    }
}