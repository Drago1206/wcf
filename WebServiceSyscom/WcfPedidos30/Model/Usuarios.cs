using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfPedidos30.Model
{
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

    [DataContract]
    public class UsuariosResponse
    {
        [DataMember]
        public string Bodega { get; set; }
       [DataMember]
        public string Compania { get; set; }
        [DataMember]
        public bool EsCliente { get; set; }
        [DataMember]
        public bool EsVendedor { get; set; }
        [DataMember]
        public string IdUsuario { get; set; }
        [DataMember]
        public string NombreTercero { get; set; }
    }
}