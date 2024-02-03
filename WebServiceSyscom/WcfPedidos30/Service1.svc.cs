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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obtProducto">Objeto que almacena los datos de la solicitud del cliente</param>
        /// <returns></returns>
        [return: MessageParameter(Name = "Producto")]
        public RespProducto ConProducto(ObtProducto obtProducto)
        {
            // Crear una nueva instancia de RespProducto y Log
            RespProducto respuesta = new RespProducto();
            respuesta.Registro = new Log();

            // Verificar si obtProducto es nulo
            if (obtProducto == null)
            {
                // Si es nulo, establecer el código y descripción del error
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }
            // Verificar si el nombre de usuario o la contraseña están vacíos
            else if (string.IsNullOrWhiteSpace(obtProducto.Usuarios.UserName) || string.IsNullOrWhiteSpace(obtProducto.Usuarios.Password))
            {
                // Si es así, establecer el código y descripción del error
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {
                // Crear una nueva instancia de ExisteUsuario
                ExisteUsuario usuario = new ExisteUsuario();
                // Verificar si el usuario existe
                if (usuario.Existe(obtProducto.Usuarios.UserName, obtProducto.Usuarios.Password, out string[] mensajeNuevo))
                {
                    // Si el usuario existe, consultar los productos
                    PaginadorProducto<ProductosResponse> DatProducto = new PaginadorProducto<ProductosResponse>();
                    respuesta = consProd.ConsultarProductos(obtProducto.DatosProducto, obtProducto.Usuarios, out DatProducto, out string[] mensaje);
                    // Establecer la lista de productos y el registro de log
                    respuesta.ListaProductos = DatProducto;
                    respuesta.Registro = new Log { Codigo = mensaje[0], Descripcion = mensaje[1] };
                }
                else
                {
                    // Si el usuario no existe, establecer el código y descripción del error
                    respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
                }
            }

            // Retorna la variable que contiene la respuesta del producto
            return respuesta;
        }

        #endregion

        [return: MessageParameter(Name = "Cliente")]
        public RespCliente ObjCliente(ObtCliente obtCliente)
        {
            // Crear una nueva instancia de ConsultarCliente y RespCliente
            ConsultarCliente consClie = new ConsultarCliente();
            RespCliente respuesta = new RespCliente();
            respuesta.Registro = new Log();

            // Verificar si el nombre de usuario es nulo
            if (obtCliente.Usuarios.UserName == null)
            {
                // Si es nulo, establecer el código y descripción del error
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }
            // Verificar si la contraseña está vacía
            else if (string.IsNullOrWhiteSpace(obtCliente.Usuarios.Password) || string.IsNullOrWhiteSpace(obtCliente.Usuarios.Password))
            {
                // Si es así, establecer el código y descripción del error
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {
                // Crear una nueva instancia de ExisteUsuario
                ExisteUsuario usuario = new ExisteUsuario();
                // Verificar si el usuario existe
                if (usuario.Existe(obtCliente.Usuarios.UserName, obtCliente.Usuarios.Password, out string[] mensajeNuevo))
                {
                    // Si el usuario existe, consultar los clientes
                    List<ClienteResponse> cliente = new List<ClienteResponse>();
                    respuesta = consClie.ConsultarClientes(obtCliente, obtCliente.Usuarios, out cliente);
                    // Establecer la lista de clientes y el registro de log
                    respuesta.DatosClientes = cliente;
                    respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
                }
                else
                {
                    // Si el usuario no existe, establecer el código y descripción del error
                    respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
                }
            }

            // Devolver la respuesta
            return respuesta;
        }

        /// <summary>
        /// Método que obtiene la información del usuario
        /// </summary>
        /// <param name="obtUsuario">Objeto que contiene la información de solicitud del usuario</param>
        /// <returns>Devuelve un objeto de tipo RespUsuario que contiene la información de respuesta para el usuario</returns>
        [return: MessageParameter(Name = "Usuario")]
        public RespUsuario ObjUsuario(ObtUsuario obtUsuario)
        {

            List<UsuariosResponse> dtUsuario = new List<UsuariosResponse>();
            // Crea una instancia de la base de datos
            ConexionBD con = new ConexionBD();
            // Crea una instancia para la tabla de respuesta del usuario
            DataSet TablaUsuarios = new DataSet();
            // Crea una instancia del objeto de respuesta
            RespUsuario respuesta = new RespUsuario();
            // Se declara una instancia para el registro 
            respuesta.Registro = new Log();
            // Condición que verifica si el objeto de la solicitud es nulo
            if (obtUsuario == null)
            {
                // En caso de que la condición se cumpla, se declara un mensaje de error con código y descripción
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }
            // Condición que verifica que los datos de solicitud del usuario no estén vacíos
            else if (string.IsNullOrWhiteSpace(obtUsuario.Usuarios.UserName) || string.IsNullOrWhiteSpace(obtUsuario.Usuarios.Password))
            {
                // En caso de que la condición se cumpla, se declara un mensaje de error con código y descripción
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            // Condición en caso de que no se cumplan las anteriores condiciones
            else
            {
                // Se declara una instancia de la clase que verifica la existencia del usuario
                ExisteUsuario existe = new ExisteUsuario();
                // Se define la conexión a la base de datos en base al nombre de la cadena de conexion
                con.setConnection("Syscom");
                // Se inicializa el dataset para capturar la tabla del resultado del procedimiento de almacenado
                DataSet TablaUsuario = new DataSet();
                // Se inicializa una lista vacía para pasar los parámetros al procedimiento de almacenado
                List<SqlParameter> parametros = new List<SqlParameter>();
                // Se agrega el parámetro necesario para ejecutar el procedimiento de almacenado, con el dato del usuario
                parametros.Add(new SqlParameter("@Usuario", obtUsuario.Usuarios.UserName));
                // Condición que verificar si el usuario existe
                if (existe.Existe(obtUsuario.Usuarios.UserName, obtUsuario.Usuarios.Password, out string[] mensajeNuevo))
                {
                    // Condición que verifica si la consulta por medio del procedimiento de almacenado se efectuó correctamente
                    if (con.ejecutarQuery("WSObtenerUsuario", parametros, out TablaUsuario, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                    {
                        // Se declara una lista que contendrá la respuesta de la solicitud del usuario
                        List<UsuariosResponse> datosUsuario = new List<UsuariosResponse>();

                        // Se define un objeto numerable para los datos de la tabla que contiene la información de los usuarios 
                        IEnumerable<DataRow> data = TablaUsuario.Tables[0].AsEnumerable();
                        // Se agrupan los elementos del objeto data 
                        IEnumerable<DataRow> dataFil = data.GroupBy(g => g.Field<string>("IdUsuario")).Select(g => g.First());
                        // Validación para saber si el usuario es registrado como cliente
                        if (dataFil.Any(row => row["EsCliente"] != null && row["EsCliente"] != DBNull.Value) == true)
                        {
                            // Se le asigna el valor a la variable dtCliente con la lista que retorna el método DataTableToList
                            dtUsuario = con.DataTableToList<UsuariosResponse>("Bodega,Compañía,EsCliente,Esvendedor,IdUsuario,NombreTercero".Split(','), TablaUsuario);
                            // Se le asigna el valor al objeto DatosUsuario con los datos que retorna el método DataTableToList 
                            respuesta.DatosUsuarios = dtUsuario.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    // En caso de que la condición no se cumpla, se establece un mensaje de respuesta
                    respuesta.Registro = new Log { Codigo = "USER_001", Descripcion = "¡Usuario no encontrado!" };
                }
            }

            // Retorna el objeto de respuesta para RespUsuario
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
                    ExisteUsuario existe = new ExisteUsuario();
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
                        else if (existe.Existe(pedido.Usuarios.UserName, pedido.Usuarios.Password, out string[] nuevoMensaje))
                        {
                            GenerarPedido genPedido = new GenerarPedido();
                            List<PedidoResponse> DatPedido = new List<PedidoResponse>();
                            List<PedidoRequest> ppedido = new List<PedidoRequest>();
                            respuesta.Error = genPedido.GenerarPedidos(pedido, out DatPedido);
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
