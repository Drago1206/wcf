using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using WcfCortesEDS.Model;

namespace WcfCortesEDS
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        string NomProyecto = "CortesEDS" + "-" + ConfigurationManager.AppSettings["NitEmpresa"];

        public RespToken generarToken(Usuarios usuario)
        {
            RespToken respuesta = new RespToken();
            respuesta.Registro = new Log();

            if (usuario == null)
            {
                respuesta.Registro = new Log { Codigo = "000", Descripcion = "Datos no valido" };
            }
            else if (string.IsNullOrWhiteSpace(usuario.Usuario) || string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                respuesta.Registro = new Log { Codigo = "001", Descripcion = "Parámetro 'Usuario/Contraseña', NO pueden ser nulos" };
            }
            else
            {

                GenerarToken token = new GenerarToken();
                respuesta.Token = token.nuevoToken(NomProyecto, usuario.Usuario, usuario.Contraseña, out string[] mensajeNuevo);
                respuesta.Registro = new Log { Codigo = mensajeNuevo[0], Descripcion = mensajeNuevo[1] };
            }
            return respuesta;

        }

       
    }
}
