using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace wcfSyscom30.Models
{
    //
    public class Cliente
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string NitCliente { get; set; }
    }
}