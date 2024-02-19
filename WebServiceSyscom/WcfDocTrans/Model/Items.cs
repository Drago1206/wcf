using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class Items
    {
        [DataMember]
        public string NitRemitente { get; set; }
        [DataMember]
        public string SedeRemitente { get; set; }
        [DataMember]
        public string NitDestinatario { get; set; }
        [DataMember]
        public string SedeDestinatario { get; set; }
        [DataMember]
        public string IdMercancia { get; set; }
        [DataMember]
        public string DescripMcias { get; set; }
        [DataMember]
        public decimal Cantidad { get; set; }
        [DataMember]
        public Nullable<int> Cases { get; set; }
        [DataMember]
        public Nullable<int> Cajas { get; set; }
        [DataMember]
        public Nullable<int> Palets { get; set; }
        [DataMember]
        public decimal Peso { get; set; }
        [DataMember]
        public Nullable<decimal> dmsLargo { get; set; }
        [DataMember]
        public Nullable<decimal> dmsAncho { get; set; }
        [DataMember]
        public Nullable<decimal> dmsAlto { get; set; }
        [DataMember]
        public Nullable<decimal> Volumen { get; set; }
        [DataMember]
        public string UndVol { get; set; }
        [DataMember]
        public string UndMed { get; set; }
        [DataMember]
        public string CiudadOrigen { get; set; }
        [DataMember]
        public string CiudadDestino { get; set; }
        [DataMember]
        public decimal TarifaCliente { get; set; }
        [DataMember]
        public string UnidadCliente { get; set; }
        [DataMember]
        public Nullable<decimal> TarifaTabla { get; set; }
        [DataMember]
        public Nullable<decimal> TarifaPago { get; set; }
        [DataMember]
        public string UndTarifaPago { get; set; }
        [DataMember]
        public string DirOrigen { get; set; }
        [DataMember]
        public string DirDestino { get; set; }
        [DataMember]
        public Nullable<decimal> VrDeclarado { get; set; }
        [DataMember]
        public Nullable<decimal> TarifSeguro { get; set; }
        [DataMember]
        public string Referencia1 { get; set; }
        [DataMember]
        public string Referencia2 { get; set; }
        [DataMember]
        public string Referencia3 { get; set; }
        //Embalaje
        [DataMember]
        public string IdEmpaque { get; set; }
        [DataMember]
        public string IdNaturaleza { get; set; }
        [DataMember]
        public string IdManejo { get; set; }
        //Riesgos-IdTmcia
        [DataMember]
        public string Riesgos { get; set; }
        [DataMember]
        public string CdRango { get; set; }
        [DataMember]
        public string DocCliente { get; set; }
        [DataMember]
        public string Contenedor1 { get; set; }
        [DataMember]
        public string Contenedor2 { get; set; }
        [DataMember]
        public string Tipo_Servicio { get; set; }
       
        [DataMember]
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