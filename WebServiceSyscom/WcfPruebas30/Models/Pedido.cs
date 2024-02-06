using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace WcfPedidos30.Models
{
    public class Pedido
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string IdCliente { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string IdAgencia { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string CodConcepto { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string IdVendedor { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int VrFlete { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string Observación { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public List<ProductosResponse> ListaProductos { get; set; }
    }
}