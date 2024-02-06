using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    [DataContract]
    public class ProductosResponse
    {
        public string CodProducto { get; set; }
        public string CodGru { get; set; }
        public string CodSub { get; set; }
        public string Descripcion { get; set; }
        public int Descuento { get; set; }
        public string FechaCreacion { get; set; }
        public int Impuesto { get; set; }
        public int Lista1 { get; set; }
        public int Lista2 { get; set; }
        public int Lista3 { get; set; }
        public string NombreGru { get; set; }
        public string NombreSub { get; set; }
        public int SaldoTotal { get; set; }
        public string CodBodega { get; set; }
        public string CodCia { get; set; }
        public int SaldoBodega { get; set; }
        public int SaldoCia { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        [DataMember]
        public string pmCodProducto { get { return CodProducto; } set { CodProducto = value; } }
        [DataMember]
        public string pmCodGru { get { return CodGru; } set { CodGru = value; } }
        [DataMember]
        public string pmCodSub { get { return CodSub; } set { CodSub = value; } }
        [DataMember]
        public string pmDescripcion { get { return Descripcion; } set { Descripcion = value; } }
        [DataMember]
        public int pmDescuento { get { return Descuento; } set { Descuento = value; } }
        [DataMember]
        public string pmFechaCreacion { get { return FechaCreacion; } set { FechaCreacion = value; } }
        [DataMember]
        public int pmImpuesto { get { return Impuesto; } set { Impuesto = value; } }
        [DataMember]
        public int pmLista1 { get { return Lista1; } set { Lista1 = value; } }
        [DataMember]
        public int pmLista2 { get { return Lista2; } set { Lista2 = value; } }
        [DataMember]
        public int pmLista3 { get { return Lista3; } set { Lista3 = value; } }
        [DataMember]
        public string pmNombreGru { get { return NombreGru; } set { NombreGru = value; } }
        [DataMember]
        public string pmNombreSub { get { return NombreSub; } set { NombreSub = value; } }
        [DataMember]
        public int pmSaldoTota { get { return SaldoTotal; } set { SaldoTotal = value; } }
        [DataMember]
        public string pmCodBodega { get { return CodBodega; } set { CodBodega = value; } }
        [DataMember]
        public string pmCodCia { get { return CodCia; } set { CodCia = value; } }
        [DataMember]
        public int pmSaldoBodega { get { return SaldoTotal; } set { SaldoTotal = value; } }
        [DataMember]
        public int pmSaldoCia { get { return SaldoTotal; } set { SaldoTotal = value; } }
        [DataMember]
        public int pmTotalPaginas { get { return SaldoTotal; } set { SaldoTotal = value; } }
        [DataMember]
        public int pmTotalRegistros { get { return SaldoTotal; } set { SaldoTotal = value; } }




    }

    [DataContract]
    public class ProductoRequest
    {
        public string CodOrDesprod { get; set; }
        public string Grupo { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public bool SaldosCiaBod { get; set; }
        public string SubGrupo { get; set; }

        [DataMember]
        public string pmCodOrDesprod { get { return CodOrDesprod; } set { CodOrDesprod = value; } }
        [DataMember]
        public string pmGrupo { get { return Grupo; } set { Grupo = value; } }
        [DataMember]
        public int pmPaginaActual { get { return PaginaActual; } set { PaginaActual = value; } }
        [DataMember]
        public int pmRegistrosPorPagina { get { return RegistrosPorPagina; } set { RegistrosPorPagina = value; } }
        [DataMember]
        public bool pmSaldosCidaBod { get { return SaldosCiaBod; } set { SaldosCiaBod = value; } }
        [DataMember]
        public string pmSubGrupo { get { return SubGrupo; } set { SubGrupo = value; } }
    }
}