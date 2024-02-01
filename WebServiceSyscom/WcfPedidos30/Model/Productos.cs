using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{
    public class itemCia
    {
        public string CodCia { get; set; }
        public int Saldocia { get; set; }
        public string CodBodega { get; set; }
        public int Saldobodega { get; set; }
    }
    [DataContract]
    public class ProductosResponse
    {
        [DataMember]
        public string CodProducto { get; set; }
        [DataMember]
        public string CodGru { get; set; }
        [DataMember]
        public string CodSub { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public int Descuento { get; set; }
        [DataMember]
        public int Impuesto { get; set; }
        [DataMember]
        public int Lista1 { get; set; }
        [DataMember]
        public int Lista2 { get; set; }
        [DataMember]
        public int Lista3 { get; set; }
        [DataMember]
        public string NombreGru { get; set; }
        [DataMember]
        public string NombreSub { get; set; }
        [DataMember]
        public int SaldoTotal { get; set; }
        [DataMember]
        public List<itemCia> ItemCia { get; set; }
        [DataMember]
        public DateTime FechaCreacion { get; set; }


    }

    [DataContract]
    public class ProductoRequest
    {
        public string CodOrDesProd { get; set; }
        public string Grupo { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public bool SaldosCiaBod { get; set; }
        public string SubGrupo { get; set; }
    }
}