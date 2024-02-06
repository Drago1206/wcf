using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    [DataContract]
    public class Cartera
    {   [DataMember]
        public int Abono { get; set; }
        [DataMember]
        public string Compañia { get; set; }
        [DataMember]
        public int Documento { get; set; }
        [DataMember]
        public string TipoDocumento { get; set; }
        [DataMember]

        public int Vencimiento { get; set; }
        [DataMember]
        public DateTime FechaEmision { get; set; }
        [DataMember]
        public DateTime FechaVencimiento { get; set; }
        [DataMember]
        public int ValorTotal { get; set; }
        [DataMember]

        public int Saldo { get; set; }


    }
    [DataContract]
    public class ItemCartera
    {

        [DataMember]
        public string Tercero { get; set; }
        [DataMember]
        public int SaldoCartera { get; set; }
        [DataMember]
        public List<Cartera> Detalle { get; set; }

    }
}