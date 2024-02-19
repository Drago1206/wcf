using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos40.Model;
using WcfPedidos40.Models;

namespace WcfPedidos40
{
   
    [ServiceContract]
    public interface IPedidos40
    {

        /// <summary>
        /// Ruta del metodo para obtener la cartera del cliente
        /// </summary>
        /// <param name="obtenerCartera">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraResponse")]
        CarteraResp RespCartera(CarteraReq ReqCartera);

        /// <summary>
        /// Ruta del metodo para obtener consultar el saldo total en deuda de todos los terceros al tiempo.
        /// </summary>
        /// <param name="obtenerCarteraTotal">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCarteraTotal", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraTotal")]
        ResObtenerCarteraTotal resObtCartTotal(ObtCarTotal Info);

        /// <summary>
        /// Ruta del metodo para obtener consultar el por un rango de fechas el saldo total en deuda de todos los clientes al mismo tiempo, 
        /// </summary>
        /// <param name="obtenerCarteraTotalDef">The information.</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCarteraTotalDet", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "CarteraTotalDet")]
        ResObtenerCartera getCarteraTotalDet(authCartera Modelo);

        /// <summary>
        ///Ruta del metodo para obtener la información necesaria para diligenciar la información de un cliente nuevo. 
        /// </summary>
        /// <param name="obtenerInformacionMaestra">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerInfMaestra", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Maestro")]
        ResInfoMaestra GetInfMaestra(InfoMaestra Parametros);

        /// <summary>
        ///Ruta del metodo para Obtener un producto
        /// </summary>
        /// <param name="obtenerProducto">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProducto", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto GetProducto(ProductoReq reqProducto);

        /// <summary>
        ///Ruta del metodo para consultar precio de lista fijo, también precio de venta calculado por margen de utilidad, de una sola compañía
        /// </summary>
        /// <param name="obtenerProducto">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductoTP", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Producto")]
        RespProducto GetProductoTP(ProductoTPReq reqProductoTP);
        /// <summary>
        ///Ruta del metodo para Obtener un productos
        /// </summary>
        /// <param name="obtenerProducto">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Productos")]
        RespProductos GetProductos(Usuario usuario);
        /// <summary>
        ///Ruta del metodo para generar un pedido
        /// </summary>
        /// <param name="GenerarPedido">The information.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        PedidoResponse SetPedido(PedidoRequest pedido);
    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    [DataContract]
    public class ProductoReq
    {
        string _id;
        string _contrasena;
        string _idproducto;
        string _idcia;
        string _idbodega;
        int _numlistprecio;

        [DataMember]
        public string IdUsuario
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Contrasena
        {
            get { return _contrasena; }
            set { _contrasena = value; }
        }

        [DataMember]
        public string IdProducto
        {
            get { return _idproducto; }
            set { _idproducto = value; }
        }

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
        Log _registro;
        string _productos;

        [DataMember]
        public Log Registro
        {
            get { return _registro; }
            set { _registro = value; }
        }

        [DataMember]
        public List<ProductosResponse> Productos { get; set; }
    }
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
        public string Contrasena { get; set; }

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
    public class RespProducto
    {
        List<Errores> _errores;
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

        //Saldo
        //PrecioL1
        //PrecioL2
        //PrecioL3
        //PrecioL4
        //PrecioL5
        [DataMember]
        public List<Errores> Errores
        {
            get { return _errores; }
            set { _errores = value; }
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
    }

    [DataContract]
    public class PedidoRequest
    {
        private Usuario _usuario;
        private List<ProductosResponse> _productos;
        private ClienteRequest _cliente;
        private string _tipopedido;
        private string _cdAgencia = "0";

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
        public List<ProductosResponse> Productos
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
        public string Agencia
        {
            get { return _cdAgencia; }
            set { _cdAgencia = value; }
        }
    }
    [DataContract]
    public class PedidoResponse
    {
        [DataMember]
        public PedidoResp Pedido { get; set; }

        [DataMember]
        public List<string> Errores { get; set; }

    }

    public class ProductoTPReq
    {
        string _id;
        string _contrasena;
        string _idproducto;
        int _numlistprecio;

        [DataMember]
        public string IdUsuario
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Contrasena
        {
            get { return _contrasena; }
            set { _contrasena = value; }
        }

        [DataMember]
        public string IdProducto
        {
            get { return _idproducto; }
            set { _idproducto = value; }
        }

        [DataMember]
        public int NumListaPrecio
        {
            get { return _numlistprecio; }
            set { _numlistprecio = value; }
        }
    }

    /// <summary>
    /// Bloque de datos para obtener la cartera
    /// </summary>
    /// <param name="_resultado">Tipo de consulta (select, insert, update).</param>
    /// <param name="_aliasConsulta">Nombre del datatable o alias.</param>
    [DataContract]
    public class CarteraReq
    {
        UsuariosRequest _usuario;
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
        public UsuariosRequest usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
    }
    /// <summary>
    /// Propiedad de la cartera del cliente
    /// </summary>
    [DataContract]
    public class CarteraResp
    {
        Log _error;
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
        public Log Error
        {
            get { return _error; }
            set { _error = value; }
        }

    }


    /// <summary>
    /// Bloque de datos para obtener la cartera Total
    /// </summary>

    [DataContract]
    public class ObtCarTotal
    {
        UsuariosRequest _usuario;
        
        [DataMember]
        public UsuariosRequest usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
    }


    [DataContract]
    public class ResObtenerCarteraTotal
    {
        Log log;

        [DataMember]
        public Log Error
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
        public string Tercero { get; set; }
    }

    /// <summary>
    /// Bloque de datos para obtener la cartera Total Def
    /// </summary>
    [DataContract]
    public class ResObtenerCartera
    {
        List<ItemCartera> _DatosCartera;
        Log log;

        [DataMember]
        public List<ItemCartera> Datoscartera
        {
            get { return _DatosCartera; }
            set { _DatosCartera = value; }
        }

        [DataMember]
        public Log Error
        {
            get { return log; }
            set { log = value; }
        }
    }


    [DataContract]
    public class authCartera

    {
        UsuariosRequest _usuario;

        [DataMember]
        public string FechaFinal { get; set; }

        [DataMember]
        public string FechaInicial { get; set; }


        [DataMember]
        public UsuariosRequest usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
       
    }

    /// <summary>
    /// Bloque de datos para obtener la informacion Maestra
    /// </summary>
    [DataContract]
    public class ResInfoMaestra
    {
        Log log;

        [DataMember]
        public Log Error
        {
            get { return log; }
            set { log = value; }
        }

        [DataMember]
        public  List<Dictionary<string, object>> Respuesta { get; set; }

    }


    [DataContract]
    public class InfoMaestra
    {
        usuario _usuario;
        private Int32 _TipoRegistro;

        [DataMember]
        public Int32 TipoRegistro { get { return _TipoRegistro; } set { _TipoRegistro = value; } }

        [DataMember]
        public usuario Usuario {
            get { return _usuario; }
            set { _usuario = value; }
        }

    }


    [DataContract]
    public class usuario {
        string _id;
       

        [DataMember]
        public string IdUsuario
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Contrasena { get; set; }

    }

    public class Regimen
    {
        public string IdRegimen { get; set; }
        public string regimen { get; set; }
    }
    
}
