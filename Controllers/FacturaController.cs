using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;
using System;

namespace ProyectoFinalTecnicas.Controllers
{
    public class FacturaController : Controller
    {
        private readonly DataContext _dataContext;

        public FacturaController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Detalles(string placa)
        {
            Auto auto = null;

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM autos WHERE placa = @placa";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@placa", placa);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        auto = new Auto
                        {
                            IdAuto = reader.GetInt32("id_auto"),
                            Marca = reader.GetString("marca"),
                            Modelo = reader.GetString("modelo"),
                            Placa = reader.GetString("placa"),
                            Tipo = reader.GetString("tipo"),
                            Estado = reader.GetInt32("estado"),
                            Price = reader.GetDouble("costo_dia"),
                            Imagen = reader["imagen"] as string
                        };
                    }
                }
            }

            if (auto == null)
            {
                return NotFound();
            }

            return View(auto);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult GenerarFactura(Factura factura, int dias)
        {
            if (ModelState.IsValid)
            {
                using (var connection = _dataContext.GetConnection())
                {
                    connection.Open();

                    DateTime fechaDevolucion = factura.Fecha.AddDays(dias);

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string queryFactura = @"INSERT INTO facturas (id_cliente, id_auto, id_empleado, fecha, subtotal, iva, total)
                                    VALUES (@id_cliente, @id_auto, @id_empleado, @fecha, @subtotal, @iva, @total)";
                            var commandFactura = new MySqlCommand(queryFactura, connection, transaction);
                            commandFactura.Parameters.AddWithValue("@id_cliente", factura.id_cliente);
                            commandFactura.Parameters.AddWithValue("@id_auto", factura.id_auto);
                            commandFactura.Parameters.AddWithValue("@id_empleado", factura.id_empleado);
                            commandFactura.Parameters.AddWithValue("@fecha", factura.Fecha);
                            commandFactura.Parameters.AddWithValue("@subtotal", factura.Subtotal);
                            commandFactura.Parameters.AddWithValue("@iva", factura.Iva);
                            commandFactura.Parameters.AddWithValue("@total", factura.Total);
                            commandFactura.ExecuteNonQuery();

                            int idFactura = (int)commandFactura.LastInsertedId;

                            string queryAlquilados = @"INSERT INTO alquilados (id_auto, id_cliente, id_empleado, id_factura, fecha, fecha_devolver, devuelto)
                                       VALUES (@id_auto, @id_cliente, @id_empleado, @id_factura, @fecha_renta, @fecha_devolucion, 0)";
                            var commandAlquilados = new MySqlCommand(queryAlquilados, connection, transaction);
                            commandAlquilados.Parameters.AddWithValue("@id_auto", factura.id_auto);
                            commandAlquilados.Parameters.AddWithValue("@id_cliente", factura.id_cliente);
                            commandAlquilados.Parameters.AddWithValue("@id_empleado", factura.id_empleado);
                            commandAlquilados.Parameters.AddWithValue("@id_factura", idFactura);
                            commandAlquilados.Parameters.AddWithValue("@fecha_renta", factura.Fecha);
                            commandAlquilados.Parameters.AddWithValue("@fecha_devolucion", fechaDevolucion);
                            commandAlquilados.Parameters.AddWithValue("@devuelto", 0);
                            commandAlquilados.ExecuteNonQuery();

                            string queryActualizarAuto = @"UPDATE autos SET estado = 0 WHERE id_auto = @id_auto";
                            var commandActualizarAuto = new MySqlCommand(queryActualizarAuto, connection, transaction);
                            commandActualizarAuto.Parameters.AddWithValue("@id_auto", factura.id_auto);
                            commandActualizarAuto.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error al generar la factura y actualizar datos", ex);
                        }
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            return View(factura);
        }

        public IActionResult AutocompletarCliente(string term)
        {
            List<object> clientes = new List<object>();

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                string query = "SELECT id_cliente, nombre FROM clientes WHERE nombre LIKE @term";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@term", "%" + term + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new
                        {
                            label = reader.GetString("nombre"),
                            id = reader.GetInt32("id_cliente")
                        });
                    }
                }
            }

            return Json(clientes);
        }

        public IActionResult AutocompletarEmpleado(string term)
        {
            List<object> empleados = new List<object>();

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                string query = "SELECT id_empleado, nombre FROM empleados WHERE nombre LIKE @term";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@term", "%" + term + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empleados.Add(new
                        {
                            label = reader.GetString("nombre"),
                            id = reader.GetInt32("id_empleado")
                        });
                    }
                }
            }

            return Json(empleados);
        }
    }
}