using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos40.Models
{
    [DataContract]
    public class ClienteResponse
    {
        private string pmIdTercero { get; set; }
        private string pmRazonSocial { get; set; }
        private string pmDireccion { get; set; }
        private string pmIdLocal { get; set; }
        private string pmLocalidad { get; set; }
        private string pmTelefono { get; set; }
        private string pmIdRegimen { get; set; }
        private string pmRegimen { get; set; }
        private string pmIdGrupo { get; set; }
        private string pmIdSector { get; set; }
        private string pmDiasEntrega { get; set; }
        private string pmSectorEco { get; set; }
        private string pmIdPlazo { get; set; }
        private Int32? pmPlazo { get; set; }
        //Versión 003
        private string pmIdZona { get; set; }
        private string pmZona { get; set; }
        //=======================
        private string pmIdSZona { get; set; }
        private string pmSubzona { get; set; }
        //Versión 003
        private string pmIdRuta { get; set; }
        private string pmRuta { get; set; }
        //=========================
        private string pmCodVend { get; set; }
        private string pmCdCms { get; set; }
        private decimal? pmTarCms { get; set; }
        private string pmIdVend { get; set; }
        private string pmVendedor { get; set; }
        private string pmNumLista { get; set; }
        private string pmInactivo { get; set; }
        
        [DataMember]
        public string IdTercero { get { return pmIdTercero; } set { pmIdTercero = value; } }
        [DataMember]
        public string RazonSocial { get { return pmRazonSocial; } set { pmRazonSocial = value; } }
        [DataMember]
        public string Direccion { get { return pmDireccion; } set { pmDireccion = value; } }
        [DataMember]
        public string IdLocal { get { return pmIdLocal; } set { pmIdLocal = value; } }
        [DataMember]
        public string Localidad { get { return pmLocalidad; } set { pmLocalidad = value; } }
        [DataMember]
        public string Telefono { get { return pmTelefono; } set { pmTelefono = value; } }
        [DataMember]
        public string IdRegimen { get { return pmIdRegimen; } set { pmIdRegimen = value; } }
        [DataMember]
        public string Regimen { get { return pmRegimen; } set { pmRegimen = value; } }
        [DataMember]
        public string IdGrupo { get { return pmIdGrupo; } set { pmIdGrupo = value; } }
        [DataMember]
        public string IdSector { get { return pmIdSector; } set { pmIdSector = value; } }
        [DataMember]
        public string SectorEco { get { return pmSectorEco; } set { pmSectorEco = value; } }
        [DataMember]
        public string IdPlazo { get { return pmIdPlazo; } set { pmIdPlazo = value; } }
        [DataMember]
        public Int32? Plazo { get { return pmPlazo; } set { pmPlazo = value; } }
        [DataMember]
        public string IdZona { get { return pmIdZona; } set { pmIdZona = value; } }
        [DataMember]
        public string Zona { get { return pmZona; } set { pmZona = value; } }
        [DataMember]
        public string IdSZona { get { return pmIdSZona; } set { pmIdSZona = value; } }
        [DataMember]
        public string Subzona { get { return pmSubzona; } set { pmSubzona = value; } }
        [DataMember]
        public string IdRuta { get { return pmIdRuta; } set { pmIdRuta = value; } }
        [DataMember]
        public string Ruta { get { return pmRuta; } set { pmRuta = value; } }
        [DataMember]
        public string CdCms { get { return pmCdCms; } set { pmCdCms = value; } }
        [DataMember]
        public decimal? TarCms { get { return pmTarCms; } set { pmTarCms = value; } }
        [DataMember]
        public string IdVend { get { return pmIdVend; } set { pmIdVend = value; } }
        [DataMember]
        public string Vendedor { get { return pmVendedor; } set { pmVendedor = value; } }
        
        [DataMember]
        public string DiasEntrega { get { return pmDiasEntrega; } set { pmDiasEntrega = value; } }
        [DataMember]
        public string NumLista { get { return pmNumLista; } set { pmNumLista = value; } }
        [DataMember]
        public string CodVend { get { return pmCodVend; } set { pmCodVend = value; } }
        [DataMember]
        public string Inactivo { get { return pmInactivo; } set { pmInactivo = value; } }

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