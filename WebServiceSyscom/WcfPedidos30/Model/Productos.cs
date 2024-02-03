using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{
    public class itemCia
    {
        /// <summary>
        /// Codigo de compañía
        /// </summary>
        public string CodCia { get; set; }
        /// <summary>
        /// Saldo de la compañía
        /// </summary>
        public int Saldocia { get; set; }
        /// <summary>
        /// Codigo de bodega
        /// </summary>
        public string CodBodega { get; set; }
        /// <summary>
        /// Saldo de la bodega
        /// </summary>
        public int Saldobodega { get; set; }
    }
    [DataContract]
    public class ProductosResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        [DataMember]
        public string CodProducto { get; set; }
        /// <summary>
        /// Codigo del grupo al que pertenece el producto
        /// </summary>
        [DataMember]
        public string CodigoGru { get; set; }
        /// <summary>
        /// Codigo del subgrupo al que pertenece el producto
        /// </summary>
        [DataMember]
        public string CodigoSub { get; set; }
        /// <summary>
        /// Descripción del producto
        /// </summary>
        [DataMember]
        public string Descripción { get; set; }
        /// <summary>
        /// Porcentaje de descuento configurado en el cliente o el grupo del producto
        /// </summary>
        [DataMember]
        public int Descuento { get; set; }
        /// <summary>
        /// Porcentaje de IVA configurado en el grupo
        /// </summary>
        [DataMember]
        public int Impuesto { get; set; }
        /// <summary>
        /// Valor de la lista de precios 1. Se debe calcular de acuerdo con el tipo de precio base
        /// </summary>
        [DataMember]
        public int Lista1 { get; set; }
        /// <summary>
        /// Valor de la lista de precios 2. Se debe calcular de acuerdo con el tipo de precio base
        /// </summary>
        [DataMember]
        public int Lista2 { get; set; }
        /// <summary>
        /// Valor de la lista de precios 1. Se debe calcular de acuerdo con el tipo de precio base
        /// </summary>
        [DataMember]
        public int Lista3 { get; set; }
        /// <summary>
        /// Nombre del grupo
        /// </summary>
        [DataMember]
        public string NombreGru { get; set; }
        /// <summary>
        /// Nombre del subgrupo
        /// </summary>
        [DataMember]
        public string NombreSub { get; set; }
        /// <summary>
        /// Total de productos en el inventario
        /// </summary>
        [DataMember]
        public int SaldoTotal { get; set; }
        /// <summary>
        /// Items de la compañía
        /// </summary>
        [DataMember]
        public List<itemCia> ItemCia { get; set; }
        /// <summary>
        /// Fecha de la creación del producto
        /// </summary>
        [DataMember]
        public DateTime FechaCreacion { get; set; }


    }

    [DataContract]
    public class ProductoRequest
    {
        /// <summary>
        /// Código del producto creado en Syscom.
        /// Si el campo está vacío devuelve todos los productos existentes.
        /// </summary>
        [DataMember]
        public string CodOrDesProd { get; set; }
        /// <summary>
        /// Grupo al que pertenece el producto.
        /// </summary>
        [DataMember]
        public string Grupo { get; set; }
        /// <summary>
        /// Carga los productos de la página que se indica.
        /// </summary>
        [DataMember]
        public int PaginaActual { get; set; }
        /// <summary>
        /// Cantidad de registro que quiere que se muestre por página.
        /// </summary>
        [DataMember]
        public int RegistrosPorPagina { get; set; }
        /// <summary>
        /// Campo para que se muestren los saldos de bodega.
        /// </summary>
        [DataMember]
        public bool SaldosCiaBod { get; set; }
        /// <summary>
        /// Subgrupo al que pertenece el producto.
        /// </summary>
        [DataMember]
        public string SubGrupo { get; set; }
    }
}