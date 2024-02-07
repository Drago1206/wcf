using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{
    [DataContract]

    
    public class Agencia
    {
        /// <summary>
        /// Codigo de la agencia
        /// </summary>
        [DataMember]
        public string CodAge { get; set; }

        /// <summary>
        /// Nombre de la agencia
        /// </summary>
        [DataMember]
        public string NomAge { get; set; }

       
    }
}