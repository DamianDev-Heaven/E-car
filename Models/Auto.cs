//using System.ComponentModel.DataAnnotations;

//namespace ProyectoFinalTecnicas.Models
//{
//    public class Auto
//    {
//        [Key]
//        public int IdAuto { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Marca { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Modelo { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Placa { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Tipo { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Estado { get; set; }

//        [Required]
//        public double Price { get; set; }

//        [StringLength(255)]
//        public string Imagen { get; set; }
//    }
//}
//sirveeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.AspNetCore.Http;

//namespace ProyectoFinalTecnicas.Models
//{
//    public class Auto
//    {
//        [Key]
//        public int IdAuto { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Marca { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Modelo { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Placa { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Tipo { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Estado { get; set; }

//        [Required]
//        public double Price { get; set; }

//        public byte[] Imagen { get; set; } // Almacena la imagen en formato BLOB

//        [NotMapped]
//        public string ImagenBase64 => Imagen != null ? Convert.ToBase64String(Imagen) : null; // Almacena la imagen en formato Base64 para la vista
//    }
//}
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalTecnicas.Models
{
    public class Auto
    {
        [Key]
        public int IdAuto { get; set; }

        [Required]
        [StringLength(100)]
        public string Marca { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(50)]
        public string Placa { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "El estado debe ser 0 (inactivo) o 1 (activo).")]
        public int Estado { get; set; }

        [Required]
        public double Price { get; set; }

        // Guardamos la ruta de la imagen
        [StringLength(255)]
        public string Imagen { get; set; }
    }
}


