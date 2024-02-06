using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Models;
using WcfPruebas30.Models;
using static WcfPruebas30.CarteraReq;

namespace WcfPruebas30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        /// <summary>
        /// Ruta del metodo para obtener la consolidacion del cliente.
        /// </summary>
        /// <param name="obtenerConSolidado">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerConsolidadoClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "ConsolidacionC")]
        RespClientes resClients(ObtInfoClientes obtenerConSolidado);

        /// <summary>
        /// Ruta del metodo para obtener cartera
        /// </summary>
        /// <param name="ReqCartera">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraResponse")]
        CarteraResp RespCartera(CarteraReq ReqCartera);



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
    public class PaginaAcceder
    {
        /// <summary>
        /// Pagina que desea acceder.
        /// </summary>
        /// <value>
        /// La pagina.
        /// </value>
        [DataMember]
        public int Pagina { get; set; }

        /// <summary>
        /// Numero de registro por pagina.
        /// </summary>
        /// <value>
        /// Numero de registro por pagina.
        /// </value>
        [DataMember]
        public int NumRegistroPagina { get; set; }
    }




    [DataContract]
    public class RespClientes
    {
        Errores _errores;
        PaginadorCliente<ClienteResponse> _clientes;

        /// <summary>
        /// Manejo de errores o procedimientos de las funciones
        /// </summary>
       
        [DataMember]
        public Errores Error
        {
            get { return _errores; }
            set { _errores = value; }
        }
        /// <summary>
        /// Paginador del cliente para acceder a la informacion 
        /// de la clase debida.
        /// </summary>
        [DataMember]
        public PaginadorCliente<ClienteResponse> ListadoClientes
        {
            get { return _clientes; }
            set { _clientes = value; }
        }

    }

    [DataContract]
    public class ObtInfoClientes
    {
        /// <summary>
        /// Propiedad del nit cliente para que el usuario,
        /// logre accerder a la funcionalidad de los metodos
        /// </summary>
        UsuariosRequest _usuario;

        [DataMember]
        public string NitCliente { get; set; }

        /// <summary>
        /// Propiedad del usuario para poder obtener el nombre de usuario y contraseña
        /// </summary>
        [DataMember]
        public UsuariosRequest usuario
        {

            get { return _usuario; }
            set { _usuario = value; }
        }



    }
    [DataContract]
    public class ObtUsuario
    {
        Usuario _usuarioRequest;

        [DataMember]
        public Usuario Usuarios
        {
            get { return _usuarioRequest; }
            set { _usuarioRequest = value; }
        }

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

    //[DataContract]
    //public class ObtCarteraTotal
    //{
    //    public Usuario _usuario;


    //    [DataMember]
    //    public string NitCliente { get; set; }

    //    [DataMember]
    //    public Usuario usuario
    //    {
    //        get { return _usuario; }
    //        set { _usuario = value; }
    //    }
    //}
    //[DataContract]
    //public class CarteraRespTotal
    //{
    //    Errores _error;

    //    [DataMember]
    //    public Cartera cartera
    //    {
    //        get { return cartera; }
    //        set { cartera = value; }
    //    }

    //    [DataMember]
    //    public Errores Error
    //    {
    //        get { return _error; }
    //        set { _error = value; }
    //    }
    //}
    //[DataContract]
    //public class ResDatosUsuario
    //{
    //    Usuario _usuario;
    //    Errores _error;

    //    [DataMember]
    //    public Usuario DatosUsuarios
    //    {
    //        get { return _usuario; }
    //        set { _usuario = value; }
    //    }

    //    [DataMember]
    //    public Errores Error
    //    {
    //        get { return _error; }
    //        set { _error = value; }
    //    }

    //}

}
