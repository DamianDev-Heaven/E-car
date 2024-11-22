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

                // Consulta para obtener los datos de alquiler, auto, cliente, empleado
                string query = @"
                    SELECT a.id_alquiler, a.id_auto, a.id_cliente, a.id_empleado, a.id_factura, 
                           a.fecha, a.fecha_devolver, a.devuelto, a.fecha_devolucion_real, a.observaciones, 
                           au.marca, au.modelo, cl.nombre AS cliente_nombre, emp.nombre AS empleado_nombre
                    FROM alquilados a
                    JOIN autos au ON a.id_auto = au.id_auto
                    JOIN clientes cl ON a.id_cliente = cl.id_cliente
                    JOIN empleados emp ON a.id_empleado = emp.id_empleado
                    WHERE a.devuelto = 0";  // Solo alquileres no devueltos

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
        [HttpPost]
        public IActionResult ProcesarDevolucion(int idAlquiler, int idAuto, DateTime fechaDevolucionReal, string observaciones)
        {
            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                // Actualizamos la tabla de alquilados
                string queryAlquiler = @"
            UPDATE alquilados
            SET fecha_devolucion_real = @fechaDevolucionReal, 
                observaciones = @observaciones, 
                devuelto = 1
            WHERE id_alquiler = @idAlquiler";

                var commandAlquiler = new MySqlCommand(queryAlquiler, connection);
                commandAlquiler.Parameters.AddWithValue("@idAlquiler", idAlquiler);
                commandAlquiler.Parameters.AddWithValue("@fechaDevolucionReal", fechaDevolucionReal);
                commandAlquiler.Parameters.AddWithValue("@observaciones", observaciones);

                // Ejecutar la actualización del alquiler
                commandAlquiler.ExecuteNonQuery();

                // Actualizamos el estado del auto a 1 (libre)
                string queryAuto = @"
            UPDATE autos
            SET estado = 1
            WHERE id_auto = @idAuto";

                var commandAuto = new MySqlCommand(queryAuto, connection);
                commandAuto.Parameters.AddWithValue("@idAuto", idAuto);

                // Ejecutar la actualización del auto
                commandAuto.ExecuteNonQuery();
            }

            return RedirectToAction("Devoluciones");
        }

    }
}
