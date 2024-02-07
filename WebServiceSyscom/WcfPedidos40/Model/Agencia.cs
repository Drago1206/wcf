using System.Runtime.Serialization;

namespace WcfPedidos40.Model
{
    [DataContract]
    public class Agencia
    {
        [DataMember]
        public string IdAgencia { get; set; }
        [DataMember]
        public string NomAgencia { get; set; }
        [DataMember]
        public string DirAgncia { get; set; }
        [DataMember]
        public string AgIdLocal { get; set; }
        [DataMember]
        public string AgLocalidad { get; set; }
        [DataMember]
        public string TelAgncia { get; set; }
        [DataMember]
        public string NomCont { get; set; }
        [DataMember]
        public string emlCont { get; set; }
        [DataMember]
        public string AgIdVend { get; set; }
        [DataMember]
        public string AgVendedor { get; set; }
        [DataMember]
        public string AgCdCms { get; set; }
        [DataMember]
        public string DiasEntrega { get; set; }
        [DataMember]
        public decimal? AgTarCms { get; set; }
        [DataMember]
        public string AgCdDct { get; set; }
        [DataMember]
        public decimal? AgTarDct { get; set; }
        [DataMember]
        //Versión 0003
        public string AgIdZona { get; set; }
        [DataMember]
        public string AgZona { get; set; }
        [DataMember]
        public string AgIdSZona { get; set; }
        [DataMember]
        public string AgSubZona { get; set; }
        [DataMember]
        public string AgIdRuta { get; set; }
        [DataMember]
        public string AgRuta { get; set; }
    }
}