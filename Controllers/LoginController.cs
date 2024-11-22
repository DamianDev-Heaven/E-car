using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;

namespace ProyectoFinalTecnicas.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext _dataContext;

        public LoginController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpGet]
        public IActionResult InicioSesion()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ValidarLogin(string email, string contrasena)
        {
            try
            {
                using (var connection = _dataContext.GetConnection())
                {
                    connection.Open();

                    // Verificar si el usuario es empleado
                    string queryEmpleado = "SELECT * FROM empleados WHERE email = @Email AND contrasena = @Contrasena";
                    using (var command = new MySqlCommand(queryEmpleado, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Contrasena", contrasena);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }

                    // Verificar si el usuario es cliente
                    string queryCliente = "SELECT * FROM clientes WHERE email = @Email AND contrasena = @Contrasena";
                    using (var command = new MySqlCommand(queryCliente, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Contrasena", contrasena);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return RedirectToAction("InicioCliente", "Home");
                            }
                        }
                    }
                }

                // Si no se encuentra al usuario en ninguna tabla
                TempData["Error"] = "Correo o contraseña incorrectos.";
                return RedirectToAction("InicioSesion");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                TempData["Error"] = $"Error al iniciar sesión: {ex.Message}";
                return RedirectToAction("InicioSesion");
            }
        }


        // POST: Procesar el formulario y agregar el alumno a la base de datos
        [HttpPost]
        public IActionResult Registrar(Cliente nuevoCliente)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión abierta exitosamente.");

                    // Consulta SQL para insertar un nuevo cliente
                    string query = @"
         INSERT INTO clientes (nombre, direccion, telefono, dui, email, contrasena) 
         VALUES (@Nombre, @Direccion, @Telefono, @DUI, @Email, @Contrasena)";

                    var command = new MySqlCommand(query, connection);

                    // Asignar valores a los parámetros de la consulta
                    command.Parameters.AddWithValue("@Nombre", nuevoCliente.Nombre);
                    command.Parameters.AddWithValue("@Direccion", nuevoCliente.Direccion);
                    command.Parameters.AddWithValue("@Telefono", nuevoCliente.Telefono);
                    command.Parameters.AddWithValue("@DUI", nuevoCliente.DUI);
                    command.Parameters.AddWithValue("@Email", nuevoCliente.Email);
                    command.Parameters.AddWithValue("@Contrasena", nuevoCliente.Contrasena);

                    // Ejecutar la consulta
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        // Redirigir en caso de éxito
                        TempData["SuccessMessage"] = "Cliente agregado exitosamente.";
                        return RedirectToAction("Registrar");
                    }
                    else
                    {
                        // En caso de que no se agregue el cliente
                        ViewBag.ErrorMessage = "No se pudo agregar el cliente. Intenta nuevamente.";
                        return View("Registrar", nuevoCliente); // Regresar a la vista con el modelo actual
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                    ViewBag.ErrorMessage = "Error al agregar el cliente: " + ex.Message;
                    return View("Registrar", nuevoCliente); // Regresar a la vista con el modelo actual
                }
            }
        }
    }

}