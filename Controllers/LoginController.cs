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
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = _dataContext.GetConnection())
                    {
                        connection.Open();

                        var command = new MySqlCommand(
                            "INSERT INTO clientes (nombre, direccion, telefono, dui, email) VALUES (@nombre,@direccion, @telefono, @dui, @email)",
                            connection
                        );

                        command.Parameters.AddWithValue("@nombre", nuevoCliente.Nombre);
                        command.Parameters.AddWithValue("@direccion", nuevoCliente.Direccion);
                        command.Parameters.AddWithValue("@telefono", nuevoCliente.Telefono);
                        command.Parameters.AddWithValue("@dui", nuevoCliente.DUI);
                        command.Parameters.AddWithValue("@email", nuevoCliente.Email);


                        command.ExecuteNonQuery();
                    }

                    // Redirige a la lista de alumnos después de agregar
                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error al agregar el cliente: {ex.Message}");
                }
            }
            return View(nuevoCliente);
        }
    }

}