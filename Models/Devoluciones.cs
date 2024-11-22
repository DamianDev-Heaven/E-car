namespace ProyectoFinalTecnicas.Models
{
    public class Devolucion
    {
        public int IdAlquiler { get; set; }
        public int IdAuto { get; set; }
        public int IdCliente { get; set; }
        public int IdEmpleado { get; set; }
        public int IdFactura { get; set; }
        public DateTime FechaRenta { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Observaciones { get; set; }
        public bool Devuelto { get; set; }

        // Agregamos estos campos para los datos relacionados
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string ClienteNombre { get; set; }
        public string EmpleadoNombre { get; set; }
    }
}
