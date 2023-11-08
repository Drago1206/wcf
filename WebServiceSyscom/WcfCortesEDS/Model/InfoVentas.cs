using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfCortesEDS.Model
{
    public class InfoVentas
    {
    }
    public class Pago
    {
        public int IdFormaPago { get; set; }
        public string FormaPago { get; set; }
    }

    public class Productos
    {
        public string CodProducto { get; set; }
        public string Nombre { get; set; }
    }

    public class Empleados
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
    }


    public class ResultadoItem
    {

        public int IdRegistroVenta { get; set; }
        public string Consecutivo { get; set; }
        public string Prefijo { get; set; }
        public int CodSurtidor { get; set; }
        public int IdCara { get; set; }
        public int CodCara { get; set; }
        public int IdManguera { get; set; }
        public int IdIsla { get; set; }

        public float Cantidad { get; set; }
        public float Precio { get; set; }
        public float Valor { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public float LecturaInicial { get; set; }
        public float LecturaFinal { get; set; }
        public int Nit { get; set; }
        public string Placa { get; set; }
        public string ROM { get; set; }
        public string Kilometraje { get; set; }

        public int IdTurno { get; set; }



        public List<Pago> Pagos { get; set; }


        public Empleados Empleado { get; set; }

        public Productos Producto { get; set; }

    }

    public class Root
    {
        public List<ResultadoItem> Resultado { get; set; }
    }
}