using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    [DataContract]
    public class Agencia
    {
        [DataMember]
        public string CodAge { get; set; }
        [DataMember]
        public string NomAge { get; set; }
        
    }
}