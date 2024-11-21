namespace ProyectoFinalTecnicas.Models
{
    public class Alquiler
    {
        public int IdAlquiler { get; set; } // Identificador único del alquiler
        public int IdAuto { get; set; } // Identificador del auto alquilado
        public int IdCliente { get; set; } // Identificador del cliente
        public int IdEmpleado { get; set; } // Identificador del empleado que realizó la gestión
        public int IdFactura { get; set; } // Identificador de la factura asociada
        public DateTime Fecha { get; set; } // Fecha en que se realizó el alquiler
        public DateTime FechaDevolver { get; set; } 
    }
}
