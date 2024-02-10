using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class Items
    {
        [DataMember(IsRequired = true)]
        public string NitRemitente { get; set; }
        [DataMember(IsRequired = true)]
        public string SedeRemitente { get; set; }
        [DataMember(IsRequired = true)]
        public string NitDestinatario { get; set; }
        [DataMember(IsRequired = true)]
        public string SedeDestinatario { get; set; }
        [DataMember(IsRequired = true)]
        public string IdMercancia { get; set; }
        [DataMember(IsRequired = false)]
        public string DescripMcias { get; set; }
        [DataMember(IsRequired = false)]
        public decimal Cantidad { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<int> Cases { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<int> Cajas { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<int> Palets { get; set; }
        [DataMember(IsRequired = true)]
        public decimal Peso { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> dmsLargo { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> dmsAncho { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> dmsAlto { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> Volumen { get; set; }
        [DataMember(IsRequired = false)]
        public string UndVol { get; set; }
        [DataMember(IsRequired = false)]
        public string UndMed { get; set; }
        [DataMember(IsRequired = false)]
        public string CiudadOrigen { get; set; }
        [DataMember(IsRequired = false)]
        public string CiudadDestino { get; set; }
        [DataMember(IsRequired = true)]
        public decimal TarifaCliente { get; set; }
        [DataMember(IsRequired = true)]
        public string UnidadCliente { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> TarifaTabla { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> TarifaPago { get; set; }
        [DataMember(IsRequired = false)]
        public string UndTarifaPago { get; set; }
        [DataMember(IsRequired = false)]
        public string DirOrigen { get; set; }
        [DataMember(IsRequired = false)]
        public string DirDestino { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> VrDeclarado { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<decimal> TarifSeguro { get; set; }
        [DataMember(IsRequired = false)]
        public string Referencia1 { get; set; }
        [DataMember(IsRequired = false)]
        public string Referencia2 { get; set; }
        [DataMember(IsRequired = false)]
        public string Referencia3 { get; set; }
        //Embalaje
        [DataMember(IsRequired = false)]
        public string IdEmpaque { get; set; }
        [DataMember(IsRequired = false)]
        public string IdNaturaleza { get; set; }
        [DataMember(IsRequired = false)]
        public string IdManejo { get; set; }
        //Riesgos-IdTmcia
        [DataMember(IsRequired = true)]
        public string Riesgos { get; set; }
        [DataMember(IsRequired = false)]
        public string CdRango { get; set; }
        [DataMember(IsRequired = false)]
        public string DocCliente { get; set; }
        [DataMember(IsRequired = false)]
        public string Contenedor1 { get; set; }
        [DataMember(IsRequired = false)]
        public string Contenedor2 { get; set; }
        [DataMember(IsRequired = true)]
        public string Tipo_Servicio { get; set; }
       
        [DataMember(IsRequired = false)]
        public string Embalajes { get; set; }
    }

    public class itemnovedad
    {
        public int item { get; set; }
        public string Descripcion { get; set; }
        public string Rubro { get; set; }
        public int Valor { get; set; }
    }
}