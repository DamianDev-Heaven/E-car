using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;

namespace ProyectoFinalTecnicas.Controllers
{
    public class AlquilerController : Controller
    {
        private readonly DataContext _dataContext;

        public AlquilerController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult MostrarAlquiler()
        {
            List<Alquiler> alquileres = new List<Alquiler>();

            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM alquilados";
                    var command = new MySqlCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        alquileres.Add(new Alquiler
                        {
                            IdAlquiler = reader.GetInt32("id_alquiler"),
                            IdAuto = reader.GetInt32("id_auto"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            IdEmpleado = reader.GetInt32("id_empleado"),
                            IdFactura = reader.GetInt32("id_factura"),
                            Fecha = reader.GetDateTime("fecha"),
                            FechaDevolver = reader.GetDateTime("fecha_devolver"),
                            Devuelto = reader.GetBoolean("devuelto"),
                            FechaDevolucionReal = reader.IsDBNull(reader.GetOrdinal("fecha_devolucion_real")) ? (DateTime?)null : reader.GetDateTime("fecha_devolucion_real"),
                            Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                        });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al obtener los alquileres: " + ex.Message;
                }
            }

            return View(alquileres);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    // Query para eliminar el alquiler con el id proporcionado
                    string query = "DELETE FROM alquilados WHERE id_alquiler = @id";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Alquiler eliminado exitosamente.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se pudo eliminar el alquiler. Intenta nuevamente.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al eliminar el alquiler: " + ex.Message;
                }
            }

            // Redirige a la vista de todos los alquileres después de la eliminación
            return RedirectToAction("MostrarAlquiler");
        }


        public IActionResult EditAlquiler(int id)
        {
            Alquiler alquiler = null;

            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM alquilados WHERE id_alquiler = @id";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        alquiler = new Alquiler
                        {
                            IdAlquiler = reader.GetInt32("id_alquiler"),
                            IdAuto = reader.GetInt32("id_auto"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            IdEmpleado = reader.GetInt32("id_empleado"),
                            IdFactura = reader.GetInt32("id_factura"),
                            Fecha = reader.GetDateTime("fecha"),
                            FechaDevolver = reader.GetDateTime("fecha_devolver"),
                            Devuelto = reader.GetBoolean("devuelto"),
                            FechaDevolucionReal = reader.IsDBNull(reader.GetOrdinal("fecha_devolucion_real")) ? (DateTime?)null : reader.GetDateTime("fecha_devolucion_real"),
                            Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
                        };
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al obtener el alquiler: " + ex.Message;
                }
            }

            if (alquiler == null)
            {
                return NotFound();
            }

            return View(alquiler);
        }

        [HttpPost]
        public IActionResult EditAlquiler(Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                using (var connection = _dataContext.GetConnection())
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE alquilados SET id_auto = @id_auto, id_cliente = @id_cliente, id_empleado = @id_empleado, id_factura = @id_factura, fecha_devolver = @fecha_devolver, devuelto = @devuelto, fecha_devolucion_real = @fecha_devolucion_real, observaciones = @observaciones WHERE id_alquiler = @id_alquiler";
                        var command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@id_alquiler", alquiler.IdAlquiler);
                        command.Parameters.AddWithValue("@id_auto", alquiler.IdAuto);
                        command.Parameters.AddWithValue("@id_cliente", alquiler.IdCliente);
                        command.Parameters.AddWithValue("@id_empleado", alquiler.IdEmpleado);
                        command.Parameters.AddWithValue("@id_factura", alquiler.IdFactura);
                        command.Parameters.AddWithValue("@fecha_devolver", alquiler.FechaDevolver);
                        command.Parameters.AddWithValue("@devuelto", alquiler.Devuelto);
                        command.Parameters.AddWithValue("@fecha_devolucion_real", alquiler.FechaDevolucionReal ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(alquiler.Observaciones) ? "" : alquiler.Observaciones);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            TempData["SuccessMessage"] = "Alquiler actualizado exitosamente";
                            return RedirectToAction("MostrarAlquiler");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "No se pudo actualizar el alquiler. Intenta nuevamente.";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "Error al actualizar el alquiler: " + ex.Message;
                    }
                }
            }

            return View(alquiler);
        }
    
    public IActionResult CreateAlquiler()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAlquiler(Alquiler alquiler)
        {
            if (ModelState.IsValid) // Verifica que el modelo es válido
            {
                try
                {
                    using (var connection = _dataContext.GetConnection())
                    {
                        connection.Open();
                        string query = "INSERT INTO alquilados (id_auto, id_cliente, id_empleado, id_factura, fecha, fecha_devolver, devuelto, observaciones) " +
                                       "VALUES (@id_auto, @id_cliente, @id_empleado, @id_factura, @fecha, @fecha_devolver, @devuelto, @observaciones)";
                        var command = new MySqlCommand(query, connection);

                        command.Parameters.AddWithValue("@id_auto", alquiler.IdAuto);
                        command.Parameters.AddWithValue("@id_cliente", alquiler.IdCliente);
                        command.Parameters.AddWithValue("@id_empleado", alquiler.IdEmpleado);
                        command.Parameters.AddWithValue("@id_factura", alquiler.IdFactura);
                        command.Parameters.AddWithValue("@fecha", alquiler.Fecha);
                        command.Parameters.AddWithValue("@fecha_devolver", alquiler.FechaDevolver);
                        command.Parameters.AddWithValue("@devuelto", alquiler.Devuelto);
                        command.Parameters.AddWithValue("@observaciones", alquiler.Observaciones);

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["SuccessMessage"] = "Alquiler registrado exitosamente";
                            return RedirectToAction("MostrarAlquiler");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "No se pudo registrar el alquiler. Intenta nuevamente. Filas afectadas: " + result;
                        }

                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al registrar el alquiler: " + ex.Message;
                }
            }

            return View(alquiler); // Si hay algún error o el modelo no es válido, vuelve a mostrar el formulario
        }

        public IActionResult DevolverAuto(int id)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    // Actualizar el alquiler como devuelto y asignar la fecha de devolución real
                    string query = "UPDATE alquilados SET devuelto = @devuelto, fecha_devolucion_real = @fecha_devolucion_real WHERE id_alquiler = @id_alquiler";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_alquiler", id);
                    command.Parameters.AddWithValue("@devuelto", true);
                    command.Parameters.AddWithValue("@fecha_devolucion_real", DateTime.Now);

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al devolver el alquiler: " + ex.Message;
                }
            }

            TempData["SuccessMessage"] = "Alquiler devuelto exitosamente.";
            return RedirectToAction("MostrarAlquiler");
        }


    }
}
