using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
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
                if(usuario.Existe(obtProducto.Usuarios.UserName, obtProducto.Usuarios.Password, out string[] mensajeNuevo))
                {
                    PaginadorProducto<ProductosResponse> DatProducto = new PaginadorProducto<ProductosResponse>();

                    respuesta = consProd.ConsultarProductos(obtProducto.DatosProducto, obtProducto.Usuarios,out DatProducto);
                    respuesta.ListaProductos = DatProducto;
                    respuesta.Registro = new Log { Codigo = "999", Descripcion = "Ok" };

                }
            }
            return respuesta;
        }
        #endregion

        [return: MessageParameter(Name = "Cliente")]
        public RespCliente ObjCliente(ObtCliente obtCliente)
        {
            RespCliente respuesta = new RespCliente();
            respuesta.Registro = new Log();

            if (obtCliente.Usuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }

            else if (string.IsNullOrWhiteSpace(obtCliente.Usuario.Usuarios.Password) || string.IsNullOrWhiteSpace(obtCliente.Usuario.Usuarios.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                /*
                  */
                ExisteUsuario usuario = new ExisteUsuario();
                if (usuario.Existe(obtCliente.Usuario.Usuarios.UserName, obtCliente.Usuario.Usuarios.UserName, out string[] mensajeNuevo))
                {


                    respuesta.Registro = new Log { Codigo = "999", Descripcion = "Ok" };

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
    }
}
