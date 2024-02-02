using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WcfPedidos30.Model
{
    public class ConsultarCliente
    {

        /// <summary>
        /// Método que consulta la información del cliente por medio de un 
        /// procedimiento de almacenado --WSPedidoObtenerClientes--
        /// </summary>
        /// <param name="cliente">Objeto con los parámetros de solicitud del cliente</param>
        /// <param name="datosUsuario">Paramétros para la verificación del usuario</param>
        /// <param name="dtCliente">Lista que contiene los datos de respuesta del cliente</param>
        /// <returns>Retorna un objeto con los datos de respuesta del cliente</returns>
        public RespCliente ConsultarClientes(ObtCliente cliente, UsuariosRequest datosUsuario, out List<ClienteResponse> dtCliente)
        {
            ///Se inicializa la conexión con la base de datos
            ConexionBD con = new ConexionBD();
            ///Se define la conexión a la base de datos en base al nombre de la cadena de conexion
            con.setConnection("Prod");
            /// Se declara una lista vacía
            dtCliente = new List<ClienteResponse>();
            /// Se inicializa el objeto de respuesta a la solicitud 
            RespCliente respuesta = new RespCliente();
            /// Se inicializa una lista vacía para pasar los parámetros al procedimiento de almacenado
            List<SqlParameter> parametros = new List<SqlParameter>();
            /// Se agrega el parámetro necesario para ejecutar el procedimiento de almacenado, con el dato del cliente
            parametros.Add(new SqlParameter("@NitCliente", cliente.Cliente.NitCliente));

            /// Se inicializa el dataset para capturar la tabla del resultado del procedimiento de almacenado
            DataSet TablaClientes = new DataSet();

            ///Condición que verifica si la consulta por medio del procedimiento de almacenado se efectuó correctamente
            if (con.ejecutarQuery("WSPedidosObtenerClientes", parametros, out TablaClientes, out string[] mensaje, CommandType.StoredProcedure))
            {
                /// Se inicializa la tabla de datos proveniente de la variable TablaClientes de salida del método ejecutarQuery 
                DataTable dtClientes = TablaClientes.Tables[0];
                /// Se inicializa la variable TotalRegistros con el valor de la cantidad de registros totales del procedimiento de almacenado
                int TotalRegistros = TablaClientes.Tables[0].Rows.Count;

                ///Condición que verifica si la cantidad de registros provenientes del procedimiento de almacenado es mayor a cero
                if (TotalRegistros > 0)
                {
                    // Se le asigna el valor a la variable dtCliente con la lista que retorna el método DataTableToList
                    dtCliente = con.DataTableToList<ClienteResponse>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','), TablaClientes);
                    // Se recorre la lista dtCliente
                    dtCliente.ForEach(m =>
                    {
                        /// Se inicializa una lista dentro de la lista dtCliente
                        m.ListaAgencias = new List<Agencia>();
                        /// Se asigna valor a la ListaAgencias con la lista que retorna el método DataTableToList
                        m.ListaAgencias = con.DataTableToList<Agencia>(dtClientes.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente)).CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                    });

                    /// Se inicializa un array de strings para pasar los mensajes de respuesta
                    mensaje = new string[2];
                    mensaje[0] = "012";
                    mensaje[1] = "Se ejecutó correctamente la consulta.";  
                }
            }
            else
            {
                mensaje = new string[2];
                mensaje[0] = "014";
                mensaje[1] = "No se encuentran productos disponibles";
            }

            /// Se retorna el cúmulo de datos del proceso en la variable respuesta
            return respuesta;
        }
    }
}