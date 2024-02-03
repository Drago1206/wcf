using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using wcfSyscom30.Models;

namespace wcfSyscom30
{

    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Servicio30 : IServicio30
    {
        public bool ExisteUsuario(Usuario Modelo)
        {
            sys_Usuarios us = new sys_Usuarios();
            bool _validar = us.ExisteUsuario(Modelo.UserName, Modelo.Password);
            return _validar;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerUsuario", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Usuario")]
        public ResDatosUsuario getUsuario(DtUsuario Modelo)
        {
            ResDatosUsuario respuesta = new ResDatosUsuario();
            respuesta.Error = null;

            try
            {
                if (Modelo.Usuarios == null)
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío! " };
                    else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                    else if (ExisteUsuario(Modelo.Usuarios))
                    {
                        DatosUsuario usu = new DatosUsuario();
                        DatosUsuario DatUsuario = new DatosUsuario();
                        respuesta.Error = usu.ConsultarUsuarios(Modelo.Usuarios.UserName, out DatUsuario);
                        if (respuesta.Error == null)
                        {
                            if (DatUsuario == null)
                                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                            else
                                respuesta.DatosUsuarios = DatUsuario;
                        }   
                    }
                    else
                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Cliente")]
        public ResObtenerClientes getClientes(DtCliente Modelo)
        {
            ResObtenerClientes respuesta = new ResObtenerClientes();
            respuesta.Error = null;

            try
            {
                if (Modelo.Usuarios == null)
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
                    else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                    else if (Modelo.Cliente.NitCliente == null || String.IsNullOrWhiteSpace(Modelo.Cliente.NitCliente))
                        respuesta.Error = new Errores { codigo = "CLIEN_001", descripcion = "¡El NitCliente no puede ser nulo o vacío!" };
                    else if (ExisteUsuario(Modelo.Usuarios))
                    {
                        DatosCliente dcl = new DatosCliente();
                        List<DatosCliente> DatCliente = new List<DatosCliente>();
                        respuesta.Error = dcl.ConsultarCliente(Modelo.Cliente.NitCliente,out DatCliente);
                        if (respuesta.Error == null)
                        {
                            if (DatCliente == null)
                                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                            else
                                respuesta.DatosClientes = DatCliente;
                        }
                    }
                    else
                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerProductos", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Productos")]
        public ResObtenerProductos getProductos(DtProducto Modelo)
        {
            ResObtenerProductos respuesta = new ResObtenerProductos();
            respuesta.Error = null;

            try
            {
                if (Modelo.Usuarios == null)
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
                    else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                    //else if (Modelo.DatosProducto.CodOrDesProd == null || String.IsNullOrWhiteSpace(Modelo.DatosProducto.CodOrDesProd))
                    //    respuesta.Error = new Errores { codigo = "PROD_001", descripcion = "¡El CodOrDesProd no puede ser nulo o vacío!" };
                    //else if (Modelo.DatosProducto.CodOrDesProd == null)
                    //    respuesta.Error = new Errores { codigo = "PROD_002", descripcion = "¡Se debe digitar criterio de búsqueda!" };
                    else if (ExisteUsuario(Modelo.Usuarios))
                    {
                        itemProducto dpr = new itemProducto();
                        //List<ListadoProductosPag> DatProducto = new List<ListadoProductosPag>();
                        PaginadorProducto<itemProducto> DatProducto = new PaginadorProducto<itemProducto>();


                        respuesta.Error = dpr.ConsultarProducto(Modelo.DatosProducto,Modelo.Usuarios, out DatProducto);
                        if (respuesta.Error == null)
                        {
                            if (DatProducto == null)
                                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                            else
                                respuesta.ListaProductos = DatProducto;
                        }
                    }
                    else
                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GenerarPedido", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Pedido")]
        public ResGenerarPedido setPedido(DtPedido Modelo)
        {
            ResGenerarPedido respuesta = new ResGenerarPedido();
            respuesta.Error = null;
            List<SqlParameter> _parametros = new List<SqlParameter>();

            try
            {
                if (Modelo.Usuarios == null)
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
                    else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                    else if (Modelo.Pedido.IdCliente == null || String.IsNullOrWhiteSpace(Modelo.Pedido.IdCliente))
                        respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡El IdCliente no puede ser nulo o vacío!" };
                    else if (Modelo.Pedido.CodConcepto == null || String.IsNullOrWhiteSpace(Modelo.Pedido.CodConcepto))
                        respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡El CodConcepto no puede ser nulo o vacío!" };
                    else if (Modelo.Pedido.IdVendedor == null || String.IsNullOrWhiteSpace(Modelo.Pedido.IdVendedor))
                        respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡El IdVendedor no puede ser nulo o vacío!" };
                    else if (Modelo.Pedido.Observación == null || String.IsNullOrWhiteSpace(Modelo.Pedido.Observación))
                        respuesta.Error = new Errores { codigo = "GPED_001", descripcion = "¡La Observación no puede ser nula o vacía!" };
                    else if (Modelo.Pedido.ListaProductos.Count == 0)
                        respuesta.Error = new Errores { codigo = "GPED_003", descripcion = "¡No existen ningún producto para generar el pedido!" };
                    else if (ExisteUsuario(Modelo.Usuarios))
                    {
                        DatosPedido dpd = new DatosPedido();
                        List<DatosPedido> DatPedido = new List<DatosPedido>();
                        List<Pedido> ppedido = new List<Pedido>();
                        respuesta.Error = dpd.GenerarPedido(Modelo,out DatPedido);
                        if (respuesta.Error == null)
                        {
                            if (DatPedido == null)
                                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                            else
                                respuesta.DatosPedido = DatPedido;
                        }

                    }
                    else
                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ObtenerCartera", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "Cartera")]
        public ResObtenerCartera getCartera(DtCliente Modelo)
        {
            ResObtenerCartera respuesta = new ResObtenerCartera();
            respuesta.Error = null;

            try
            {
                if (Modelo.Usuarios == null)
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
                    respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
                else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
                    respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                else if (Modelo.Cliente.NitCliente == null || String.IsNullOrWhiteSpace(Modelo.Cliente.NitCliente))
                    respuesta.Error = new Errores { codigo = "CLIEN_001", descripcion = "¡El NitCliente no puede ser nulo o vacío!" };
                else if (ExisteUsuario(Modelo.Usuarios))
                {
                    DatosCartera dcl = new DatosCartera();
                    List<DatosCartera> DatCartera = new List<DatosCartera>();
                    respuesta.Error = dcl.ConsultarCartera(Modelo.Cliente.NitCliente, out DatCartera);
                    if (respuesta.Error == null)
                    {
                        if (DatCartera == null)
                            respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                        else
                            respuesta.Datoscartera = DatCartera;
                    }
                }
                else
                    respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }
            return respuesta;
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ConsolidadoClientes", BodyStyle = WebMessageBodyStyle.Bare)]
        [return: MessageParameter(Name = "cliente")]
        public ResObtenerConsClientes getConsClientes(DtClientes Modelo)
        {
            ResObtenerConsClientes respuesta = new ResObtenerConsClientes();
            respuesta.Error = null;

            try
            {
                if (Modelo.Usuarios == null)
                    respuesta.Error = new Errores { codigo = "USER_002", descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                else
                {
                    if (Modelo.Usuarios.UserName == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.UserName))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };
                    else if (Modelo.Usuarios.Password == null || String.IsNullOrWhiteSpace(Modelo.Usuarios.Password))
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };
                    //else if (Modelo.Cliente.NitCliente == null || String.IsNullOrWhiteSpace(Modelo.Cliente.NitCliente))
                    //    respuesta.Error = new Errores { codigo = "CLIEN_001", descripcion = "¡El NitCliente no puede ser nulo o vacío!" };
                    else if (ExisteUsuario(Modelo.Usuarios))
                    {
                        DatosCliente dcl = new DatosCliente();
                        PaginadorCliente<DatosCliente> DatCliente = new PaginadorCliente<DatosCliente>();
                        respuesta.Error = dcl.ConsultarConsCliente(Modelo.Clientes, out DatCliente);
                        if (respuesta.Error == null)
                        {
                            if (DatCliente == null)
                                respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                            else
                                respuesta.ListadoClientes = DatCliente;
                        }
                    }
                    else
                        respuesta.Error = new Errores { codigo = "USER_001", descripcion = "¡Usuario no encontrado!" };
                }
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }
    }
}
