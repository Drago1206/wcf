using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

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
        public List<itemCia> mItemCia { get; set; }

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
        public List<itemCia> itemCia { get { return mItemCia; } set { mItemCia= value; } }




    }

    [DataContract]
    public class ProductoRequest
    {
        public string mCodOrDesprod { get; set; }
        public string mGrupo { get; set; }
        public int mPaginaActual { get; set; }
        public int mRegistrosPorPagina { get; set; }
        public bool mSaldosCiaBod { get; set; }
        public string mSubGrupo { get; set; }

        [DataMember]
        public string CodOrDesProd { get { return mCodOrDesprod; } set { mCodOrDesprod = value; } }
        [DataMember]
        public string Grupo { get { return mGrupo; } set { mGrupo = value; } }
        [DataMember]
        public int PaginaActual { get { return mPaginaActual; } set { mPaginaActual = value; } }
        [DataMember]
        public int RegistrosPorPagina { get { return mRegistrosPorPagina; } set { mRegistrosPorPagina = value; } }
        [DataMember]
        public bool SaldosCiaBod { get { return mSaldosCiaBod; } set { mSaldosCiaBod = value; } } 
        [DataMember]
        public string Subgrupo { get { return mSubGrupo; } set { mSubGrupo = value; } }
    }
}