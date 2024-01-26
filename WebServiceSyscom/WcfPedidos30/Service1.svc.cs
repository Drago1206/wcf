using System;
using System.Collections.Generic;
using System.Linq;
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

        #region ObtenerProducto 

        [return: MessageParameter(Name = "Producto")]
        public RespProducto ConProducto(ObtProducto obtProducto)
        {
            RespProducto respuesta = new RespProducto();
            respuesta.Registro = new Log();
            
            if (obtProducto.Usuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }
            
            else if (string.IsNullOrWhiteSpace(obtProducto.Usuario.UserName) || string.IsNullOrWhiteSpace(obtProducto.Usuario.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                /*
                  */
                ExisteUsuario usuario = new ExisteUsuario();
                if(usuario.Existe(obtProducto.Usuario.UserName, obtProducto.Usuario.Password, out string[] mensajeNuevo))
                {
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

            else if (string.IsNullOrWhiteSpace(obtCliente.Usuario.UserName) || string.IsNullOrWhiteSpace(obtCliente.Usuario.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                /*
                  */
                ExisteUsuario usuario = new ExisteUsuario();
                if (usuario.Existe(obtCliente.Usuario.UserName, obtCliente.Usuario.Password, out string[] mensajeNuevo))
                {
                    respuesta.Registro = new Log { Codigo = "999", Descripcion = "Ok" };

                }
            }
            return respuesta;
        }

        [return: MessageParameter(Name = "Usuario")]
        public RespUsuario ObjUsuario(ObtUsuario obtUsuario)
        {
            RespUsuario respuesta = new RespUsuario();
            respuesta.Registro = new Log();

            if (obtUsuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Dato no válido" };
            }

            else if (string.IsNullOrWhiteSpace(obtUsuario.UserName) || string.IsNullOrWhiteSpace(obtUsuario.Password))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                /*
                  */
                ExisteUsuario usuario = new ExisteUsuario();
                if (usuario.Existe(obtUsuario.UserName, obtUsuario.Password, out string[] mensajeNuevo))
                {
                    respuesta.Registro = new Log { Codigo = "999", Descripcion = "Ok" };

                }
            }
            return respuesta;
        }
    }
}
