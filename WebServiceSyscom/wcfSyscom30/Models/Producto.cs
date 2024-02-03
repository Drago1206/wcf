using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace wcfSyscom30.Models
{
    public class Producto
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string IdProducto { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public decimal Cantidad { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int NroLista { get; set; }
    }
}