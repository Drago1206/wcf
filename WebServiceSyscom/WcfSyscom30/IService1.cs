using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfSyscom30.Models;

namespace WcfSyscom30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {
        #region
        //[OperationContract]
        //string GetData(int value);

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: agregue aquí sus operaciones de servicio
        #endregion
        //ObtenerUsuario
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerUsuario", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Usuario")]
        ResDatosUsuario getUsuario(DtUsuario usuario);

        //ObtenerClientes
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Cliente")]
        ResObtenerClientes getClientes(DtCliente cliente);

        //ObtenerProductos
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Productos")]
        ResObtenerProductos getProductos(DtProducto producto);

        //GenerarPedido
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        ResGenerarPedido setPedido(DtPedido pedido);

        //ObtenerCartera
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Cartera")]
        ResObtenerCartera getCartera(DtCliente cliente);

        //Consolidado Clientes
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ConsolidadoClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "cliente")]
        ResObtenerConsClientes getConsClientes(DtClientes cliente);
    }

    [DataContract]
    public class DtUsuario
    {
        Usuario _usuario;

        [DataMember]
        public Usuario Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
    }

    [DataContract]
    public class ResDatosUsuario
    {
        DatosUsuario _DatosUsuario;
        Errores _error;

        [DataMember]
        public DatosUsuario DatosUsuarios
        {
            get { return _DatosUsuario; }
            set { _DatosUsuario = value; }
        }

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }

    [DataContract]
    public class DtCliente
    {
        Usuario _usuario;
        Cliente _cliente;

        [DataMember]
        public Usuario Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        [DataMember]
        public Cliente Cliente
        {
            get { return _cliente; }
            set { _cliente = value; }
        }
    }

    [DataContract]
    public class DtClientes
    {
        Usuario _usuario;
        Clientes _clientes;

        [DataMember]
        public Usuario Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        [DataMember]
        public Clientes Clientes
        {
            get { return _clientes; }
            set { _clientes = value; }
        }
    }

    [DataContract]
    public class ResObtenerClientes
    {
        List<DatosCliente> _DatosCliente;
        Errores _error;

        [DataMember]
        public List<DatosCliente> DatosClientes
        {
            get { return _DatosCliente; }
            set { _DatosCliente = value; }
        }

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }

    [DataContract]
    public class ResObtenerConsClientes
    {
        public PaginadorCliente<DatosCliente> _DatosCliente;
        Errores _error;

        [DataMember]
        public PaginadorCliente<DatosCliente> ListadoClientes
        {
            get { return _DatosCliente; }
            set { _DatosCliente = value; }
        }

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }

    [DataContract]
    public class DtProducto
    {
        Usuario _usuario;
        //List<Productos> _productos;
        //string _codOrDesProd;
        DatosProducto _productos;

        [DataMember]
        public Usuario Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        [DataMember]
        public DatosProducto DatosProducto
        {
            get { return _productos; }
            set { _productos = value; }
        }
    }

    [DataContract]
    public class ResObtenerProductos
    {
        PaginadorProducto<ItemProducto> _DatosProducto;
        Errores _error;

        [DataMember]
        public PaginadorProducto<ItemProducto> ListaProductos
        {
            get { return _DatosProducto; }
            set { _DatosProducto = value; }
        }

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }

    [DataContract]
    public class DtPedido
    {
        Usuario _usuario;
        Pedido _pedido;

        [DataMember]
        public Usuario Usuarios
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        [DataMember]
        public Pedido Pedido
        {
            get { return _pedido; }
            set { _pedido = value; }
        }
    }

    [DataContract]
    public class ResGenerarPedido
    {
        List<DatosPedido> _DatosPedido;
        Errores _error;

        [DataMember]
        public List<DatosPedido> DatosPedido
        {
            get { return _DatosPedido; }
            set { _DatosPedido = value; }
        }

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }

    [DataContract]
    public class ResObtenerCartera
    {
        List<DatosCartera> _DatosCartera;
        Errores _error;

        [DataMember]
        public List<DatosCartera> Datoscartera
        {
            get { return _DatosCartera; }
            set { _DatosCartera = value; }
        }

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
