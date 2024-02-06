using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Models;
using WcfPruebas30.Models;

namespace WcfPedidos40
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface Pedidos40
    {
        /// <summary>
        /// Ruta del metodo para obtener cartera
        /// </summary>
        /// <param name="ReqCartera">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraResponse")]
        CarteraResp RespCartera(CarteraReq ReqCartera);

        // TODO: agregue aquí sus operaciones de servicio
    }


    [DataContract]
    public class CarteraReq
    {
        Usuario _usuario;
        /// <summary>
        /// Propiedad del nit cliente para que el usuario,
        /// logre accerder a la funcionalidad de los metodos
        /// </summary>

        [DataMember]
        public string NitCliente { get; set; }

        /// <summary>
        /// Propiedad del usuario para poder obtener el nombre de usuario y contraseña
        /// </summary>

        [DataMember]
        public Usuario usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
    }
    [DataContract]
    public class CarteraResp
    {
        Errores _error;
        List<ItemCartera> _DatosCartera;

        /// <summary>
        /// Propiedad para poder listar los datos de la clase cartera que a su vez
        /// tiene la lista de la clase cartera para almacenar los resultados.
        /// </summary>

        [DataMember]
        public List<ItemCartera> DatosCartera
        {
            get { return _DatosCartera; }
            set { _DatosCartera = value; }
        }
        /// <summary>
        /// Manejo de errores o estados de las solicitudes de los metodos
        /// </summary>

        [DataMember]
        public Errores Error
        {
            get { return _error; }
            set { _error = value; }
        }

    }

    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.

}
