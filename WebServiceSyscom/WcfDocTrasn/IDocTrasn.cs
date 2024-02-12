using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfDocTrasn.Model;

namespace WcfDocTrasn
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IDocTrasn
    {
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarToken", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Token")]
        RespuestaWS GetToken(User User);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        RespuestaPedido SetPedido(DtPedido DtPedido);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarTrazabilidad", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Trazabilidad")]
        RespuestaTrazabilidad GetTrazabilida(DtTrazabilidad DtTrazabilidad);


    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.

    [DataContract]
    public class User
    {
        Usuarios _user;
        [DataMember]
        [Required]
        public Usuarios Usuario
        {
            get { return _user; }
            set { _user = value; }
        }
    }


    [DataContract]
    public class RespuestaWS
    {
        Errores _error;
        Usuario _resp;
        [DataMember]
        public Errores Errores
        {
            get { return _error; }
            set { _error = value; }
        }

        [DataMember]
        public Usuario Respuesta
        {
            get { return _resp; }
            set { _resp = value; }
        }
    }


    public class RespuestaPedido
    {
        Errores _error;
        Pedidos _resp;
        [DataMember]
        public Errores Errores
        {
            get { return _error; }
            set { _error = value; }
        }

        [DataMember]
        public Pedidos Respuesta
        {
            get { return _resp; }
            set { _resp = value; }
        }
    }
    public class DtPedido
    {
        Pedidos _DtEncabezado;
        List<Items> _DtDetalle;
        List<Conceptos> _conceptos;
        string _token;

        [DataMember]
        [Required]
        public Pedidos Encabezado
        {
            get { return _DtEncabezado; }
            set { _DtEncabezado = value; }
        }

        public List<Items> Detalle
        {
            get { return _DtDetalle; }
            set { _DtDetalle = value; }
        }

        public List<Conceptos> Conceptos
        {
            get { return _conceptos; }
            set { _conceptos = value; }
        }

        [DataMember]
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
    }
    public class RespuestaTrazabilidad
    {
        Errores _error;
        List<DatosTrazabilidad> _resp;
        [DataMember]
        public Errores Errores
        {
            get { return _error; }
            set { _error = value; }
        }

        [DataMember]
        public List<DatosTrazabilidad> Respuesta
        {
            get { return _resp; }
            set { _resp = value; }
        }
    }
    public class DtTrazabilidad
    {
        string _token;
        int _NumPedido;
        string _CiaPedido;

        [DataMember]
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public int NumPedido
        {
            get { return _NumPedido; }
            set { _NumPedido = value; }
        }

        [DataMember]
        public string CiaPedido
        {
            get { return _CiaPedido; }
            set { _CiaPedido = value; }
        }
    }
    [DataContract]

    public class Usuario {
        string _Token;
        [DataMember]
        public string Token {
            get { return _Token; }
            set { _Token = value; }
        }
    }
}
