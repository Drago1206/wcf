using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    [DataContract]
    public class ClienteResponse
    {
        private string pmNitCliente { get; set; }
        private string pmNombreCliente { get; set; }
        private string pmCiudad { get; set; }
        private string pmDireccion { get; set; }
        private string pmNitVendedor { get; set; }
        private string pmNomVendedor { get; set; }
        private int pmNumLista { get; set; }
        private string pmTelefono { get; set; }

        [DataMember]
        public List<Agencia> ListaAgencia { get; set; }
        private PaginadorCliente<ClienteResponse> pmPaginadorCliente;

        [DataMember]
        public string NitCliente { get { return pmNitCliente; } set { pmNitCliente = value; } }
        [DataMember]
        public string NombreCliente { get { return pmNombreCliente; } set { pmNombreCliente = value; } }

        [DataMember]
        public string Ciudad { get { return pmCiudad; } set { pmCiudad = value; } }
        [DataMember]
        public string Direccion { get { return pmDireccion; } set { pmDireccion = value; } }       
        
        [DataMember]
        public string NitVendedor { get { return pmNitVendedor; } set { pmNitVendedor = value; } }
        [DataMember]
        public string NomVendedor { get { return pmNomVendedor; } set { pmNomVendedor = value; } }
        [DataMember]
        public int NumLista { get { return pmNumLista; } set { pmNumLista = value; } }
        [DataMember]
        public string Telefono { get { return pmTelefono; } set { pmTelefono = value; } }
        /*
        [DataMember]
        public List<Agencia> ListaAgencia { get { return pmListaAgencias; } set { pmListaAgencias = value; } }
       */

    }
    public class ClienteRequest
    {
        public string NitCliente { get; set; }
        public int Password { get; set; }
        public string UserName { get; set; }

        [DataMember]
        public string pmNitCliente { get { return NitCliente; } set { NitCliente = value; } }
        [DataMember]
        public string pmUserName { get { return UserName; } set { UserName = value; } }
        [DataMember]
        public int pmPassword { get { return Password; } set { Password = value; } }
    }

}