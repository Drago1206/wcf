using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using wcfSyscom.Model;

namespace wcfSyscom
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IServicio
    {

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Clientes")]
        RespClientes GetClientes(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Productos")]
        RespProductos GetProductos(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        PedidoResponse SetPedido(PedidoRequest pedido);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerInfMaestra", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Maestro")]
        ResInfoMaestra GetInfMaestra(InfoMaestra Parametros);
    }

    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
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
        public string Productos
        {
            get { return _productos; }
            set { _productos = value; }
        }        
    }

    [DataContract]
    public class PedidoRequest
    {
        private Usuario _usuario;
        private List<Producto> _productos;
        private ClienteRequest _cliente;
        private string _tipopedido;
        private string _cdAgencia="0";

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
        public List<Producto> Productos
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
        public string cdAgencia
        {
            get { return _cdAgencia; }
            set { _cdAgencia = value; }
        }

    }

    [DataContract]
    public class PedidoResponse
    {
        string _pedido;
//        string _productos;
        string _errores;

        [DataMember]
        public dynamic Pedido
        {
            get { return _pedido; }
            set { _pedido = value; }
        }

/*        [DataMember]
        public dynamic Productos
        {
            get { return _productos; }
            set { _productos = value; }
        }
*/
        [DataMember]
        public string Errores
        {
            get { return _errores; }
            set { _errores = value; }
        }

    }

    [DataContract]
    public class InfoMaestra
    {
        private Usuario _Usuario;
        private Int32 _TipoRegistro;

        [DataMember]
        public Usuario Usuario { get { return _Usuario; } set { _Usuario = value; } }
        [DataMember]
        public Int32 TipoRegistro { get { return _TipoRegistro; } set { _TipoRegistro = value; } }
    }

    [DataContract]
    public class ResInfoMaestra
    {
        private string _respuesta;

        [DataMember]
        public string Respuesta { get { return _respuesta; } set { _respuesta = value; } }
    }

}
