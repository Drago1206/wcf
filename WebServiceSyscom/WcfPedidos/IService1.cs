using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos.Model;
using System.Data;


namespace WcfPedidos
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {
        /// <summary>
        /// generar token de seguridad.
        /// </summary>
        /// <param name="usuario">The usuario.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerToken", BodyStyle = WebMessageBodyStyle.Bare)]
        RespToken generarToken(Usuarios usuario);

        /// <summary>
        /// Consultar un producto en especifico.
        /// </summary>
        /// <param name="obtProducto">The obt producto.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto ConProducto(ObtProducto obtProducto);

        /// <summary>
        /// Metodo para consultar todos los clientes y uno en especial.
        /// </summary>
        /// <param name="obtenerConFecha">The obtener con fecha.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Clientes")]
        RespClientes GetClientes(ObtInfoClientes obtenerConFecha);

        /// <summary>
        /// Resources the generar pedido.
        /// </summary>
        /// <param name="pedido">The pedido.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        ResGenerarPedido resGenerarPedido(GenPedido pedido);

        /// <summary>
        /// Resources the information maestra.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerInfMaestra", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "InfMaestra")]
        ResInfoMaestra resInfoMaestra(InfoMaestra Info);

        /// <summary>
        /// Resources the obt cart total.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCarteraTotal", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraTotal")]
        ResObtenerCarteraTotal resObtCartTotal(ObtCarTotal Info);

        /// <summary>
        /// Resources the obt cart total definition.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCarteraTotalDef", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraDef")]
        ResObtenerCarteraTotalDef resObtCartTotalDef(ObtCarTotalDef Info);

        /// <summary>
        /// Rests the anul pedido.
        /// </summary>
        /// <param name="Info">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AnularPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "AnularPedido")]
        RestAnulPedido restAnulPedido(AnulPedido Info);


    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    

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
    public class ObtProducto
    {
        GenerarToken _token;
        PaginaAcceder paginaAcceder;

        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string IdProducto { get; set; }

        [DataMember]
        public string IdCia { get; set; }

        [DataMember]
        public string IdBodega { get; set; }

        [DataMember]
        public PaginaAcceder pagina
        {
            get { return paginaAcceder; }
            set { paginaAcceder = value; }
        }

    }

    
    [DataContract]
    public class RespProducto
    {
        OrganizadorPagina _organizadorPagina;
        Log _registro;

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }

        [DataMember]
        public List<Producto> Productos { get; set; }

        [DataMember]
        public OrganizadorPagina paginas
        {
            get { return _organizadorPagina; }
            set { _organizadorPagina = value; }
        }
       
        
    }

   [DataContract]
   public class GenPedido
    {
        GenerarToken _token;
        ClientePedido clientePedido;
        DatosPedido datosPedido;
        infContactoCliente infContacto;

        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public ClientePedido Cliente
        {
            get { return clientePedido; }
            set { clientePedido = value; }
        }
        [DataMember]
        public DatosPedido DatosDelPedido
        {
            get { return datosPedido; }
            set { datosPedido = value; }
        }
        [DataMember]
        public infContactoCliente InformacionDeContacto
        {
            get { return infContacto; }
            set { infContacto = value; }
        }
        [DataMember]
        public List<ProductosPed> Productos { get; set; }

    }

    [DataContract]
    public class ClientePedido
    {
        [DataMember]
        public string TipoDoc { get; set; }

        [DataMember]
        public string Documento { get; set; }

        [DataMember]
        public string DigitoDeVerificacion { get; set; }

        [DataMember]
        public string CdAgencia { get; set; }

        //[DataMember]
        //public string Nombre { get; set; }

        //[DataMember]
        //public string Apellidos { get; set; }

        [DataMember]
        public string RazonSocial { get; set; }

        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string Municipio { get; set; }

        [DataMember]
        public string Telefono { get; set; }

        [DataMember]
        public string GrupoCliente { get; set; }

        [DataMember]
        public string ActEconomica { get; set; }

        [DataMember]
        public string RegimnDian { get; set; }

        [DataMember]
        public string Profesion { get; set; }

        [DataMember]
        public string Zona { get; set; }

        [DataMember]
        public string SubZona { get; set; }

        [DataMember]
        public string Ruta { get; set; }

        [DataMember]
        public string Plazo { get; set; }

        [DataMember]
        public string IdVendedor { get; set; }

        [DataMember]
        public string MunExpedicion { get; set; }

        [DataMember]
        public int DiasEntrega { get; set; }

        [DataMember]
        public string CentroCosto { get; set; }

        [DataMember]
        public string SubCCosto { get; set; }

       

    }

    [DataContract]
    public class DatosPedido
    {
        [DataMember]
        public string pmFechaPedido { get; set; }

        [DataMember]
        public string pmFormaPago { get; set; }
        [DataMember]
        public int pmDiasPlazo { get; set; }
        [DataMember]
        public string pmIdVendedor { get; set; }
        [DataMember]
        public string pmIdCompania { get; set; }
        [DataMember]
        public string pmIdBodega { get; set; }
        [DataMember]
        public string pmTarifaComision { get; set; }
        [DataMember]
        public string pmEstadoPedido { get; set; }
    }

    [DataContract]
    public class infContactoCliente
    {
        [DataMember]
        public string NitContacto { get; set; }
        [DataMember]
        public string NombreContacto { get; set; }
        [DataMember]
        public string TelefonoContacto { get; set; }
        [DataMember]
        public string EmailContacto { get; set; }
        [DataMember]
        public string CargoContacto { get; set; }
    }

    [DataContract]
    public class ProductosPed
    {
        [DataMember]
        public string pmIdProducto { get; set; }
        [DataMember]
        public string pmIdTanque { get; set; }
        [DataMember]
        public int pmCantidad { get; set; }
        [DataMember]
        public decimal pmVrPrecio { get; set; }
        [DataMember]
        public int pmCantObsequio { get; set; }
        [DataMember]
        public string pmIdTarDcto { get; set; }
        [DataMember]
        public int pmIdListaDePrecio { get; set; }
       
    }

    
    [DataContract]
    public class ResGenerarPedido
    {
        Log log;

        [DataMember]
        public string TipoDoc{ get; set; }

        [DataMember]
        public string IdCia { get; set; }

        [DataMember]
        public string CdAgencia { get; set; }

        [DataMember]
        public string Fecha { get; set; }

        [DataMember]
        public Log Registro
        {
            get { return log; }
            set { log = value; }
        }
    }

    [DataContract]
    public class InfoMaestra
    {
        GenerarToken _token;
  
        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }
        [DataMember]
        public int TipoRegistro { get; set; }

    }

    [DataContract]
    public class ResInfoMaestra
    {
        Log log;
        
        [DataMember]
        public Log Registro
        {
            get { return log; }
            set { log = value; }
        }

        [DataMember]
        public List<Dictionary<string, object>> tableList
        { get; set; }


    }

    [DataContract]
    public class ObtCarTotal
    {
        GenerarToken _token;

        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string IdCliente { get; set; }
    }


    [DataContract]
    public class ResObtenerCarteraTotal
    {
        Log log;

        [DataMember]
        public Log Registro
        {
            get { return log; }
            set { log = value; }
        }

        [DataMember]
        public List<ResCarteraTotal> DatosCartera { get; set; }

    }

    public class ResCarteraTotal
    {
        public int SaldoCartera { get; set; }
        public string Cliente { get; set; }
    }


    [DataContract]
    public class ObtCarTotalDef
    {
        GenerarToken _token;
        PaginaAcceder paginaAcceder;

        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string IdCliente { get; set; }

        [DataMember]
        public string FechaInicial { get; set; }

        [DataMember]
        public string FechaFinal { get; set; }

        [DataMember]
        public PaginaAcceder pagina
        {
            get { return paginaAcceder; }
            set { paginaAcceder = value; }
        }
    }


    public class itemCartera
    {
        public string TipoDocumento { get; set; }
        public int Documento { get; set; }
        public string Compañia { get; set; }
        public int Vencimiento { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int ValorTotal { get; set; }
        public int Abono { get; set; }
        public int Saldo { get; set; }
    }


    [DataContract]
    public class ResObtenerCarteraTotalDef
    {
        Log log;
        OrganizadorPagina _organizadorPagina;

        [DataMember]
        public Log Registro
        {
            get { return log; }
            set { log = value; }
        }

        [DataMember]
        public List<itemCartera> DatosCartera { get; set; }

        [DataMember]
        public OrganizadorPagina paginas
        {
            get { return _organizadorPagina; }
            set { _organizadorPagina = value; }
        }

    }

    [DataContract]
    public class AnulPedido
    {
        GenerarToken _token;
       

        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string  IdPedido { get; set; }

        [DataMember]
        public string IdCia { get; set; }
    }

    [DataContract]
    public class RestAnulPedido
    {
        Log log;
       
        [DataMember]
        public Log Registro
        {
            get { return log; }
            set { log = value; }
        }
    }

    [DataContract]
    public class RespClientes
    {
        Log _registro;
        List<ClienteResponse> _clientes;
        OrganizadorPagina organizadorPagina;

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

        [DataMember]
        public OrganizadorPagina paginas
        {
            get { return organizadorPagina; }
            set { organizadorPagina = value; }
        }
    }

    [DataContract]
    public class ObtInfoClientes
    {
        GenerarToken _token;
        PaginaAcceder paginaAcceder;

        [DataMember]
        public GenerarToken Token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string NitCliente { get; set; }

        [DataMember]
        public PaginaAcceder Pagina{
            get { return paginaAcceder; }
            set { paginaAcceder = value; }
            }
        

    }
    
}
