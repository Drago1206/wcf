using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfSyscom30.Models
{
    public class Usuario
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string UserName { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string Password { get; set; }
    }
}