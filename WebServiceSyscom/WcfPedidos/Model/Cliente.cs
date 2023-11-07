using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos.Model
{
    [DataContract]
    public class ClienteResponse
    {
        string IdTercero { get; set; }
        string RazonSocial { get; set; }
        string Direccion { get; set; }
        string IdLocal { get; set; }
        string Localidad { get; set; }
        string Telefono { get; set; }
        string IdRegimen { get; set; }
        string Regimen { get; set; }
        string IdGrupo { get; set; }
        string IdSector { get; set; }
        string DiasEntrega { get; set; }
        string SectorEco { get; set; }
        string IdPlazo { get; set; }
        string Plazo { get; set; }
        string IdZona { get; set; }
        string Zona { get; set; }
        string IdSZona { get; set; }
        string Subzona { get; set; }
        string IdRuta { get; set; }
        string Ruta { get; set; }
        string CodVend { get; set; }
        string CdCms { get; set; }
        decimal? TarCms { get; set; }
        string IdVend { get; set; }
        string Vendedor { get; set; }
        string NumLista { get; set; }
        string Inactivo { get; set; }
        List<Agencia> Agencias { get; set; }

        [DataMember]
        public string pmIdTercero { get { return IdTercero; } set { IdTercero = value; } }
        [DataMember]
        public string pmRazonSocial { get { return RazonSocial; } set { RazonSocial = value; } }
        [DataMember]
        public string pmDireccion { get { return Direccion; } set { Direccion = value; } }
        [DataMember]
        public string pmIdLocal { get { return IdLocal; } set { IdLocal = value; } }
        [DataMember]
        public string pmLocalidad { get { return Localidad; } set { Localidad = value; } }
        [DataMember]
        public string pmTelefono { get { return Telefono; } set { Telefono = value; } }
        [DataMember]
        public string pmIdRegimen { get { return IdRegimen; } set { IdRegimen = value; } }
        [DataMember]
        public string pmRegimen { get { return Regimen; } set { Regimen = value; } }
        [DataMember]
        public string pmIdGrupo { get { return IdGrupo; } set { IdGrupo = value; } }
        [DataMember]
        public string pmIdSector { get { return IdSector; } set { IdSector = value; } }
        [DataMember]
        public string pmSectorEco { get { return SectorEco; } set { SectorEco = value; } }
        [DataMember]
        public string pmIdPlazo { get { return IdPlazo; } set { IdPlazo = value; } }
        [DataMember]
        public string pmPlazo { get { return Plazo; } set { Plazo = value; } }
        [DataMember]
        public string pmIdZona { get { return IdZona; } set { IdZona = value; } }
        [DataMember]
        public string pmZona { get { return Zona; } set { Zona = value; } }
        [DataMember]
        public string pmIdSZona { get { return IdSZona; } set { IdSZona = value; } }
        [DataMember]
        public string pmSubzona { get { return Subzona; } set { Subzona = value; } }
        [DataMember]
        public string pmIdRuta { get { return IdRuta; } set { IdRuta = value; } }
        [DataMember]
        public string pmRuta { get { return Ruta; } set { Ruta = value; } }
        [DataMember]
        public string pmCdCms { get { return CdCms; } set { CdCms = value; } }
        [DataMember]
        public decimal? pmTarCms { get { return TarCms; } set { TarCms = value; } }
        [DataMember]
        public string pmIdVend { get { return IdVend; } set { IdVend = value; } }
        [DataMember]
        public string pmVendedor { get { return Vendedor; } set { Vendedor = value; } }
        [DataMember]
        public List<Agencia> pmAgencias { get { return Agencias; } set { Agencias = value; } }
        [DataMember]
        public string pmDiasEntrega { get { return DiasEntrega; } set { DiasEntrega = value; } }
        [DataMember]
        public string pmNumLista { get { return NumLista; } set { NumLista = value; } }
        [DataMember]
        public string pmCodVend { get { return CodVend; } set { CodVend = value; } }
        [DataMember]
        public string pmInactivo { get { return Inactivo; } set { Inactivo = value; } }
    }

    [DataContract]
    public class ClienteRequest
    {
        private string pmIdTercero { get; set; }
        private string pmNombres { get; set; }
        private string pmApellidos { get; set; }
        private string pmTipoDoc { get; set; }
        private string pmDireccion { get; set; }
        private string pmIdLocal { get; set; }
        private string pmTelefono { get; set; }
        private string pmIdSector { get; set; }
        private string pmIdRegimen { get; set; }
        private string pmTipEnte { get; set; }
        private string pmMunCCExp { get; set; }
        private string pmDiasEntrega { get; set; }
        private string pmIdZona { get; set; }
        private string pmIdGrupo { get; set; }
        private string pmIdPlazo { get; set; }
        private string pmIdRuta { get; set; }
        private string pmIdCentroCosto { get; set; }
        private string pmIdSCCosto { get; set; }

        [DataMember]
        public string Documento { get { return pmIdTercero; } set { pmIdTercero = value; } }
        [DataMember]
        public string Nombres { get { return pmNombres; } set { pmNombres = value; } }
        [DataMember]
        public string Apellidos { get { return pmApellidos; } set { pmApellidos = value; } }
        [DataMember]
        public string TipoDoc { get { return pmTipoDoc; } set { pmTipoDoc = value; } }
        [DataMember]
        public string Direccion { get { return pmDireccion; } set { pmDireccion = value; } }
        [DataMember]
        public string Municipio { get { return pmIdLocal; } set { pmIdLocal = value; } }
        [DataMember]
        public string Telefono { get { return pmTelefono; } set { pmTelefono = value; } }
        [DataMember]
        public string ActEconomica { get { return pmIdSector; } set { pmIdSector = value; } }
        [DataMember]
        public string RegimenDIAN { get { return pmIdRegimen; } set { pmIdRegimen = value; } }
        [DataMember]
        public string TipEnte { get { return pmTipEnte; } set { pmTipEnte = value; } }
        [DataMember]
        public string MunExpedicion { get { return pmMunCCExp; } set { pmMunCCExp = value; } }
        [DataMember]
        public string DiasEntrega { get { return pmDiasEntrega; } set { pmDiasEntrega = value; } }
        [DataMember]
        public string Zona { get { return pmIdZona; } set { pmIdZona = value; } }
        [DataMember]
        public string Grupo { get { return pmIdGrupo; } set { pmIdGrupo = value; } }
        [DataMember]
        public string Plazo { get { return pmIdPlazo; } set { pmIdPlazo = value; } }
        [DataMember]
        public string Ruta { get { return pmIdRuta; } set { pmIdRuta = value; } }
        [DataMember]
        public string CentroCosto { get { return pmIdCentroCosto; } set { pmIdCentroCosto = value; } }
        [DataMember]
        public string SubCCosto { get { return pmIdSCCosto; } set { pmIdSCCosto = value; } }
    }
}