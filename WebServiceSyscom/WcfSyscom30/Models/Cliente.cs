using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfSyscom30.Models
{
    [DataContract]
    public class ClienteResponse
    {
        
        public string NitCliente { get; set; }
        public string CiudadCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string NombreCliente { get; set; }
        public string TelefonoCLiente { get; set; }
        public string NitVendedor { get; set; }
        public string NomVendedor { get; set; }
        List<ItemAgencia> Agencia { get; set; }

        [DataMember]
        public string _NitCliente { get { return NitCliente; } set { NitCliente = value; } }
        [DataMember]
        public string _CiudadCliente { get { return _CiudadCliente; } set { _CiudadCliente = value; } }
        [DataMember]
        public string _DireccionCliente { get { return _DireccionCliente; } set { _DireccionCliente = value; } }
        [DataMember]
        public string _NombreCliente { get { return _NombreCliente; } set { _NombreCliente = value; } }
        [DataMember]
        public string _TelefonoCLiente { get { return _TelefonoCLiente; } set { _TelefonoCLiente = value; } }
        [DataMember]
        public string _NitVendedor { get { return _NitVendedor; } set { _NitVendedor = value; } }
        [DataMember]
        public string _NomVendedor { get { return _NomVendedor; } set { _NomVendedor = value; } }
    }
    [DataContract]
    public class ClienteResquest
    {
        public string NitCliente { get; set; }
        public string CiudadCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string NombreCliente { get; set; }
        public string TelefonoCLiente { get; set; }
        public string NitVendedor { get; set; }
        public string NomVendedor { get; set; }
        List<ItemAgencia> Agencia { get; set; }

        [DataMember]
        public string _NitCliente { get { return NitCliente; } set { NitCliente = value; } }
        [DataMember]
        public string _CiudadCliente { get { return _CiudadCliente; } set { _CiudadCliente = value; } }
        [DataMember]
        public string _DireccionCliente { get { return _DireccionCliente; } set { _DireccionCliente = value; } }
        [DataMember]
        public string _NombreCliente { get { return _NombreCliente; } set { _NombreCliente = value; } }
        [DataMember]
        public string _TelefonoCLiente { get { return _TelefonoCLiente; } set { _TelefonoCLiente = value; } }
        [DataMember]
        public string _NitVendedor { get { return _NitVendedor; } set { _NitVendedor = value; } }
        [DataMember]
        public string _NomVendedor { get { return _NomVendedor; } set { _NomVendedor = value; } }
    }
}