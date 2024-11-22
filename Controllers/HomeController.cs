using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;

namespace ProyectoFinalTecnicas.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;

        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<string> marcas = new List<string>();

            try
            {
                using (var connection = _dataContext.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT DISTINCT marca FROM autos";
                    var command = new MySqlCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        marcas.Add(reader.GetString("marca"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Asignar marcas al ViewBag
            ViewBag.Marcas = marcas;

            return View(new List<Auto>()); // Pasamos un modelo vacío inicialmente
        }



        [HttpPost]
        [HttpPost]
        public IActionResult BuscarAutos(int? estado, string marca, double? precio)
        {
            List<Auto> res_busqueda = new List<Auto>();
            List<string> marcas = new List<string>();

            try
            {
                using (var connection = _dataContext.GetConnection())
                {
                    connection.Open();

                    // Cargar marcas
                    string marcasQuery = "SELECT DISTINCT marca FROM autos";
                    using (var marcasCommand = new MySqlCommand(marcasQuery, connection))
                    {
                        using (var marcasReader = marcasCommand.ExecuteReader())
                        {
                            while (marcasReader.Read())
                            {
                                marcas.Add(marcasReader.GetString("marca"));
                            }
                        }
                    }

                    // Construcción dinámica de la consulta SQL
                    string query = "SELECT * FROM autos WHERE 1=1"; // Base para añadir filtros
                    var parameters = new List<MySqlParameter>();

                    if (estado.HasValue)
                    {
                        query += " AND estado = @estado";
                        parameters.Add(new MySqlParameter("@estado", estado.Value));
                    }

                    if (!string.IsNullOrEmpty(marca))
                    {
                        query += " AND marca = @marca";
                        parameters.Add(new MySqlParameter("@marca", marca));
                    }

                    if (precio.HasValue)
                    {
                        query += " AND costo_dia <= @precio";
                        parameters.Add(new MySqlParameter("@precio", precio.Value));
                    }

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res_busqueda.Add(new Auto
                                {
                                    IdAuto = reader.GetInt32("id_auto"),
                                    Marca = reader.GetString("marca"),
                                    Modelo = reader.GetString("modelo"),
                                    Placa = reader.GetString("placa"),
                                    Tipo = reader.GetString("tipo"),
                                    Estado = reader.GetInt32("estado"),
                                    Price = reader.GetDouble("costo_dia"),
                                    Imagen = reader["imagen"] as string
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Pasar marcas y autos al ViewBag
            ViewBag.Marcas = marcas;

            return View("Index", res_busqueda);
        }

    }
}
