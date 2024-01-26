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


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.


    [DataContract]
    public class RespProducto
    {
        Log _registro;
        [DataMember]
        public List<ProductosResponse> Productos { get; set; }

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
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
        PaginaAcceder paginaAcceder;
        ObtUsuario obtUsuario;
        [DataMember]
        public ObtUsuario Usuario
        {
            get { return obtUsuario; }
            set { obtUsuario = value; }
        }

        [DataMember]
        public string CodOrDesProd { get; set; }
        
        [DataMember]
        public string Grupo { get; set; }

        [DataMember]
        public bool SaldosCiaBod { get; set; }
        
        [DataMember]
        public string SubGrupo { get; set; }

        [DataMember]
        public PaginaAcceder pagina
        {
            get { return paginaAcceder; }
            set { paginaAcceder = value; }
        }


    }


    public class ObtCliente
    {
        ObtUsuario obtUsuario;
        [DataMember]
        public string NitCliente { get; set; }
        [DataMember]
        public ObtUsuario Usuario
        {
            get { return obtUsuario; }
            set { obtUsuario = value; }
        }

    }
    [DataContract]
    public class ObtUsuario
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }

    }
    [DataContract]
   public class RespCliente
    {
        Log _registro;
        [DataMember]
        public List<ClienteResponse> Clientes { get; set; }

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
        [DataMember]
        public List<UsuariosResponse> Usuarios { get; set; }

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
    }

}
