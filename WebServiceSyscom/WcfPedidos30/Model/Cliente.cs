using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{

    [DataContract]
    public class ClienteResponse
    {
        [DataMember]
        public string NitCliente { get; set; }
        [DataMember]
        public string NomCliente { get; set; }
        [DataMember]
        public string Direccion { get; set; }
        [DataMember]
        public string Ciudad { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public int NumLista { get; set; }
        [DataMember]
        public string NitVendedor { get; set; }
        [DataMember]
        public string NomVendedor { get; set; }
        [DataMember]
        public List<Agencia> ListaAgencias { get; set; }
       
    }
    public class ClienteRequest
    {
        [DataMember]
        public string NitCliente { get; set; }
    }
}