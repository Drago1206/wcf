using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Model
{
    [DataContract]
    public class PedidoResponse
    {
        [DataMember]
        public string TipoDoc { get; set; }
        [DataMember]
        public string IdCia { get; set; }
        [DataMember]
        public DateTime Fecha { get; set; }
        [DataMember]
        public int TotalProductos { get; set; }
        [DataMember]
        public int Subtotal { get; set; }
        [DataMember]
        public int Descuento { get; set; }
        [DataMember]
        public int Iva { get; set; }
    }
    
    [DataContract]
    public class PedidoRequest
    {
        [DataMember]
        public string IdCliente { get; set; }
        [DataMember]
        public string IdAgencia { get; set; }
        [DataMember]
        public string CodConcepto { get; set; }
        [DataMember]
        public string IdVendedor { get; set; }
        [DataMember]
        public int VrFlete { get; set; }
        [DataMember]
        public string Observación { get; set; }
        [DataMember]
        public List<ProductosPed> ListaProductos { get ; set; }
    }
}