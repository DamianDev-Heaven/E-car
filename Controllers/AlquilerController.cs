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
            List<Alquiler> alquiler = new List<Alquiler>();

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
                        alquiler.Add(new Alquiler
                        {
                            IdAlquiler = reader.GetInt32("id_alquiler"),
                            IdAuto = reader.GetInt32("id_auto"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            IdEmpleado = reader.GetInt32("id_empleado"),
                            IdFactura = reader.GetInt32("id_factura"),
                            Fecha = reader.GetDateTime("fecha"),
                            FechaDevolver = reader.GetDateTime("fecha_devolver")
                        });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al obtener los alquileres: " + ex.Message;
                }
            }

            return View(alquiler);
        }

    }
}
