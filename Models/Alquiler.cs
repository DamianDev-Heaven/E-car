using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalTecnicas.Models
{
    public class Alquiler
    {
        public int IdAlquiler { get; set; }
        public int IdAuto { get; set; }
        public int IdCliente { get; set; }
        public int IdEmpleado { get; set; }
        public int IdFactura { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaDevolver { get; set; }
        public bool Devuelto { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        [Required(ErrorMessage = "El campo Observaciones es obligatorio.")]
        public string Observaciones { get; set; } = string.Empty;
    }
}
