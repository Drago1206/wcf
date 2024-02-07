using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfPedidos40.Models;
using WcfPruebas40.Models;

namespace WcfPedidos40
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : Pedidos40
    {


        public CarteraResp RespCartera(CarteraReq ReqCartera)
        {
            //Instanciamo la conexion
            ConexionDB con = new ConexionDB();

            //Instanciamos la clase de CarteraResp para poder ingresar los resultados en dicha clase
            CarteraResp respuesta = new CarteraResp();

            try
            {   //Realizamos un try para realizar todas las validaciones , incluyendo el usuario , contraseña y nit del cliente
                respuesta.Error = null;
                if (ReqCartera.usuario == null)
                {
                    respuesta.Error = new Errores { codigo = "user_002", descripcion = "¡todas las variables del usuario no pueden ser nulas!" };
                }
                else
                {
                    ExisteUsuario ExistUsu = new ExisteUsuario();
                    if (ExistUsu.Existe(ReqCartera.usuario.UserName, ReqCartera.usuario.Password, out string[] mensajeNuevo))
                    {
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
                                if (con.ejecutarQuery("WcfPedidos_ConsultarCartera", parametros, out Tablainfo, out string[] nuevoMennsaje, CommandType.StoredProcedure))
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
                                respuesta.Error = new Errores { descripcion = e.Message };
                            }
                        }
                    }
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
