using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Models
{
    public class Usuario
    {
        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string UserName { get; set; }
    }
    [DataContract]
    public class UsuariosRequest
    {
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [DataMember]
        public string Password { get; set; }
    }
}