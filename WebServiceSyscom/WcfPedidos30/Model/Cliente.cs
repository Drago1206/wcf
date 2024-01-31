using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Model
{
    [DataContract]
    public class ClienteResponse 
    {
        public string NitCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public int NumLista { get; set; }
        public string NitVendedor { get; set; }
        public string NomVendedor { get; set; }
        public List<Agencia> ListaAgencias { get; set; }

        [DataMember]
        public string pmNitCliente { get { return NitCliente; } set { NitCliente = value; } }
        [DataMember]
        public string pmCiudad { get { return Ciudad; } set { Ciudad = value; } }
        [DataMember]
        public string pmDireccion { get { return Direccion; } set { Direccion = value; } }
        [DataMember]
        public List<Agencia> pmListaAgencia { get { return ListaAgencias; } set { ListaAgencias = value; } }
        [DataMember]
        public string pmNitVendedor { get { return NitVendedor; } set { NitVendedor = value; } }
        [DataMember]
        public string pmNomVendedor { get { return NomVendedor; } set { NomVendedor = value;} }
        [DataMember]
        public int pmNumLista { get { return NumLista; } set { NumLista = value; } }

    }
    public class ClienteRequest
    {
        [DataMember]
        public string NitCliente { get; set; }
    }
}