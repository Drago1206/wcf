using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Model
{
    [DataContract]
    public class Agencia
    {
        string CodAge { get; set; }
        string NomAge { get; set; }

        [DataMember]
        public string pmCodAge { get { return CodAge; } set { CodAge = value; } }
        [DataMember]
        public string pmNomAge { get { return NomAge; } set { NomAge = value; } }
    }
}