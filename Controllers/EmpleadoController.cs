using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;

namespace ProyectoFinalTecnicas.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly DataContext _dataContext;

        public EmpleadoController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            List<Empleado> empleados = new List<Empleado>();

            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM empleados";
                    var command = new MySqlCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        empleados.Add(new Empleado
                        {
                            IdEmpleado = reader.GetInt32("id_empleado"),
                            Nombre = reader.GetString("nombre"),
                            Telefono = reader.GetString("telefono"),
                            Cargo = reader.GetString("cargo"),
                            Email = reader.GetString("email"),
                            Contrasena = reader.GetString("contrasena") // Agregar contraseña
                        });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al obtener los empleados: " + ex.Message;
                }
            }

            return View(empleados);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM empleados WHERE id_empleado = @id_empleado";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_empleado", id);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Empleado eliminado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se pudo eliminar el empleado. Intenta nuevamente.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al eliminar el empleado: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Empleado empleado = null;

            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM empleados WHERE id_empleado = @id";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        empleado = new Empleado
                        {
                            IdEmpleado = reader.GetInt32("id_empleado"),
                            Nombre = reader.GetString("nombre"),
                            Telefono = reader.GetString("telefono"),
                            Cargo = reader.GetString("cargo"),
                            Email = reader.GetString("email"),
                            Contrasena = reader.GetString("contrasena") // Agregar contraseña
                        };
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al obtener el empleado: " + ex.Message;
                }
            }

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        [HttpPost]
        public IActionResult Edit(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                using (var connection = _dataContext.GetConnection())
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE empleados SET nombre = @nombre, telefono = @telefono, cargo = @cargo, email = @email, contrasena = @contrasena WHERE id_empleado = @id_empleado";
                        var command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@id_empleado", empleado.IdEmpleado);
                        command.Parameters.AddWithValue("@nombre", empleado.Nombre);
                        command.Parameters.AddWithValue("@telefono", empleado.Telefono);
                        command.Parameters.AddWithValue("@cargo", empleado.Cargo);
                        command.Parameters.AddWithValue("@email", empleado.Email);
                        command.Parameters.AddWithValue("@contrasena", empleado.Contrasena);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            TempData["SuccessMessage"] = "Empleado actualizado exitosamente";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "No se pudo actualizar el empleado. Intenta nuevamente.";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "Error al actualizar el empleado: " + ex.Message;
                    }
                }
            }

            return View(empleado);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = _dataContext.GetConnection())
                    {
                        connection.Open();
                        string query = "INSERT INTO empleados (nombre, telefono, cargo, email, contrasena) " +
                                       "VALUES (@nombre, @telefono, @cargo, @email, @contrasena)";
                        var command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@nombre", empleado.Nombre);
                        command.Parameters.AddWithValue("@telefono", empleado.Telefono);
                        command.Parameters.AddWithValue("@cargo", empleado.Cargo);
                        command.Parameters.AddWithValue("@email", empleado.Email);
                        command.Parameters.AddWithValue("@contrasena", empleado.Contrasena); // Asegúrate de incluir la contraseña.

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            TempData["SuccessMessage"] = "Empleado agregado exitosamente";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "No se pudo agregar el empleado. Intenta nuevamente.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al agregar el empleado: " + ex.Message;
                }
            }

            return View(empleado);
        }

    }
}
