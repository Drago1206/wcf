using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{
    [DataContract]

    
    public class Agencia
    {
        [DataMember]
        string CodAge { get; set; }
        [DataMember]
        string NomAge { get; set; }

       
    }
}