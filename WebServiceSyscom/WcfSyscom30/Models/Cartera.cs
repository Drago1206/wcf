using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfSyscom30.Models
{
    public class Cartera
    {
        public int Abono { get; set; }
        public string Compania { get; set; }
        public int DocumentoCliente { get; set; }
        public string TipoDocumentoCliente { get; set; }
        public string Tercero { get; set; }

        public int VencimientoFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int ValorTotal { get; set; }

        public int Saldo { get; set; }
        public int SaldoCartera { get; set; }

        [DataMember]
        public int _Abono { get { return _Abono; } set { _Abono = value; } }

        [DataMember]
        public String _Compania { get { return _Compania; } set { _Compania = value; } }

        [DataMember]
        public int _DocumentoCliente { get { return _DocumentoCliente; } set { _DocumentoCliente = value; } }

        [DataMember]
        public string _TipoDocumentoCliente { get { return _TipoDocumentoCliente; } set { _TipoDocumentoCliente = value; } }

        [DataMember]
        public string _Terceros { get { return _Terceros; } set { _Terceros = value; } }

        [DataMember]
        public int _VencimientoFactura { get { return _VencimientoFactura; } set { _VencimientoFactura = value; } }

        [DataMember]
        public DateTime _FechaEmision { get { return _FechaEmision; } set { _FechaEmision = value; } }

        [DataMember]
        public DateTime _FechaVencimiento { get { return _FechaVencimiento; } set { _FechaVencimiento = value; } }

        [DataMember]
        public DateTime _ValorTotal { get { return _ValorTotal; } set { _ValorTotal = value; } }

        [DataMember]
        public int _Saldo { get { return _Saldo; } set { _Saldo = value; } }

        [DataMember]
        public int _SaldoCartera { get { return _Saldo; } set { _Saldo = value; } }

    }
}