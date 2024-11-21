using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalTecnicas.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(200)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(10)]
        public string DUI { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
