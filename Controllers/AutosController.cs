using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;
using static Mysqlx.Crud.Order.Types;

namespace ProyectoFinalTecnicas.Controllers
{
    public class AutosController : Controller
    {
        private readonly DataContext _dataContext;

        public AutosController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Auto nuevoAuto, IFormFile Imagen)
        {
            try
            {
                string rutaImagen = null;

                if (Imagen != null && Imagen.Length > 0)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = $"{Guid.NewGuid()}_{Imagen.FileName}";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Imagen.CopyTo(stream);
                    }

                    rutaImagen = $"/images/{fileName}";
                }

                nuevoAuto.Imagen = rutaImagen;

                using (var connection = _dataContext.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO autos (marca, modelo, placa, tipo, estado, costo_dia, imagen) VALUES (@marca, @modelo, @placa, @tipo, @estado, @costo_dia, @imagen)";
                    var command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@marca", nuevoAuto.Marca);
                    command.Parameters.AddWithValue("@modelo", nuevoAuto.Modelo);
                    command.Parameters.AddWithValue("@placa", nuevoAuto.Placa);
                    command.Parameters.AddWithValue("@tipo", nuevoAuto.Tipo);
                    command.Parameters.AddWithValue("@estado", nuevoAuto.Estado);
                    command.Parameters.AddWithValue("@costo_dia", nuevoAuto.Price);
                    command.Parameters.AddWithValue("@imagen", nuevoAuto.Imagen);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Auto agregado exitosamente";
                        return RedirectToAction("Registrar");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se pudo agregar el auto";
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar el auto: {ex.Message}");
            }

            return View(nuevoAuto);
        }

        


        public IActionResult MostrarAutos()
        {
            List<Auto> autos = new List<Auto>();

            using (var connection = _dataContext.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM autos";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Auto auto = new Auto
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
                    autos.Add(auto);
                }
            }

            return View(autos);
        }

    }

}
