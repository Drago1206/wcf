using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCortesEDS.Model
{
    public class InfoVentas
    {
        public int Consecutivo { get; set; }
        public int ConsecutivoDetalle { get; set; }
        public string Prefijo { get; set; }
        public int Factura { get; set; }
        public int Equipo { get; set; }
        public int Cara { get; set; }
        public int Posicion { get; set; }
        public int Manguera { get; set; }
        public int Isla { get; set; }
        public string CodProducto { get; set; }
        public string Producto { get; set; }
        public float Cantidad { get; set; }
        public float ValorUnitario { get; set; }
        public float ValorTotal { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }
        public float LecturaVolumenInicial { get; set; }
        public float LecturaVolumenFinal { get; set; }
        public float LecturaDineroInicial { get; set; }
        public float LecturaDineroFinal { get; set; }
        public int IdTipoTransaccion { get; set; }
        public string TipoTransaccion { get; set; }
        public int IdFormaPago { get; set; }
        public string FormaPago { get; set; }
        public string Cuenta { get; set; }
        public string NIT { get; set; }
        public string Placa { get; set; }
        public string IdRom { get; set; }
        public int Kilometraje { get; set; }
        public string CedulaConductor { get; set; }
        public string NombreConductor { get; set; }
        public int Turno { get; set; }
        public string CedulaVendedor { get; set; }
        public string NombreVendedor { get; set; }
        public int Corte { get; set; }
        public string FEFechaFactura { get; set; }
        public string FENumeroFactura { get; set; }
        public string FETipoPersona { get; set; }
        public string FETipoDocumento { get; set; }
        public string FEDigitoVerificacion { get; set; }
        public string FENumeroDocumento { get; set; }
        public string FENombreCliente { get; set; }
        public string FEDireccion { get; set; }
        public string FETelefono { get; set; }
        public string FECorreo { get; set; }
        public string FECufe { get; set; }
        public string FEQr { get; set; }
        public int FacturaContingencia { get; set; }
        public float ValorFP { get; set; }



    }
    
}