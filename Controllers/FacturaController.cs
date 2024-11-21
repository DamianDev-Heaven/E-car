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

        // Método para obtener detalles del auto por placa (sin cambios)
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
        public IActionResult GenerarFactura(Factura factura)
        {

            if (ModelState.IsValid)
            {
                using (var connection = _dataContext.GetConnection())
                {
                    connection.Open();

                    // Query para insertar la factura en la base de datos
                    string query = @"INSERT INTO facturas (id_cliente, id_auto, id_empleado, fecha, subtotal, iva, total)
                                     VALUES (@id_cliente, @id_auto, @id_empleado, @fecha, @subtotal, @iva, @total)";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_cliente", factura.id_cliente);
                    command.Parameters.AddWithValue("@id_auto", factura.id_auto);
                    command.Parameters.AddWithValue("@id_empleado", factura.id_empleado);
                    command.Parameters.AddWithValue("@fecha", factura.Fecha);
                    command.Parameters.AddWithValue("@subtotal", factura.Subtotal);
                    command.Parameters.AddWithValue("@iva", factura.Iva);
                    command.Parameters.AddWithValue("@total", factura.Total);

                    command.ExecuteNonQuery();
                }

                return RedirectToAction("Index", "Home"); // Redirige a una página de confirmación o al índice
            }

            return View(factura); // Si hay errores de validación, vuelve al formulario
        }
    


        // Método para autocompletar cliente
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

        // Método para autocompletar empleado
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