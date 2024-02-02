using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{

    [DataContract]
    public class ClienteResponse
    {
        /// <summary>
        /// NIT del cliente
        /// </summary>
        [DataMember]
        public string NitCliente { get; set; }

        /// <summary>
        /// Nombre del cliente
        /// </summary>
        [DataMember]
        public string NombreCliente { get; set; }
        /// <summary>
        /// Direccion del cliente
        /// </summary>
        [DataMember]
        public string Direccion { get; set; }
        /// <summary>
        /// Ciudad del cliente
        /// </summary>
        [DataMember]
        public string Ciudad { get; set; }
        /// <summary>
        /// Telefono del cliente
        /// </summary>
        [DataMember]
        public string Telefono { get; set; }
        /// <summary>
        /// Numero de lista
        /// </summary>
        [DataMember]
        public int NumLista { get; set; }

        /// <summary>
        /// NIT del vendedor
        /// </summary>
        [DataMember]
        public string NitVendedor { get; set; }
        /// <summary>
        /// Nombre del vendedor
        /// </summary>
        [DataMember]
        public string NomVendedor { get; set; }
        /// <summary>
        /// Lista de agencias
        /// </summary>
        [DataMember]
        public List<Agencia> ListaAgencias { get; set; }
       
    }
    public class ClienteRequest
    {
        /// <summary>
        /// Nit del cliente
        /// </summary>
        [DataMember]
        public string NitCliente { get; set; }
    }
}