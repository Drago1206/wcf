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
            /// Se agrega el parámetro necesario para la consulta del cliente
            parametros.Add(new SqlParameter("@NitCliente", cliente.Cliente.NitCliente));

            /// Se inicializa el dataset para capturar la tabla del resultado del procedimiento de almacenado
            DataSet TablaClientes = new DataSet();

            if (con.ejecutarQuery("WSPedidosObtenerClientes", parametros, out TablaClientes, out string[] mensaje, CommandType.StoredProcedure))
            {
                DataTable dtClientes = TablaClientes.Tables[0];
                int TotalRegistros = TablaClientes.Tables[0].Rows.Count;
                if (TotalRegistros > 0)
                {

                    dtCliente = con.DataTableToList<ClienteResponse>("NitCliente,NombreCliente,Direccion,Ciudad,Telefono,NumLista,NitVendedor,NomVendedor".Split(','), TablaClientes);
                    dtCliente.ForEach(m =>
                    {
                        m.ListaAgencias = new List<Agencia>();
                        m.ListaAgencias = m.ListaAgencias = con.DataTableToList<Agencia>(dtClientes.Copy().Rows.Cast<DataRow>().Where(r => r.Field<string>("NitCliente").Equals(m.NitCliente)).CopyToDataTable().AsDataView().ToTable(true, "CodAge,NomAge".Split(',')));
                    });

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

            return respuesta;
        }
    }
}