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
        public RespCliente ConsultarClientes(ObtCliente cliente, UsuariosRequest datosUsuario, out List<ClienteResponse> dtCliente)
        {
            ConexionBD con = new ConexionBD();
            con.setConnection("Prod");

            dtCliente = new List<ClienteResponse>();
            RespCliente respuesta = new RespCliente();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@NitCliente", cliente.Cliente.NitCliente));
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