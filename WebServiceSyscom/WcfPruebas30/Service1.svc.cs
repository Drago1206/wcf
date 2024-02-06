using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos30.Model;
using WcfPedidos30.Models;
using WcfPruebas30.Models;
using WcfSyscom30.Conexion;
using static WcfPruebas30.CarteraReq;

namespace WcfPruebas30
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public int NumeroPagina = 1;
        public int ResultadoPorPagina = 10;
        public int RegistroXPagina = 10;
        public int InicioPaginacion = 0;
        public int FinPaginacion = 10;
        public int ResultadoTotal = 0;




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
                    respuesta.Error = new Errores { codigo = "user_002", descripcion = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(ReqCartera.usuario.UserName, ReqCartera.usuario.Password, out string[] mensajeNuevo)) {
                        respuesta.Error = new Errores { codigo = "999", descripcion = "Ok" };
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
                                if (con.ejecutarQuery("ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
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
                                     respuesta.DatosCartera.Add(cartItem);


                                }

                            }
                            catch (Exception e)
                            {
                                respuesta.Error = new Errores { descripcion = e.Message };
                            }
                        }
                    }
                }
                   

            }
            catch (IOException ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }
            return respuesta;
        }

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



            try
            {
                ExisteUsuario ExistUsu = new ExisteUsuario();
                if (ExistUsu.Existe(obtenerConSolidado.usuario.Password, obtenerConSolidado.usuario.UserName, out string[] mensajeNuevo)) {
                    respuesta.Error = new Errores { codigo = "999", descripcion = "Ok" };

                    if (obtenerConSolidado.usuario.UserName == null || String.IsNullOrWhiteSpace(obtenerConSolidado.usuario.UserName))
                    {
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El UserName no puede ser nulo o vacío!" };

                    }
                    else if (obtenerConSolidado.usuario.Password == null || String.IsNullOrWhiteSpace(obtenerConSolidado.usuario.Password))
                    {
                        respuesta.Error = new Errores { codigo = "USER_003", descripcion = "¡El Password no puede ser nulo o vacío!" };

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
                        if (con.ejecutarQuery("ConsolidacionClientes", parametros, out TablaCliente, out string[] NuevoMensaje, CommandType.StoredProcedure))
                        {
                            clientes = con.DataTableToList<ClienteResponse>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','), TablaCliente);

                            // creamos un DataTable con la cual asignando la primera tabla dentro del conjunto de tablas TablaCliente a la variable lista. 
                            //Esto significa que lista contendrá la primera tabla del conjunto de datos TablaCliente.
                            DataTable lista = TablaCliente.Tables[0];
                            int TotalRegistros = TablaCliente.Tables[0].Rows.Count;

                            clientes.ForEach(m =>
                            {
                                // Inicializa la lista de agencias
                                m.ListaAgencia = new List<Agencia>();
                                //Pasamos la instancia de la clase cliente la cual contiene una lista de agencias para poder obtener los valores de allí.
                                //Le pasamos un objeto tipo DataTable de tipo lista para recibir listas de tipo string y poder asignarles el valor de la consulta
                                m.ListaAgencia = con.DataTableToList<Agencia>(lista.Copy().Rows.Cast<DataRow>()
                                                                .Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente))
                                                                .CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                            });
                            respuesta.ListadoClientes = new PaginadorCliente<ClienteResponse> { Resultado = clientes };

                            respuesta.Error = new Errores { codigo = "008", descripcion = "Se ejecutó correctamente la consulta" };

                        }
                    }
                    else
                    {
                        try
                        {
                            //Se conecta a la cadena de conexion la cual recibe como nombre syscom.
                            // Tiene la funcionalidad de conectar con la base de datos y realizar los procedimientos
                            con.setConnection("Syscom");
                            DataSet TablaCliente = new DataSet();

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
                                        m.ListaAgencia = new List<Agencia>();
                                        //Pasamos la instancia de la clase cliente la cual contiene una lista de agencias para poder obtener los valores de allí.
                                        //Le pasamos un objeto tipo DataTable de tipo lista para recibir listas de tipo string y poder asignarles el valor de la consulta
                                        m.ListaAgencia = con.DataTableToList<Agencia>(lista.Copy().Rows.Cast<DataRow>()
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

                                    respuesta.Error = new Errores { codigo = "008", descripcion = "Se ejecutó correctamente la consulta" };

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

                            respuesta.Error = new Errores { descripcion = e.Message };
                        }
                    }
                }

                else
                {
                    respuesta.Error = new Errores { codigo = "999", descripcion = "El usuario no existe" };//El usuario no existe
                }//else
            }
            catch (Exception ex)
            {
                respuesta.Error = new Errores { descripcion = ex.Message };
            }

            return respuesta;
        }

       
        //public CarteraRespTotal RespCarteraTotal(ObtCarteraTotal obtCarteraTotal)
        //{
        //    ConexionBD con = new ConexionBD();
        //    ConexionBD Conex = new ConexionBD();
        //    CarteraRespTotal respuesta = new CarteraRespTotal();
        //    respuesta.Error = null;
        //    if (obtCarteraTotal.usuario == null)
        //        respuesta.Error = new Errores { codigo = "user_002", descripcion = "¡todas las variables del usuario no pueden ser nulas!" };
        //    if (obtCarteraTotal.usuario.UserName == null || string.IsNullOrWhiteSpace(obtCarteraTotal.usuario.UserName))
        //        respuesta.Error = new Errores { codigo = "user_003", descripcion = "¡el username no puede ser nulo o vacío!" };
        //    else if (obtCarteraTotal.usuario.Password == null || string.IsNullOrWhiteSpace(obtCarteraTotal.usuario.Password))
        //        respuesta.Error = new Errores { codigo = "user_003", descripcion = "¡el password no puede ser nulo o vacío!" };
        //    else if (obtCarteraTotal.NitCliente == null || string.IsNullOrWhiteSpace(obtCarteraTotal.NitCliente))
        //        respuesta.Error = new Errores { codigo = "clien_001", descripcion = "¡el nitcliente no puede ser nulo o vacío!" };

        //    try {
                
        //        DataSet Tablainfo = new DataSet();
        //        con.setConnection("SyscomDBSYSCOMSOPORTE");
        //        Conex.setConnection("SyscomDBSAL");
        //        List<SqlParameter> parametros = new List<SqlParameter>();

        //        parametros.Add(new SqlParameter("@NitCliente", obtCarteraTotal.NitCliente));
        //        con.addParametersProc(parametros);

        //        //Ejecuta procedimiento almacenado
        //        DataTable DT = new DataTable();
        //        con.resetQuery();
                

        //    } catch {


        //    }

        //    return respuesta;
        //}


    }   
}
