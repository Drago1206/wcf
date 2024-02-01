using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Model;

namespace WcfPedidos30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IPedido30
    {

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto ConProducto(ObtProducto obtProducto);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Cliente")]
        RespCliente ObjCliente(ObtCliente obtCliente);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerUsuarios", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Usuario")]
        RespUsuario ObjUsuario(ObtUsuario obtUsuario);
    }


    [DataContract]
    public class RespProducto
    {
        OrganizadorPagina _organizadorPagina;
        Log _registro;
        List<ProductosResponse> _DatosProducto;

        [DataMember]
        public List<ProductosResponse> ListaProductos
        {
            get { return _DatosProducto; }
            set { _DatosProducto = value; }
        }
        /*
            [DataMember]
            public List<ProductosResponse> ListaProductos { get; set; }
        */
        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
        /*
            [DataMember]
            public OrganizadorPagina paginas
            {
                get { return _organizadorPagina; }
                set { _organizadorPagina = value; }
            }
        */
    }
    [DataContract]
    public class PaginaAcceder
    {
        [DataMember]
        public int Pagina { get; set; }

        [DataMember]
        public int RegistroPorPagina { get; set; }
    }
    [DataContract]
    public class OrganizadorPagina
    {
        [DataMember]
        public int NumeroDePaginas { get; set; }

        [DataMember]
        public int PaginaActual { get; set; }

        [DataMember]
        public int RegistroPorPagina { get; set; }

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

        [DataMember]
        public string Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
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
            get { return _mensaje; }
            set { _mensaje = value; }
        }
    }

    [DataContract]
    public class ObtProducto
    {
        ProductoRequest _productoRequest;
        UsuariosRequest _usuarioRequest;

        [DataMember]
        public UsuariosRequest Usuarios
        {
            get { return _usuarioRequest; }
            set { _usuarioRequest = value; }
        }
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

        [DataMember]
        public UsuariosRequest Usuarios
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

    }
    [DataContract]
    public class ObtUsuario
    {
        UsuariosRequest _usuarioRequest;

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

        [DataMember]
        public List<ClienteResponse> DatosClientes
        {
            get { return _DatosCliente; }
            set { _DatosCliente = value; }
        }
        /*
        [DataMember]
        public List<ClienteResponse> Clientes { get; set; }
        */
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

        [DataMember]
        public UsuariosResponse DatosUsuarios
        {
            get { return _usuarioResponse; }
            set { _usuarioResponse = value; }
        }

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
    }

}
