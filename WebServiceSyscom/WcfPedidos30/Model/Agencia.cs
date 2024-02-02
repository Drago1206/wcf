using System.Runtime.Serialization;

namespace WcfPedidos30.Model
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