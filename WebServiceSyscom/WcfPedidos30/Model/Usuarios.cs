using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{
    [DataContract]
    public class UsuariosRequest
    {
        /// <summary>
        /// Nombre del usuario para acceder a SYSCOM
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// Contraseña del usuario para acceder a SYSCOM
        /// </summary>
        [DataMember]
        public string Password { get; set; }
    }

    [DataContract]
    public class UsuariosResponse
    {
        /// <summary>
        /// Bodega asociada al usuario sobre la cual se registrar el pedido. 
        /// </summary>
        [DataMember]
        public string Bodega { get; set; }
        /// <summary>
        /// Nombre de la compañía asociada al usuario sobre la cual se registrará el pedido
        /// </summary>
        [DataMember]
        public string Compañía { get; set; }
        /// <summary>
        /// Indica si el usuario está registrado como cliente
        /// </summary>
        [DataMember]
        public bool EsCliente { get; set; }
        /// <summary>
        /// Indicar si el usuario registrado esta creado en terceros como vendedor
        /// </summary>
        [DataMember]
        public bool Esvendedor { get; set; }
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        [DataMember]
        public string IdUsuario { get; set; }
        /// <summary>
        /// Nombre registrado en terceros
        /// </summary>
        [DataMember]
        public string NombreTercero { get; set; }
    }
}