using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace wcfSyscom.Model
{
    [DataContract]
    public class Agencia
    {
        string IdAgencia { get; set; }
        string NomAgencia { get; set; }
        string DirAgncia { get; set; }
        string AgIdLocal { get; set; }
        string AgLocalidad { get; set; }
        string TelAgncia { get; set; }
        string NomCont { get; set; }
        string emlCont { get; set; }
        string AgIdVend { get; set; }
        string AgVendedor { get; set; }
        string AgCdCms { get; set; }
        string DiasEntrega { get; set; }
        decimal? AgTarCms { get; set; }
        string AgCdDct { get; set; }
        decimal? AgTarDct { get; set; }
        //Versión 0003
        string AgIdZona { get; set; }
        string AgZona { get; set; }
        string AgIdSZona { get; set; }
        string AgSubZona { get; set; }
        string AgIdRuta { get; set; }
        string AgRuta { get; set; }
        [DataMember]
        public string pmIdAgencia { get { return IdAgencia; } set { IdAgencia = value; } }
        [DataMember]
        public string pmNomAgencia { get { return NomAgencia; } set { NomAgencia = value; } }
        [DataMember]
        public string pmDirAgncia { get { return DirAgncia; } set { DirAgncia = value; } }
        [DataMember]
        public string pmAgIdLocal { get { return AgIdLocal; } set { AgIdLocal = value; } }
        [DataMember]
        public string pmAgLocalidad { get { return AgLocalidad; } set { AgLocalidad = value; } }
        [DataMember]
        public string pmTelAgncia { get { return TelAgncia; } set { TelAgncia = value; } }
        [DataMember]
        public string pmNomCont { get { return NomCont; } set { NomCont = value; } }
        [DataMember]
        public string pmemlCont { get { return emlCont; } set { emlCont = value; } }
        [DataMember]
        public string pmAgIdVend { get { return AgIdVend; } set { AgIdVend = value; } }
        [DataMember]
        public string pmAgVendedor { get { return AgVendedor; } set { AgVendedor = value; } }
        [DataMember]
        public string pmAgCdCms { get { return AgCdCms; } set { AgCdCms = value; } }
        [DataMember]
        public decimal? pmAgTarCms { get { return AgTarCms; } set { AgTarCms = value; } }
        [DataMember]
        public string pmAgCdDct { get { return AgCdDct; } set { AgCdDct = value; } }
        [DataMember]
        public decimal? pmAgTarDct { get { return AgTarDct; } set { AgTarDct = value; } }
        [DataMember]
        public string pmDiasEntrega { get { return DiasEntrega; } set { DiasEntrega = value; } }
        //Versión 0003
        [DataMember]
        public string pmAgIdZona { get { return AgIdZona; } set { AgIdZona = value; } }
        [DataMember]
        public string pmAgZona { get { return AgZona; } set { AgZona = value; } }
        [DataMember]
        public string pmAgIdSZona { get { return AgIdSZona; } set { AgIdSZona = value; } }
        [DataMember]
        public string pmAgSubzona { get { return AgSubZona; } set { AgSubZona = value; } }
        [DataMember]
        public string pmAgIdRuta { get { return AgIdRuta; } set { AgIdRuta = value; } }
        [DataMember]
        public string pmAgRuta { get { return AgRuta; } set { AgRuta = value; } }

    }
}