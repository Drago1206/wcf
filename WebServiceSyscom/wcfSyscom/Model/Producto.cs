using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace wcfSyscom.Model
{
    public class Producto
    {
        private string IdProducto { get; set; }
        private string DescripProd { get; set; }
        private string IvaInc { get; set; }
        private string LtPreDef { get; set; }
        private decimal VrPrecio1 { get; set; }
        private decimal VrPrecio2 { get; set; }
        private decimal VrPrecio3 { get; set; }
        private decimal VrPrecio4 { get; set; }
        private decimal VrPrecio5 { get; set; }
        private decimal TarifaIva { get; set; }
        private bool ExcluidoImp { get; set; }
        private Int32 Cantidad { get; set; }
        private bool EsObsequio { get; set; }
        private List<string> DisponibleEnCia { get; set; }

        [DataMember] 
        public string pmIdProducto { get { return IdProducto; }set { IdProducto = value; } }
        
        [DataMember] 
        public string pmDescripProd { get { return DescripProd; }set { DescripProd = value; } }
        
        [DataMember] 
        public string pmIvaInc { get { return IvaInc; }set { IvaInc = value; } }

        [DataMember] 
        public string pmLtPreDef { get { return LtPreDef; }set { LtPreDef = value; } }
        
        [DataMember] 
        public decimal pmVrPrecio1 { get { return VrPrecio1; }set { VrPrecio1 = value; } }
        
        [DataMember] 
        public decimal pmVrPrecio2 { get { return VrPrecio2; }set { VrPrecio2 = value; } }
        
        [DataMember] 
        public decimal pmVrPrecio3 { get { return VrPrecio3; }set { VrPrecio3 = value; } }

        [DataMember]
        public decimal pmVrPrecio4 { get { return VrPrecio4; } set { VrPrecio4 = value; } }

        [DataMember]
        public decimal pmVrPrecio5 { get { return VrPrecio5; } set { VrPrecio5 = value; } }

        [DataMember]
        public decimal pmTarifaIva { get { return TarifaIva; } set { TarifaIva = value; } }

        [DataMember] 
        public bool pmExcluidoImp { get { return ExcluidoImp; }set { ExcluidoImp = value; } }

        [DataMember]
        public Int32 pmCantidad { get { return Cantidad; } set { Cantidad = value; } }

        [DataMember]
        public bool pmEsObsequio { get { return EsObsequio; } set { EsObsequio = value; } }

        [DataMember]
        public List<string> pmDisponibleEnConpania { get { return DisponibleEnCia; } set { DisponibleEnCia = value; } }
    }


}