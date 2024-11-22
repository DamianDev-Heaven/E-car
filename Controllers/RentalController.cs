using ProyectoFinalTecnicas.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using ProyectoFinalTecnicas.Data;

namespace ProyectoFinalTecnicas.Controllers
{
    public class RentalController : Controller
    {
        private readonly DataContext _dataContext;

        public RentalController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Devoluciones()
        {
            List<Devolucion> devoluciones = new List<Devolucion>();

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                // Query to get rental data
                string query = @"
                    SELECT a.id_alquiler, a.id_auto, a.id_cliente, a.id_empleado, a.id_factura, 
                           a.fecha, a.fecha_devolver, a.devuelto, a.fecha_devolucion_real, a.observaciones, 
                           au.marca, au.modelo, cl.nombre AS cliente_nombre, emp.nombre AS empleado_nombre
                    FROM alquilados a
                    JOIN autos au ON a.id_auto = au.id_auto
                    JOIN clientes cl ON a.id_cliente = cl.id_cliente
                    JOIN empleados emp ON a.id_empleado = emp.id_empleado
                    WHERE a.devuelto = 0";  // Only non-returned rentals

                var command = new MySqlCommand(query, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        devoluciones.Add(new Devolucion
                        {
                            IdAlquiler = reader.GetInt32("id_alquiler"),
                            IdAuto = reader.GetInt32("id_auto"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            IdEmpleado = reader.GetInt32("id_empleado"),
                            IdFactura = reader.GetInt32("id_factura"),
                            FechaRenta = reader.GetDateTime("fecha"),
                            FechaDevolucion = reader.GetDateTime("fecha_devolver"),
                            FechaDevolucionReal = reader.IsDBNull(reader.GetOrdinal("fecha_devolucion_real")) ? (DateTime?)null : reader.GetDateTime("fecha_devolucion_real"),
                            Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones"),
                            Devuelto = reader.GetBoolean("devuelto"),
                            Marca = reader.GetString("marca"),
                            Modelo = reader.GetString("modelo"),
                            ClienteNombre = reader.GetString("cliente_nombre"),
                            EmpleadoNombre = reader.GetString("empleado_nombre")
                        });
                    }
                }
            }

            return View(devoluciones);
        }
        // Acción para procesar la devolución
        public IActionResult AutocompletarCliente(string term)
        {
            List<object> clientes = new List<object>();

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT id_cliente, nombre, dui
                    FROM clientes
                    WHERE nombre LIKE @term OR dui LIKE @term";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@term", "%" + term + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new
                        {
                            label = $"{reader.GetString("nombre")} ({reader.GetString("dui")})",
                            id = reader.GetInt32("id_cliente")
                        });
                    }
                }
            }

            return Json(clientes);
        }
    }
}
