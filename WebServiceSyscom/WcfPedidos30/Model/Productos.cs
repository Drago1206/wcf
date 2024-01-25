using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Model
{
    [DataContract]
    public class ProductosResponse
    {
        public string mCodProducto { get; set; }
        public string mCodGru { get; set; }
        public string mCodSub { get; set; }
        public string mDescripcion { get; set; }
        public int mDescuento { get; set; }
        public string mFechaCreacion { get; set; }
        public int mImpuesto { get; set; }
        public int mLista1 { get; set; }
        public int mLista2 { get; set; }
        public int mLista3 { get; set; }
        public string mNombreGru { get; set; }
        public string mNombreSub { get; set; }
        public int mSaldoTotal { get; set; }
        public string mCodBodega { get; set; }
        public string mCodCia { get; set; }
        public int mSaldoBodega { get; set; }
        public int mSaldoCia { get; set; }
        public int mTotalPaginas { get; set; }
        public int mTotalRegistros { get; set; }

        [DataMember]
        public string CodProducto { get { return mCodProducto; } set { mCodProducto = value; } }
        [DataMember]
        public string CodGru { get { return mCodGru; } set { mCodGru = value; } }
        [DataMember]
        public string CodSub { get { return mCodSub; } set { mCodSub = value; } }
        [DataMember]
        public string Descripcion { get { return mDescripcion; } set { mDescripcion = value; } }
        [DataMember]
        public int Descuento { get { return mDescuento; } set { mDescuento = value; } }
        [DataMember]
        public string FechaCreacion { get { return mFechaCreacion; } set { mFechaCreacion = value; } } 
        [DataMember]
        public int Impuesto { get { return mImpuesto; } set { mImpuesto = value; } }
        [DataMember]
        public int Lista1 { get { return mLista1; } set { mLista1 = value; } }
        [DataMember]
        public int Lista2 { get { return mLista2; } set { mLista2 = value; } }
        [DataMember]
        public int Lista3 { get { return mLista3; } set { mLista3 = value; } }
        [DataMember]
        public string NombreGru { get { return mNombreGru; } set { mNombreGru = value; } }
        [DataMember]
        public string NombreSub { get { return mNombreSub; } set { mNombreSub = value; } }
        [DataMember]
        public int SaldoTotal { get { return mSaldoTotal; } set { mSaldoTotal = value; } }
        [DataMember]
        public string CodBodega { get { return mCodBodega; } set { mCodBodega = value; } }
        [DataMember]
        public string CodCia { get { return mCodCia; } set { mCodCia = value; } }
        [DataMember]
        public int SaldoBodega { get { return mSaldoTotal; } set { mSaldoTotal = value; } }
        [DataMember]
        public int SaldoCia { get { return mSaldoTotal; } set { mSaldoTotal = value; } }
        [DataMember]
        public int TotalPaginas { get { return mSaldoTotal; } set { mSaldoTotal = value; } }
        [DataMember]
        public int TotalRegistros { get { return mSaldoTotal; } set { mSaldoTotal = value; } }




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