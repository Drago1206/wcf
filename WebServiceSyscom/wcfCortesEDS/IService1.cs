using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using wcfCortesEDS.Model;

namespace wcfCortesEDS
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerToken", BodyStyle = WebMessageBodyStyle.Bare)]
        RespToken generarToken(Usuarios usuario);

        // TODO: agregue aquí sus operaciones de servicio
    }


    [DataContract]
    public class RespToken
    {
        Log _registro;
        GenerarToken _token;
        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }
        [DataMember]
        public GenerarToken Token
        {
            get { return _token; }
            set { _token = value; }
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


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.

}
