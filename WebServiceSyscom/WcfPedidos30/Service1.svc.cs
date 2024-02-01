using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using WcfPedidos30.Model;

namespace WcfPedidos30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IPedido30
    {
        ConsultarProducto consProd = new ConsultarProducto();
        #region ObtenerProducto 

        [return: MessageParameter(Name = "Producto")]
        public RespProducto ConProducto(ObtProducto obtProducto)
        {
            RespProducto respuesta = new RespProducto();
            respuesta.Registro = new Log();

            if (obtProducto == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }

            else if (string.IsNullOrWhiteSpace(obtProducto.Usuarios.UserName) || string.IsNullOrWhiteSpace(obtProducto.Usuarios.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {
                ExisteUsuario usuario = new ExisteUsuario();
                if (usuario.Existe(obtProducto.Usuarios.UserName, obtProducto.Usuarios.Password, out string[] mensajeNuevo))
                {
                    PaginadorProducto<ProductosResponse> DatProducto = new PaginadorProducto<ProductosResponse>();
                    respuesta = consProd.ConsultarProductos(obtProducto.DatosProducto, obtProducto.Usuarios, out DatProducto);
                    respuesta.ListaProductos = DatProducto;
                    respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
                }
                else
                {
                    respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
                }
            }

            return respuesta;
        }
        #endregion

        [return: MessageParameter(Name = "Cliente")]
        public RespCliente ObjCliente(ObtCliente obtCliente)
        {
            ConsultarCliente consClie = new ConsultarCliente();
            RespCliente respuesta = new RespCliente();
            respuesta.Registro = new Log();

            if (obtCliente.Usuarios.UserName == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }

            else if (string.IsNullOrWhiteSpace(obtCliente.Usuarios.Password) || string.IsNullOrWhiteSpace(obtCliente.Usuarios.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {
                ExisteUsuario usuario = new ExisteUsuario();
                if (usuario.Existe(obtCliente.Usuarios.UserName, obtCliente.Usuarios.Password, out string[] mensajeNuevo))
                {
                    List<ClienteResponse> cliente = new List<ClienteResponse>();
                    respuesta = consClie.ConsultarClientes(obtCliente, obtCliente.Usuarios, out cliente);
                    respuesta.DatosClientes = cliente;
                    respuesta.Registro = new Log { Codigo = "999", Descripcion = "Ok" };
                }
                else
                {
                    respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
                }
            }
            return respuesta;
        }

        [return: MessageParameter(Name = "Usuario")]
        public RespUsuario ObjUsuario(ObtUsuario obtUsuario)
        {
            ConexionBD con = new ConexionBD();
            DataSet TablaUsuarios = new DataSet();
            RespUsuario respuesta = new RespUsuario();
            respuesta.Registro = new Log();

            if (obtUsuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }

            else if (string.IsNullOrWhiteSpace(obtUsuario.Usuarios.UserName) || string.IsNullOrWhiteSpace(obtUsuario.Usuarios.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {
                ExisteUsuario existe = new ExisteUsuario();
                con.setConnection("Syscom");
                DataSet TablaUsuario = new DataSet();
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Usuario", obtUsuario.Usuarios.UserName));
                if (existe.Existe(obtUsuario.Usuarios.UserName, obtUsuario.Usuarios.Password, out string[] mensajeNuevo))
                {
                    if (con.ejecutarQuery("WSObtenerUsuario", parametros, out TablaUsuario, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                    {
                        List<UsuariosResponse> datosUsuario = new List<UsuariosResponse>();
                        IEnumerable<DataRow> data = TablaUsuario.Tables[0].AsEnumerable();
                        IEnumerable<DataRow> dataFil = data.GroupBy(g => g.Field<string>("IdUsuario")).Select(g => g.First());
                        //Convert.ToBoolean(row["EsCliente"]) != false [Validaciòn para saber si el usuario es registrado como cliente]
                        if (dataFil.Any(row => row["EsCliente"] != null && row["EsCliente"] != DBNull.Value) == true)
                        {
                            dataFil.ToList().ForEach(i => datosUsuario.Add(new UsuariosResponse
                            {
                                Bodega = i.Field<string>("Bodega"),
                                Compañía = i.Field<string>("Compañía"),
                                EsCliente = i.Field<bool>("EsCliente"),
                                Esvendedor = i.Field<bool>("Esvendedor"),
                                IdUsuario = i.Field<string>("IdUsuario"),
                                NombreTercero = i.Field<string>("NombreTercero")
                            }));
                            respuesta.Registro = new Log { Codigo = "999", Descripcion = "Ok" };
                            respuesta.DatosUsuarios = datosUsuario.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    respuesta.Registro = new Log { Codigo = "USER_001", Descripcion = "¡Usuario no encontrado!" };
                }
            }
            return respuesta;
        }

        [return: MessageParameter(Name = "Pedido")]
        public ResGenerarPedido setPedido(DtPedido pedido)
        {
            {
                ResGenerarPedido respuesta = new ResGenerarPedido();
                respuesta.Error = null;
                List<SqlParameter> _parametros = new List<SqlParameter>();

                try
                {
                    if (pedido.Usuarios == null)
                        respuesta.Error = new Log { Codigo = "USER_002", Descripcion = "¡Todas las variables del usuario no pueden ser nulas!" };
                    else
                    {
                        if (pedido.Usuarios.UserName == null || String.IsNullOrWhiteSpace(pedido.Usuarios.UserName))
                            respuesta.Error = new Log { Codigo = "USER_003", Descripcion = "¡El UserName no puede ser nulo o vacío!" };
                        else if (pedido.Usuarios.Password == null || String.IsNullOrWhiteSpace(pedido.Usuarios.Password))
                            respuesta.Error = new Log { Codigo = "USER_003", Descripcion = "¡El Password no puede ser nulo o vacío!" };
                        else if (pedido.Pedido.IdCliente == null || String.IsNullOrWhiteSpace(pedido.Pedido.IdCliente))
                            respuesta.Error = new Log { Codigo = "GPED_001", Descripcion = "¡El IdCliente no puede ser nulo o vacío!" };
                        else if (pedido.Pedido.CodConcepto == null || String.IsNullOrWhiteSpace(pedido.Pedido.CodConcepto))
                            respuesta.Error = new Log { Codigo = "GPED_001", Descripcion = "¡El CodConcepto no puede ser nulo o vacío!" };
                        else if (pedido.Pedido.IdVendedor == null || String.IsNullOrWhiteSpace(pedido.Pedido.IdVendedor))
                            respuesta.Error = new Log { Codigo = "GPED_001", Descripcion = "¡El IdVendedor no puede ser nulo o vacío!" };
                        else if (pedido.Pedido.Observación == null || String.IsNullOrWhiteSpace(pedido.Pedido.Observación))
                            respuesta.Error = new Log { Codigo = "GPED_001", Descripcion = "¡La Observación no puede ser nula o vacía!" };
                        else if (pedido.Pedido.ListaProductos.Count == 0)
                            respuesta.Error = new Log { Codigo = "GPED_003", Descripcion = "¡No existen ningún producto para generar el pedido!" };
                        else if (ExisteUsuario(pedido.Usuarios))
                        {
                            Pedido dpd = new Pedido();
                            List<Pedido> DatPedido = new List<Pedido>();
                            List<Pedido> ppedido = new List<Pedido>();
                            respuesta.Error = dpd.GenerarPedido(pedido, out DatPedido);
                            if (respuesta.Error == null)
                            {
                                if (DatPedido == null)
                                    respuesta.Error = new Log { Codigo = "USER_001", Descripcion = "¡Usuario no encontrado!" };
                                else
                                    respuesta.DatosPedido = DatPedido;
                            }

                        }
                        else
                            respuesta.Error = new Log { Codigo = "USER_001", Descripcion = "¡Usuario no encontrado!" };
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Error = new Log { Descripcion = ex.Message };
                }

                return respuesta;
            }
        }
    }
}
