using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class Pedidos
    {
        [DataMember(IsRequired = true)]
        public DateTime Fecha { get; set; }
        [DataMember(IsRequired = true)]
        public int DiasVigencia { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime FecDespacho { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime FecEntrega { get; set; }
        [DataMember(IsRequired = true)]
        public string NitCliente { get; set; }
        [DataMember(IsRequired = false)]
        public string IdAgencia { get; set; }
        [DataMember(IsRequired = false)]
        public string Moneda { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrTasa { get; set; }
        [DataMember(IsRequired = true)]
        public string IdVend { get; set; }
        [DataMember(IsRequired = false)]
        public string TarifaComision { get; set; }

        //Datos adicionales
        [DataMember(IsRequired = false)]
        public string IdRuta { get; set; }
        [DataMember(IsRequired = false)]
        public string Modalidad { get; set; }
        [DataMember(IsRequired = false)]
        public string Vigencia { get; set; }
        [DataMember(IsRequired = false)]
        public string TipoTarifa { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrEscolta { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrDevolucionContdor { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrTraUrbano { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrEmbalajes { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrCargos { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrDctos { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrCargue { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrDesCargue { get; set; }


        //anexo
        [DataMember(IsRequired = false)]
        public string TipoTransporte { get; set; }
        [DataMember(IsRequired = false)]
        public Boolean PolizaEspecifica { get; set; }
        [DataMember(IsRequired = false)]
        public string NumPolizaEsp { get; set; }
        [DataMember(IsRequired = false)]
        public string NitCiaPoliza { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<DateTime> FecVencePol { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrLimiteDesp { get; set; }
        [DataMember(IsRequired = false)]
        public string NitSIA { get; set; }
        [DataMember(IsRequired = false)]
        public string ContactoSIA { get; set; }
        [DataMember(IsRequired = false)]
        public string TeleContactoSIA { get; set; }
        [DataMember(IsRequired = true)]
        public string TipoRuta { get; set; }
        [DataMember(IsRequired = false)]
        public string Embarque { get; set; }
        [DataMember(IsRequired = true)]
        public string CdTipCarga { get; set; }
        [DataMember(IsRequired = true)]
        public string Seguros { get; set; }
        [DataMember(IsRequired = true)]
        public string Cargue { get; set; }
        [DataMember(IsRequired = true)]
        public string Descargue { get; set; }
        [DataMember(IsRequired = true)]
        public string TipoMargen { get; set; }
        [DataMember(IsRequired = true)]
        public decimal MargenFalt { get; set; }
        [DataMember(IsRequired = true)]
        public string UndCalcFalt { get; set; }
        [DataMember(IsRequired = true)]
        public decimal TarifFaltPago { get; set; }
        [DataMember(IsRequired = true)]
        public decimal TarifFaltCobro { get; set; }
        [DataMember(IsRequired = false)]
        public string Observacion { get; set; }
        [DataMember(IsRequired = true)]
        public Boolean DevContenedor { get; set; }
        [DataMember(IsRequired = true)]
        public string PatioCont { get; set; }
        [DataMember(IsRequired = true)]
        public string CiudadDevContenedor { get; set; }
        [DataMember(IsRequired = true)]
        public string NomContacto { get; set; }
        [DataMember(IsRequired = true)]
        public string TelContacto { get; set; }
        [DataMember(IsRequired = true)]
        public string emailContac { get; set; }
        [DataMember(IsRequired = true)]
        public string NomContactoDest { get; set; }
        [DataMember(IsRequired = true)]
        public string TelContactoDest { get; set; }
        [DataMember(IsRequired = true)]
        public string emailContacDest { get; set; }
        [DataMember(IsRequired = true)]
        public string CdTipoEsc { get; set; }
        [DataMember(IsRequired = true)]
        public string CdTipoVehic { get; set; }
    }
}