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
    public class Pedidos30 : IPedido30
    {
        ConsultarProducto consProd = new ConsultarProducto();
        #region ObtenerProducto 
        /// <summary>
        /// Método que recibe la respuesta de la solicitud del producto
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

        #region ObtenerCliente
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
        #endregion

        #region ObtenerUsuario
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
                            respuesta.Registro = new Log { Codigo = "USER_064", Descripcion = "Respuesta exitosa" };
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
        #endregion

        #region GenerarPedido

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
                            else
                            {
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
        #endregion
                
        #region ObtenerCartera


        //El metodo obtener cartera tiene la funcionalidad de mostrar la cartera de un cliente que viene por nit o de todos los clientes,
        // si  dicho nit llega como nulo entonces el resultado del metodo mostrara todas las carteras
        public CarteraResp RespCartera(CarteraReq ReqCartera)
        {
            //Instanciamo la conexion
            ConexionBD con = new ConexionBD();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            CarteraResp respuesta = new CarteraResp();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;
                if (ReqCartera.usuario == null)
                {
                    respuesta.Error = new Log { Codigo = "user_002", Descripcion = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else
                {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(ReqCartera.usuario.UserName, ReqCartera.usuario.Password, out string[] mensajeNuevo))
                    {
                        respuesta.Error = new Log { Codigo = "999", Descripcion = "Ok" };
                        if (ReqCartera.NitCliente == null || string.IsNullOrWhiteSpace(ReqCartera.NitCliente))
                        {
                            // Codigo para obtener todas las carteras..
                        }
                        else
                        {
                            // Implementamos la instancia de un dataset para representar un conjunto completo de datos, incluyendo las tablas que contienen, ordenan y restringen los datos
                            DataSet Tablainfo = new DataSet();
                            //Instanciamos la clase de cartera para poder ingresar los datos obtenidos en el metodo
                            Cartera cart = new Cartera();
                            //Instanciamos la clase item cartera para instanciar la lista de cartera que esta clase contiene 
                            ItemCartera cartItem = new ItemCartera();
                            //Creamos una nueva instancia de la lista de cartera la cual la contienen un nombre que se llama detalle.


                            List<ItemCartera> datItemCart = new List<ItemCartera>();


                            try
                            {
                                //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                                // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                                con.setConnection("SyscomDBSAL");
                                //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                                List<SqlParameter> parametros = new List<SqlParameter>();
                                //Indicamos el parametro que vamos a pasar 
                                parametros.Add(new SqlParameter("@NitCliente", ReqCartera.NitCliente));
                                con.addParametersProc(parametros);

                                //Ejecuta procedimiento almacenado
                                //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                                DataTable DT = new DataTable();
                                // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                                con.resetQuery();
                                //Verificamos y ejecutamos al mismo tiempo el procedimiento de almacenado 
                                if (con.ejecutarQuery("WcfPedidos30_ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                                {
                                    //IEnumerable Convierte la tabla en una secuencia de objetos DataRow que se pueden usar en consultas LINQ.



                                    datItemCart = con.DataTableToList<ItemCartera>("Tercero,SaldoCartera".Split(','), Tablainfo);
                                    datItemCart.ForEach(m =>
                                    {
                                        m.Detalle = new List<Cartera>();

                                        m.Detalle = con.DataTableToList<Cartera>(Tablainfo.Tables[0].Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("Tercero").Equals(m.Tercero)).CopyToDataTable().AsDataView().ToTable(true, "TipoDocumento,Documento,Compañia,Vencimiento,FechaEmision,FechaVencimiento,ValorTotal,Abono,Saldo".Split(',')));
                                    });


                                    //Pasamos las listas obtenidas a los bloques de contrato para de esta manera poder obtener los datos.
                                    respuesta.DatosCartera = datItemCart;
                                    //respuesta.DatosCartera.Add(cartItem);


                                }

                            }
                            catch (Exception e)
                            {
                                respuesta.Error = new Log { Descripcion = e.Message };
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Descripcion = ex.Message };
            }
            return respuesta;
        }
        #endregion


        //El metodo cosolidado de clientes tiene la funcionalidad de mostrar la informacion de algunos clientes con sus respectivas agencias ,
        //si llega un nit que le exija al metodo un resultado en especifico este lo dara , en el caso contrario dara la informacion de todos los clientes
        public RespClientes resClients(ObtInfoClientes obtenerConSolidado)
        {
            RespClientes respuesta = new RespClientes();
            ClienteResponse agencia = new ClienteResponse();
            respuesta.Error = null;
            ConexionBD con = new ConexionBD();
            string cliente = "";
            List<ClienteResponse> clientes = new List<ClienteResponse>();
            int ResultadoPorPagina = 10;

            try
            {
                ExisteUsuario ExistUsu = new ExisteUsuario();
                if (ExistUsu.Existe(obtenerConSolidado.usuario.UserName, obtenerConSolidado.usuario.Password, out string[] mensajeNuevo))
                {
                    respuesta.Error = new Log { Codigo = "999", Descripcion = "Ok" };

                    if (obtenerConSolidado.usuario.UserName == null || String.IsNullOrWhiteSpace(obtenerConSolidado.usuario.UserName))
                    {
                        respuesta.Error = new Log { Codigo = "USER_003", Descripcion = "¡El UserName no puede ser nulo o vacío!" };

                    }
                    else if (obtenerConSolidado.usuario.Password == null || String.IsNullOrWhiteSpace(obtenerConSolidado.usuario.Password))
                    {
                        respuesta.Error = new Log { Codigo = "USER_003", Descripcion = "¡El Password no puede ser nulo o vacío!" };

                    }
                    else if (obtenerConSolidado.NitCliente == null || String.IsNullOrWhiteSpace(obtenerConSolidado.NitCliente))
                    {
                        // Si NitCliente es nulo o está en blanco, asumimos un valor predeterminado 
                        //obtenerConSolidado.NitCliente = null; // Asigna un valor predeterminado 

                        con.setConnection("Syscom");
                        DataSet TablaCliente = new DataSet();
                        //le pasamos el valor del nit ingresado por el usuario   
                        cliente = obtenerConSolidado.NitCliente;

                        //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                        List<SqlParameter> parametros = new List<SqlParameter>();
                        PaginadorCliente<ClienteResponse> paginador = new PaginadorCliente<ClienteResponse>();
                        
                        int Regisros_X_Pagina = paginador.RegistrosPorPagina;
                        int NumeroPagina = paginador.PaginaActual;

                        //Indicamos el parametro que vamos a pasar 
                        parametros.Add(new SqlParameter("@NitCliente", obtenerConSolidado.NitCliente));
                        con.addParametersProc(parametros);

                        //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                        DataTable DT = new DataTable();

                        // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                        con.resetQuery();
                        if (con.ejecutarQuery("WcfPedido_ConsolidacionClientes", parametros, out TablaCliente, out string[] NuevoMensaje, CommandType.StoredProcedure))
                        {
                            clientes = con.DataTableToList<ClienteResponse>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','), TablaCliente);

                            // creamos un DataTable con la cual asignando la primera tabla dentro del conjunto de tablas TablaCliente a la variable lista. 
                            //Esto significa que lista contendrá la primera tabla del conjunto de datos TablaCliente.
                            DataTable lista = TablaCliente.Tables[0];
                            int TotalRegistros = TablaCliente.Tables[0].Rows.Count;

                            clientes.ForEach(m =>
                            {
                                // Inicializa la lista de agencias
                                m.ListaAgencias = new List<Agencia>();
                                //Pasamos la instancia de la clase cliente la cual contiene una lista de agencias para poder obtener los valores de allí.
                                //Le pasamos un objeto tipo DataTable de tipo lista para recibir listas de tipo string y poder asignarles el valor de la consulta
                                m.ListaAgencias = con.DataTableToList<Agencia>(lista.Copy().Rows.Cast<DataRow>()
                                                                .Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente))
                                                                .CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                            });
                            respuesta.ListadoClientes = new PaginadorCliente<ClienteResponse> { Resultado = clientes };

                            respuesta.Error = new Log { Codigo = "008", Descripcion = "Se ejecutó correctamente la consulta" };

                        }
                    }
                    else
                    {
                        try
                        {
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                            con.setConnection("Prod");
                            DataSet TablaCliente = new DataSet();

                            //Se realiza una lista de parametros para poder ingresar los datos a los procesos de almacenado
                            List<SqlParameter> parametros = new List<SqlParameter>();
                            PaginadorCliente<ClienteResponse> paginador = new PaginadorCliente<ClienteResponse>();
                            int Regisros_X_Pagina = paginador.RegistrosPorPagina;
                            int NumeroPagina = 1;

                            //Indicamos el parametro que vamos a pasar 
                            parametros.Add(new SqlParameter("@NitCliente", obtenerConSolidado.NitCliente));
                            con.addParametersProc(parametros);

                            //Representa una tabla de datos en memoria, en esta mostraremos los datos obtenidos en una tabla.
                            DataTable DT = new DataTable();

                            // Se usa para limpiar cualquier estado interno o consultas previas que se hayan configurado en ese objeto con.
                            con.resetQuery();

                            // Define NumeroPagina y ResultadoPorPagina
                            ////int NumeroPagina = 1; // Número de la página solicitada




                            if (con.ejecutarQuery("ConsolidacionClientes", parametros, out TablaCliente, out string[] NuevoMensaje, CommandType.StoredProcedure))
                            {
                                // Calcula el número total de elementos en la tabla 1 de TablaCliente
                                int ResultadoTotal = TablaCliente.Tables[0].Rows.Count;

                                int totalPaginas = (int)Math.Ceiling((double)ResultadoTotal / ResultadoPorPagina);

                                // Verifica si la página solicitada es válida
                                if (NumeroPagina <= totalPaginas)
                                {
                                    // Calcula el índice de inicio y fin para la paginación
                                    int startIndex = (NumeroPagina - 1) * ResultadoPorPagina;
                                    int endIndex = Math.Min(startIndex + ResultadoPorPagina, ResultadoTotal);

                                    // Filtrar los datos por el NIT del cliente y aplicar paginación
                                    IEnumerable<DataRow> data = TablaCliente.Tables[0].AsEnumerable()
                                                                //Verificamos el nit que obtenemos por medio del bloque de contrato
                                                                .Where(row => row.Field<string>("NitCliente") == obtenerConSolidado.NitCliente)
                                                                //Indicamos el skip para podernos saltar dicha cantidad de paginas
                                                                .Skip(startIndex)
                                                                //Implementacion del take para tomar los resultados por pagina
                                                                .Take(ResultadoPorPagina);

                                    clientes = con.DataTableToList<ClienteResponse>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','), TablaCliente);


                                    // creamos un DataTable con la cual asignando la primera tabla dentro del conjunto de tablas TablaCliente a la variable lista. 
                                    //Esto significa que lista contendrá la primera tabla del conjunto de datos TablaCliente.
                                    DataTable lista = TablaCliente.Tables[0];
                                    clientes.ForEach(m =>
                                    {   // Inicializa la lista de agencias
                                        m.ListaAgencias = new List<Agencia>();
                                        //Pasamos la instancia de la clase cliente la cual contiene una lista de agencias para poder obtener los valores de allí.
                                        //Le pasamos un objeto tipo DataTable de tipo lista para recibir listas de tipo string y poder asignarles el valor de la consulta
                                        m.ListaAgencias = con.DataTableToList<Agencia>(lista.Copy().Rows.Cast<DataRow>()
                                                                        .Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente))
                                                                        .CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                                    });

                                    // Asignar la lista filtrada a RespClientes
                                    //La lista obtenida en clientes se la pasamos para poder realizar un paginado de esta

                                    respuesta.ListadoClientes = new PaginadorCliente<ClienteResponse> { Resultado = clientes };
                                    respuesta.ListadoClientes = new PaginadorCliente<ClienteResponse>
                                    {
                                        Resultado = clientes,
                                        PaginaActual = NumeroPagina,
                                        TotalRegistros = ResultadoTotal,
                                        TotalPaginas = totalPaginas,
                                        RegistrosPorPagina = ResultadoTotal

                                    };

                                    respuesta.Error = new Log { Codigo = "008", Descripcion = "Se ejecutó correctamente la consulta" };

                                    //int ResultadoPorPagina = 10; // Número de resultados por página (puedes ajustarlo según tus necesidades)
                                }
                                else
                                {
                                    // La página solicitada no es válida, maneja el error
                                }
                            }

                        }

                        catch (Exception e)
                        {

                            respuesta.Error = new Log { Descripcion = e.Message };
                        }
                    }
                }

                else
                {
                    respuesta.Error = new Log { Codigo = "999", Descripcion = "El usuario no existe" };//El usuario no existe
                }//else
            }
            catch (Exception ex)
            {
                respuesta.Error = new Log { Descripcion = ex.Message };
            }

            return respuesta;
        }

    }
}
