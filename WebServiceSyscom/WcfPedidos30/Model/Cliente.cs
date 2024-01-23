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
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public List<Agencia> ListaAgencias { get; set; }
        public string CodAge { get; set; }
        public string NomAge { get; set; }
        public string NitVendedor { get; set; }
        public string NomVendedor { get; set; }
        public int NumLista { get; set; }

        [DataMember]
        public string pmNitCliente { get { return NitCliente; } set { NitCliente = value; } }
        [DataMember]
        public int pmPaginaActual { get { return PaginaActual; } set { PaginaActual = value; } }
        [DataMember]
        public int pmRegistrosPorPagina { get { return RegistrosPorPagina; } set { RegistrosPorPagina = value; } }
        [DataMember]
        public string pmCiudad { get { return Ciudad; } set { Ciudad = value; } }
        [DataMember]
        public string pmDireccion { get { return Direccion; } set { Direccion = value; } }
        [DataMember]
        public List<Agencia> pmListaAgencia { get { return ListaAgencias; } set { ListaAgencias = value; } }
        [DataMember]
        public string pmCodAge { get { return CodAge; } set { CodAge = value; } }
        [DataMember]
        public string pmNomAge { get { return NomAge; } set { NomAge = value; } }
        [DataMember]
        public string pmNitVendedor { get { return NitVendedor; } set { NitVendedor = value; } }
        [DataMember]
        public string pmNomVendedor { get { return NomVendedor; } set { NomVendedor = value;} }
        [DataMember]
        public int pmNumLista { get { return NumLista; } set { NumLista = value; } }

    }
    public class ClienteRequest
    {
        public string NitCliente { get; set; }
        public int PaginaActual { get; set; }
    }
}