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
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ConsolidadoClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespCliente ObjCliente(ObtCliente obtCliente);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerUsuarios", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespUsuarios ObjUsuario(ObtUsuario obtUsuario);
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

    [DataContract]
    public class RespProducto
    {
        List<ProductosResponse> _producto;
        public List<ProductosResponse> Productos
        {
            get { return _producto; }
            set { _producto = value; }
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

    public class ObtProducto
    {

        PaginaAcceder paginaAcceder;
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

   public class RespCliente
    {

    }
    public class RespUsuario
    {

    }
    
}
