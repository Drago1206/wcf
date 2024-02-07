using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos40.Model
{
    public class Clientes
    {
        [DataContract]
        public class ClienteResponse
        {
            [DataMember]
            public string IdTercero { get; set; }
            [DataMember]
            public string RazonSocial { get; set; }
            [DataMember]
            public string Direccion { get; set; }
            [DataMember]
            public string IdLocal { get; set; }
            [DataMember]
            public string Localidad { get; set; }
            [DataMember]
            public string Telefono { get; set; }
            [DataMember]
            public string IdRegimen { get; set; }
            [DataMember]
            public string Regimen { get; set; }
            [DataMember]
            public string IdGrupo { get; set; }
            [DataMember]
            public string IdSector { get; set; }
            [DataMember]
            public string DiasEntrega { get; set; }
            [DataMember]
            public string SectorEco { get; set; }
            [DataMember]
            public string IdPlazo { get; set; }
            [DataMember]
            public Int32? Plazo { get; set; }
            //Versión 003
            [DataMember]
            public string IdZona { get; set; }
            [DataMember]
            public string Zona { get; set; }
            //=======================
            [DataMember]
            public string IdSZona { get; set; }
            [DataMember]
            public string Subzona { get; set; }
            [DataMember]
            //Versión 003
            public string IdRuta { get; set; }
            [DataMember]
            public string Ruta { get; set; }
            [DataMember]
            //=========================
            public string CodVend { get; set; }
            [DataMember]
            public string CdCms { get; set; }
            [DataMember]
            public decimal? TarCms { get; set; }
            [DataMember]
            public string IdVend { get; set; }
            [DataMember]
            public string Vendedor { get; set; }
            [DataMember]
            public string NumLista { get; set; }
            [DataMember]
            public string Inactivo { get; set; }
            [DataMember]
            public List<Agencia> Agencias { get; set; }
        }
    }
}