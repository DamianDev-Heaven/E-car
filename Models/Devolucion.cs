namespace ProyectoFinalTecnicas.Models
{
    public class Devolucion
    {
        public int IdAlquiler { get; set; }
        public string NombreCliente { get; set; }
        public string NombreEmpleado { get; set; }
        public string Auto { get; set; } // Combinar Marca y Modelo
        public DateTime FechaRenta { get; set; }
        public DateTime FechaDevolver { get; set; }
        public int IdAuto { get; set; }
    }
}
