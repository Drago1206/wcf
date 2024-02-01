using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfPedidos30.Model
{

    [DataContract]
    public class ClienteResponse
    {
        public string mNitCliente { get; set; }
        public string mNomCliente { get; set; }
        public string mDireccion { get; set; }
        public string mCiudad { get; set; }
        public string mTelefono { get; set; }
        public int mNumLista { get; set; }
        public string mNitVendedor { get; set; }
        public string mNomVendedor { get; set; }
        public List<Agencia> mListaAgencias { get; set; }

        [DataMember]
        public string NitCliente { get { return mNitCliente; } set { mNitCliente = value; } }
        [DataMember]
        public string Ciudad { get { return mCiudad; } set { mCiudad = value; } }
        [DataMember]
        public string Direccion { get { return mDireccion; } set { mDireccion = value; } }
        [DataMember]
        public List<Agencia> ListaAgencias { get { return mListaAgencias; } set { mListaAgencias = value; } }
        [DataMember]
        public string NitVendedor { get { return mNitVendedor; } set { mNitVendedor = value; } }
        [DataMember]
        public string NomVendedor { get { return mNomVendedor; } set { mNomVendedor = value; } }
        [DataMember]
        public string NombreCliente { get { return mNomCliente; } set { mNomCliente = value; } }
        [DataMember]
        public int NumLista { get { return mNumLista; } set { mNumLista = value; } }
        [DataMember]
        public string Telefono { get { return mTelefono; } set { mTelefono = value; } }

    }
    public class ClienteRequest
    {
        [DataMember]
        public string NitCliente { get; set; }
    }
}