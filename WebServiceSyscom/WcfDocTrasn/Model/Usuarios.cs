using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class Usuarios
    {
        [DataMember(IsRequired = true)]
        public string Usuario { get; set; }
        [DataMember(IsRequired = true)]
        public string Password { get; set; }
        [DataMember(IsRequired = true)]
        public string Compania { get; set; }
        [DataMember(IsRequired = true)]
        public string Nit { get; set; }
    }
}