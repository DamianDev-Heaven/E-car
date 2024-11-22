using Microsoft.AspNetCore.Mvc;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;
using MySql.Data.MySqlClient;
using System;

namespace ProyectoFinalTecnicas.Controllers
{
    public class DevolucionController : Controller
    {
        private readonly DataContext _dataContext;

        public DevolucionController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
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

        // Método para obtener el auto alquilado de un cliente
        public IActionResult ObtenerAutoAlquilado(int id_cliente)
        {
            Auto auto = null;

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                string query = @"SELECT a.id_auto, a.marca, a.modelo 
                                 FROM autos a 
                                 JOIN alquilados al ON a.id_auto = al.id_auto 
                                 WHERE al.id_cliente = @id_cliente AND al.devuelto = 0";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_cliente", id_cliente);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        auto = new Auto
                        {
                            IdAuto = reader.GetInt32("id_auto"),
                            Marca = reader.GetString("marca"),
                            Modelo = reader.GetString("modelo")
                        };
                    }
                }
            }

            return Json(auto);
        }

        // Método para devolver el auto
        [HttpPost]
        public IActionResult DevolverAuto(int id_cliente, int id_auto, DateTime fechaDevolucion)
        {
            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();

                // Iniciar una transacción
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Actualizar la tabla "alquilados" para marcar el auto como devuelto
                        string queryActualizarAlquilado = @"UPDATE alquilados SET devuelto = 1, fecha_devolucion = @fecha_devolucion 
                                                            WHERE id_cliente = @id_cliente AND id_auto = @id_auto AND devuelto = 0";
                        var commandAlquilado = new MySqlCommand(queryActualizarAlquilado, connection, transaction);
                        commandAlquilado.Parameters.AddWithValue("@id_cliente", id_cliente);
                        commandAlquilado.Parameters.AddWithValue("@id_auto", id_auto);
                        commandAlquilado.Parameters.AddWithValue("@fecha_devolucion", fechaDevolucion);
                        commandAlquilado.ExecuteNonQuery();

                        // Actualizar el estado del auto a "libre"
                        string queryActualizarAuto = @"UPDATE autos SET estado = 0 WHERE id_auto = @id_auto";
                        var commandActualizarAuto = new MySqlCommand(queryActualizarAuto, connection, transaction);
                        commandActualizarAuto.Parameters.AddWithValue("@id_auto", id_auto);
                        commandActualizarAuto.ExecuteNonQuery();

                        // Confirmar la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Manejo de errores
                        throw new Exception("Error al devolver el auto", ex);
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
