using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos40.Model;
namespace WcfPedidos40
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IPedidos40
    {

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Clientes")]
        RespClientes GetClientes(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto GetProducto(ProductoReq reqProducto);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductoTP", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto GetProductoTP(ProductoTPReq reqProductoTP);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Productos")]
        RespProductos GetProductos(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        PedidoResponse SetPedido(PedidoRequest pedido);
    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    [DataContract]
    public class ProductoReq
    {
        string _id;
        string _contrasena;
        string _idproducto;
        string _idcia;
        string _idbodega;
        int _numlistprecio;

        [DataMember]
        public string IdUsuario
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Contrasena
        {
            get { return _contrasena; }
            set { _contrasena = value; }
        }

        [DataMember]
        public string IdProducto
        {
            get { return _idproducto; }
            set { _idproducto = value; }
        }

        [DataMember]
        public string IdCia
        {
            get { return _idcia; }
            set { _idcia = value; }
        }

        [DataMember]
        public string IdBodega
        {
            get { return _idbodega; }
            set { _idbodega = value; }
        }

        [DataMember]
        public int NumListaPrecio
        {
            get { return _numlistprecio; }
            set { _numlistprecio = value; }
        }
    }

    [DataContract]
    public class RespProductos
    {
        Log _registro;
        string _productos;

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }

        [DataMember]
        public List<ProductosResponse> Productos { get; set; }
    }
    [DataContract]
    public class Usuario
    {
        string _id;
        string _fechaact;

        [DataMember]
        public string IdUsuario
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Contrasena { get; set; }

        [DataMember]
        public string Fecha_Act
        {
            get { return _fechaact; }
            set { _fechaact = value; }
        }
    }
    [DataContract]
    public class Log
    {
        string _fecha;
        Int32 _registros;
        string _codigo;
        string _mensaje;

        [DataMember]
        public string Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }

        [DataMember]
        public Int32 Registros
        {
            get { return _registros; }
            set { _registros = value; }
        }

        [DataMember]
        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        [DataMember]
        public string Msg
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }
    }
    [DataContract]
    public class RespClientes
    {
        Log _registro;
        List<ClienteResponse> _clientes;

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }

        [DataMember]
        public List<ClienteResponse> Clientes
        {
            get { return _clientes; }
            set { _clientes = value; }
        }
    }
    public class RespProducto
    {
        List<Errores> _errores;
        string _codigo;
        string _descripcion;
        string _descripcorta;
        string _compania;
        string _bodega;
        decimal _saldo;
        decimal preciol1;
        decimal preciol2;
        decimal preciol3;
        decimal preciol4;
        decimal preciol5;

        //Saldo
        //PrecioL1
        //PrecioL2
        //PrecioL3
        //PrecioL4
        //PrecioL5
        [DataMember]
        public List<Errores> Errores
        {
            get { return _errores; }
            set { _errores = value; }
        }

        [DataMember]
        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        [DataMember]
        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }

        [DataMember]
        public string DesCorta
        {
            get { return _descripcorta; }
            set { _descripcorta = value; }
        }

        [DataMember]
        public string Compania
        {
            get { return _compania; }
            set { _compania = value; }
        }

        [DataMember]
        public string Bodega
        {
            get { return _bodega; }
            set { _bodega = value; }
        }

        [DataMember]
        public decimal Saldo
        {
            get { return _saldo; }
            set { _saldo = value; }
        }
        [DataMember]
        public decimal PrecioL1
        {
            get { return preciol1; }
            set { preciol1 = value; }
        }
        [DataMember]
        public decimal PrecioL2
        {
            get { return preciol2; }
            set { preciol2 = value; }
        }
        [DataMember]
        public decimal PrecioL3
        {
            get { return preciol3; }
            set { preciol3 = value; }
        }
        [DataMember]
        public decimal PrecioL4
        {
            get { return preciol4; }
            set { preciol4 = value; }
        }
        [DataMember]
        public decimal PrecioL5
        {
            get { return preciol5; }
            set { preciol5 = value; }
        }
    }

    [DataContract]
    public class PedidoRequest
    {
        private Usuario _usuario;
        private List<ProductosResponse> _productos;
        private ClienteRequest _cliente;
        private string _tipopedido;
        private string _cdAgencia = "0";

        [DataMember]
        public Usuario Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        [DataMember]
        public ClienteRequest Cliente
        {
            get { return _cliente; }
            set { _cliente = value; }
        }

        [DataMember]
        public List<ProductosResponse> Productos
        {
            get { return _productos; }
            set { _productos = value; }
        }

        [DataMember]
        public string TipoPedido
        {
            get { return _tipopedido; }
            set { _tipopedido = value; }
        }

        [DataMember]
        public string Agencia
        {
            get { return _cdAgencia; }
            set { _cdAgencia = value; }
        }
    }
    [DataContract]
    public class PedidoResponse
    {
        [DataMember]
        public PedidoResp Pedido { get; set; }

        [DataMember]
        public List<string> Errores { get; set; }

    }
    public class ProductoTPReq
    {
        string _id;
        string _contrasena;
        string _idproducto;
        int _numlistprecio;

        [DataMember]
        public string IdUsuario
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Contrasena
        {
            get { return _contrasena; }
            set { _contrasena = value; }
        }

        [DataMember]
        public string IdProducto
        {
            get { return _idproducto; }
            set { _idproducto = value; }
        }

        [DataMember]
        public int NumListaPrecio
        {
            get { return _numlistprecio; }
            set { _numlistprecio = value; }
        }
    }
}
