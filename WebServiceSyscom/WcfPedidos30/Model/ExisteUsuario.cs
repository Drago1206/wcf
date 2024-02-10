using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace WcfPedidos30.Model
{
    public class ExisteUsuario
    {
        ConexionBD con = new ConexionBD();

        /// <summary>
        /// Verifica si el usuario existe.
        /// </summary>
        /// <param name="usuario">Usuario.</param>
        /// <param name="clave">contraseña.</param>
        /// <param name="mensaje">El mensaje en posicion 0 esta el codigo y en la posicion 1 la descripcion.</param>
        /// <returns></returns>
        public bool Existe(string usuario, string password, out string[] mensaje)
        {
            mensaje = null;

            bool existe = false;
            /// Condición que verifica si el usuario que se está recibiendo como parámetro es diferente de nulo 
            if (usuario != null)
            {
                /// Configuración de la cadena de conexión para determinar a qué base de datos va dirigida la consulta
                con.setConnection("DBSYSCOMSOPORTE");

                /// Se inicializa el DataSet que contendrá la respuesta del procedimiento de almacenado
                DataSet TablaIncio = new DataSet();
                /// Se inicializa una lista de parámetros para enviárselos al procedimiento de almacenado
                List<SqlParameter> parametros = new List<SqlParameter>();
                /// Se ajustan los parámetros que recibirá el procedimiento de almacenado
                parametros.Add(new SqlParameter("@Usuario", usuario));

                /// Condición para verificar si el procedimiento de almacenado se ejecuta correctamente
                if (con.ejecutarQuery("WSPedidosInicioSeccion", parametros, out TablaIncio, out string[] nuevoMennsaje, CommandType.StoredProcedure))
                {
                    /// Condición para verificar si la cantidad de registros recibidos son mayor a cero
                    if (TablaIncio.Tables[0].Rows.Count > 0)
                    {

                        /// Se define la variable que contendrá la contraseña del usuario encriptada
                        pwdSyscom pwd = new pwdSyscom(TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));
                        /// Se decodifica la contraseña y se guarda en la lista que contiene las contraseña
                        pwd.Decodificar(TablaIncio.Tables[0].AsEnumerable().FirstOrDefault().Field<string>("PwdLog"));

                        /// Condición que verifica si la la contraseña recibida en la solicitud 
                        /// Es igual a la contraseña decodificada de la base de datos
                        string cadena = pwd.contrasenna;
                        int indice = cadena.LastIndexOf("=");
                        string contrasena;
                        if (indice != -1)
                        {
                            contrasena = cadena.Substring(indice + 1);
                            if (password.ToLower() == contrasena)
                            {
                                /// Si la condición se cumple, 
                                /// Define mensajes de respuesta existoso,
                                /// y define la variable como true
                                mensaje = new string[2];
                                mensaje[0] = "USER_064";
                                mensaje[1] = "Respuesta exitosa";
                                existe = true;
                            }
                            else
                            {
                                /// En caso de que la condición no se cumpla,
                                /// Define mensajes de respuesta negativo
                                mensaje = new string[2];
                                mensaje[0] = "USER_003";
                                mensaje[1] = "Usuario o Contraseña inválido";
                            }
                        }
                        
                    }
                    else
                    {
                        mensaje = new string[2];
                        mensaje[0] = "USER_001";
                        mensaje[1] = "¡Usuario no encontrado!";

                    }
                }
                else
                {
                    mensaje = new string[2];
                    mensaje[0] = nuevoMennsaje[0];
                    mensaje[1] = nuevoMennsaje[1];
                }
            }
            return existe;
        }

    }
}