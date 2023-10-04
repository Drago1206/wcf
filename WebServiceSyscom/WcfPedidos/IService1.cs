using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos.Model;


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
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarToken", BodyStyle = WebMessageBodyStyle.Bare)]
        RespToken generarToken(Usuarios usuario);

        /// <summary>
        /// Consultar un producto en especifico.
        /// </summary>
        /// <param name="obtProducto">The obt producto.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        RespProducto ConProducto(ObtProducto obtProducto);

        /// <summary>
        /// Consulta de productos y clasificados en lista.
        /// </summary>
        /// <param name="obtenerConFecha">The obtener con fecha.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        RespProductos GetProductos(ObtInfoGeneral obtenerConFecha);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Clientes")]
        RespClientes GetClientes(ObtInfoClientes obtenerConFecha);

        /*
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCarteraTotal", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraTotal")]
        ResObtenerCarteraTotal getCarteraTotal(Token token);
        */
        /*
        /// <summary>
        /// Consulta un cliente.
        /// </summary>
        /// <param name="obtenerConFecha">The obtener con fecha.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCliente", BodyStyle = WebMessageBodyStyle.Bare)]
        RespCliente GetCliente(ObtCliente obtCliente);
        */


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
        public string Descripcion
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }
    }

    [DataContract]
    public class RespProducto
    {
        Log _registro;
        string _codigo;
        string _descripcion;
        string _descripcorta;
        string _compania;
        string _bodega;
        decimal _saldo;
        decimal preciol1;
        decimal preciol2;
        decimal preciol3;
        decimal preciol4;
        decimal preciol5;
        
        [DataMember]
        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        [DataMember]
        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }

        [DataMember]
        public string DesCorta
        {
            get { return _descripcorta; }
            set { _descripcorta = value; }
        }

        [DataMember]
        public string Compania
        {
            get { return _compania; }
            set { _compania = value; }
        }

        [DataMember]
        public string Bodega
        {
            get { return _bodega; }
            set { _bodega = value; }
        }

        [DataMember]
        public decimal Saldo
        {
            get { return _saldo; }
            set { _saldo = value; }
        }
        [DataMember]
        public decimal PrecioL1
        {
            get { return preciol1; }
            set { preciol1 = value; }
        }
        [DataMember]
        public decimal PrecioL2
        {
            get { return preciol2; }
            set { preciol2 = value; }
        }
        [DataMember]
        public decimal PrecioL3
        {
            get { return preciol3; }
            set { preciol3 = value; }
        }
        [DataMember]
        public decimal PrecioL4
        {
            get { return preciol4; }
            set { preciol4 = value; }
        }
        [DataMember]
        public decimal PrecioL5
        {
            get { return preciol5; }
            set { preciol5 = value; }
        }

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }

    }

    [DataContract]
    public class ObtProducto
    {
        GenerarToken _token;    
        string _idcia;
        string _idbodega;
        int _numlistprecio;

        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }
        [DataMember]
        public string IdProducto { get; set; }

        [DataMember]
        public string IdCia
        {
            get { return _idcia; }
            set { _idcia = value; }
        }

        [DataMember]
        public string IdBodega
        {
            get { return _idbodega; }
            set { _idbodega = value; }
        }

        [DataMember]
        public int NumListaPrecio
        {
            get { return _numlistprecio; }
            set { _numlistprecio = value; }
        }
    }

    [DataContract]
    public class RespProductos
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
    public class ObtInfoGeneral
    {
        GenerarToken _token;

        [DataMember]
        public GenerarToken Token {
            get { return _token; }
            set { _token = value; }
        }

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
        public int NumResgitroPagina { get; set; }
        
    }

    [DataContract]
    public class ObtCliente
    {
        GenerarToken _token;
        [DataMember]
        public GenerarToken token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string IdUsuario
        {
            get; set;
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
        public string NitClinte { get; set; }

        [DataMember]
        public PaginaAcceder Pagina{
            get { return paginaAcceder; }
            set { paginaAcceder = value; }
            }
        

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
        public int NumResgitroPagina { get; set; }
    }

    /*
    [DataContract]
    public class ObtInfoCarteras
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
        public PaginaAcceder Pagina
        {
            get { return paginaAcceder; }
            set { paginaAcceder = value; }
        }

    }
    */
}
