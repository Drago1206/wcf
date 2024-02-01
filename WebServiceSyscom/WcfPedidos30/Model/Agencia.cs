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
        string mCodAge { get; set; }
        string mNomAge { get; set; }

        [DataMember]
        public string CodAge { get { return mCodAge; } set { mCodAge = value; } }
        [DataMember]
        public string NomAge { get { return mNomAge; } set { mNomAge = value; } }
    }
}