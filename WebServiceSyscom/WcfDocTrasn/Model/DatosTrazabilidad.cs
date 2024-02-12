using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfDocTrasn.Model
{
    public class DatosTrazabilidad
    {
        public string TipoDocumento { get; set; }
        public int Numero { get; set; }
        public string Compania { get; set; }
        public DateTime Fecha { get; set; }
        public int Valor { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string ClaseVeh { get; set; }
        public string IdConductor { get; set; }
        public string NomConductor { get; set; }
        public string GPSoperador { get; set; }
        public string GPSUsuario { get; set; }
        public string GPSClave { get; set; }
        public List<itemnovedad> novedad { get; set; }
        public string FechaInicioCargue { get; set; }
        public string HoraInicioCargue { get; set; }
        public string FechaFinCargue { get; set; }
        public string HoraFinCargue { get; set; }
        public string FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public string Precinto { get; set; }
    }
}