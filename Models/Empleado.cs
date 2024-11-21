using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalTecnicas.Models
{
    public class Empleado
    {
        [Required(ErrorMessage = "El ID del empleado es obligatorio.")]
        public int IdEmpleado { get; set; } // Identificador único del empleado

        [Required(ErrorMessage = "El nombre del empleado es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } // Nombre del empleado

        [Required(ErrorMessage = "El teléfono del empleado es obligatorio.")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        public string Telefono { get; set; } // Teléfono de contacto del empleado

        [Required(ErrorMessage = "El cargo del empleado es obligatorio.")]
        [StringLength(50, ErrorMessage = "El cargo no puede exceder los 50 caracteres.")]
        public string Cargo { get; set; } // Cargo o puesto del empleado

        [Required(ErrorMessage = "El correo electrónico del empleado es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; } // Dirección de correo electrónico del empleado

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "La contraseña debe tener entre 5 y 50 caracteres.")]
        public string Contrasena { get; set; } // Contraseña del empleado
    }
}
