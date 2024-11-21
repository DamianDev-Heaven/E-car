namespace ProyectoFinalTecnicas.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int id_cliente { get; set; }
        public int id_empleado { get; set; }
        public int id_auto { get; set; }
        public DateTime Fecha { get; set; }
        public double Subtotal { get; set; }
        public double Iva { get; set; }
        public double Total { get; set; }
    }
}
