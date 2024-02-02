using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using WcfPedidos30.Model;

namespace WcfPedidos30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IPedido30
    {
        /// <summary>
        /// Método que se encarga de procesar la solicitud de un producto.
        /// </summary>
        /// <param name="obtProducto">Objeto que contiene los detalles del producto a obtener.</param>
        /// <returns>Devuelve una respuesta que contiene los detalles del producto solicitado.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto ConProducto(ObtProducto obtProducto);
        /// <summary>
        /// Método que se encarga de procesar la solicitud del cliente
        /// </summary>
        /// <param name="obtCliente">Objeto que contiene los detalles del cliente a obtener.</param>
        /// <returns>Devuelve una respuesta que contiene los detalles del cliente solicitado.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Cliente")]
        RespCliente ObjCliente(ObtCliente obtCliente);
        /// <summary>
        /// Método que se encarga de procesar la solicitud del usuario
        /// </summary>
        /// <param name="obtUsuario">Objeto que contiene los detalles del usuario a obtener.</param>
        /// <returns>Devuelve una respuesta que contiene los detalles del usuario solicitado.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerUsuarios", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Usuario")]
        RespUsuario ObjUsuario(ObtUsuario obtUsuario);

        /// <summary>
        /// Método que se encarga de procesar la solicitud del pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns>Devuelve una respuesta que contiene los detalles del pedido solicitado.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido",BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        ResGenerarPedido setPedido(DtPedido pedido);
    }

    [DataContract]
    public class ResGenerarPedido
    {
        List<PedidoResponse> _DatosPedido;
        Log _error;
        /// <summary>
        /// Lista de respuesta de los pedidos
        /// </summary>
        [DataMember]
        public List<PedidoResponse> DatosPedido
        {
            get { return _DatosPedido; }
            set { _DatosPedido = value; }
        }
        /// <summary>
        /// Objeto Log para registrar los errores
        /// </summary>
        [DataMember]
        public Log Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }
    [DataContract]
    public class DtPedido
    {
        UsuariosRequest _usuario;
        PedidoRequest _pedido;
        /// <summary>
        /// Objeto que obtiene los datos de solicitud del usuario
        /// </summary>
        [DataMember]
        public UsuariosRequest Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        /// <summary>
        /// Objeto que obtiene los datos de solicitud del pedido
        /// </summary>
        [DataMember]
        public PedidoRequest Pedido
        {
            get { return _pedido; }
            set { _pedido = value; }
        }
    }
    [DataContract]
    public class RespProducto
    {
        Log _registro;
        PaginadorProducto<ProductosResponse> _DatosProducto;
        /// <summary>
        /// Objeto que contiene la respuesta de los productos
        /// </summary>
        [DataMember]
        public PaginadorProducto<ProductosResponse> ListaProductos
        {
            get { return _DatosProducto; }
            set { _DatosProducto = value; }
        }
        /// <summary>
        /// Objeto Log para registrar los errores
        /// </summary>
        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
    }

    [DataContract]
    public class OrganizadorPagina
    {
        /// <summary>
        /// Número total de páginas.
        /// </summary>
        [DataMember]
        public int NumeroDePaginas { get; set; }

        /// <summary>
        /// Número de la página actual.
        /// </summary>
        [DataMember]
        public int PaginaActual { get; set; }

        /// <summary>
        /// Número de registros por página.
        /// </summary>
        [DataMember]
        public int RegistroPorPagina { get; set; }

        /// <summary>
        /// Número total de registros.
        /// </summary>
        [DataMember]
        public int RegistroTotal { get; set; }
    }


    [DataContract]
    public class Log
    {
        string _fecha;
        Int32 _registros;
        string _codigo;
        string _mensaje;

        /// <summary>
        /// Fecha en la que se registró el log.
        /// </summary>
        [DataMember]
        public string Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }

        /// <summary>
        /// Código asociado al log.
        /// </summary>
        [DataMember]
        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        /// <summary>
        /// Descripción detallada del log.
        /// </summary>
        [DataMember]
        public string Descripcion
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }
    }


    [DataContract]
    public class ObtProducto
    {
        ProductoRequest _productoRequest;
        UsuariosRequest _usuarioRequest;

        /// <summary>
        /// Objeto que contiene la información del usuario.
        /// </summary>
        [DataMember]
        public UsuariosRequest Usuarios
        {
            get { return _usuarioRequest; }
            set { _usuarioRequest = value; }
        }

        /// <summary>
        /// Objeto que contiene la información del producto.
        /// </summary>
        [DataMember]
        public ProductoRequest DatosProducto
        {
            get { return _productoRequest; }
            set { _productoRequest = value; }
        }
    }



    public class ObtCliente
    {
        ObtUsuario obtUsuario;
        UsuariosRequest _usuario;
        ClienteRequest _cliente;
        /// <summary>
        /// Objeto que contiene la información del usuario.
        /// </summary>
        [DataMember]
        public UsuariosRequest Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        /// <summary>
        /// Objeto que contiene la información del cliente.
        /// </summary>
        [DataMember]
        public ClienteRequest Cliente
        {
            get { return _cliente; }
            set { _cliente = value; }
        }

    }
    [DataContract]
    public class ObtUsuario
    {
        UsuariosRequest _usuarioRequest;
        /// <summary>
        /// Objeto que contiene la información del usuario.
        /// </summary>
        [DataMember]
        public UsuariosRequest Usuarios
        {
            get { return _usuarioRequest; }
            set { _usuarioRequest = value; }
        }

    }
    [DataContract]
    public class RespCliente
    {
        Log _registro;
        List<ClienteResponse> _DatosCliente;
        /// <summary>
        /// Lista que contiene los datos del cliente
        /// </summary>
        [DataMember]
        public List<ClienteResponse> DatosClientes
        {
            get { return _DatosCliente; }
            set { _DatosCliente = value; }
        }
        /// <summary>
        /// Objeto Log para registrar los errores
        /// </summary>
        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
    }

    [DataContract]
    public class RespUsuario
    {
        Log _registro;

        UsuariosResponse _usuarioResponse;
        /// <summary>
        /// Objeto que contiene la información del usuario.
        /// </summary>
        [DataMember]
        public UsuariosResponse DatosUsuarios
        {
            get { return _usuarioResponse; }
            set { _usuarioResponse = value; }
        }
        /// <summary>
        /// Objeto Log para registrar los errores
        /// </summary>
        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
    }

}
