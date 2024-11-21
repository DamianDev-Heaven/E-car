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

                // Verificar si hay una imagen subida
                if (Imagen != null && Imagen.Length > 0)
                {
                    // Crear la ruta completa para guardar la imagen en wwwroot/images
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Generar un nombre único para la imagen
                    string fileName = $"{Guid.NewGuid()}_{Imagen.FileName}";
                    string filePath = Path.Combine(folderPath, fileName);

                    // Guardar la imagen en el sistema de archivos
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Imagen.CopyTo(stream);
                    }

                    // Guardar la ruta relativa en la base de datos
                    rutaImagen = $"/images/{fileName}";
                }

                // Asignar la ruta de la imagen al objeto `nuevoAuto`
                nuevoAuto.Imagen = rutaImagen;

                // Guardar en la base de datos
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
                    command.Parameters.AddWithValue("@imagen", nuevoAuto.Imagen);  // Guardar la ruta de la imagen

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
                        Estado = reader.GetInt32("estado"),// Convertir a bool,
                        Price = reader.GetDouble("costo_dia"),
                        Imagen = reader["imagen"] as string // Esto obtendrá la ruta si existe
                    };
                    autos.Add(auto);
                }
            }

            return View(autos); // Pasamos la lista de autos a la vista
        }






        //--------------------------------------sirve---------------

        //[HttpPost]
        //public IActionResult Registrar(Auto nuevoAuto, IFormFile Imagen)
        //{

        //    try
        //    {
        //        if (Imagen != null && Imagen.Length > 0)
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                // Copiar el contenido del archivo de la imagen en el MemoryStream
        //                Imagen.CopyTo(memoryStream);
        //                nuevoAuto.Imagen = memoryStream.ToArray();  // Guardar la imagen como un arreglo de bytes
        //            }
        //        }

        //        // Guardar en la base de datos
        //        using (var connection = _dataContext.GetConnection())
        //        {
        //            connection.Open();
        //            string query = "INSERT INTO autos (marca, modelo, placa, tipo, estado, costo_dia, imagen) VALUES (@marca, @modelo, @placa, @tipo, @estado, @costo_dia, @imagen)";
        //            var command = new MySqlCommand(query, connection);

        //            command.Parameters.AddWithValue("@marca", nuevoAuto.Marca);
        //            command.Parameters.AddWithValue("@modelo", nuevoAuto.Modelo);
        //            command.Parameters.AddWithValue("@placa", nuevoAuto.Placa);
        //            command.Parameters.AddWithValue("@tipo", nuevoAuto.Tipo);
        //            command.Parameters.AddWithValue("@estado", nuevoAuto.Estado);
        //            command.Parameters.AddWithValue("@costo_dia", nuevoAuto.Price);
        //            command.Parameters.AddWithValue("@imagen", nuevoAuto.Imagen);  // Guardar los datos binarios de la imagen

        //            int result = command.ExecuteNonQuery();

        //            if (result > 0)
        //            {
        //                TempData["SuccessMessage"] = "Auto agregado exitosamente";
        //                return RedirectToAction("Registrar");
        //            }
        //            else
        //            {
        //                ViewBag.ErrorMessage = "No se pudo agregar el auto";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error al agregar el auto: {ex.Message}");
        //    }

        //    return View(nuevoAuto);
        //}
        //public IActionResult MostrarAutos()
        //{
        //    List<Auto> autos = new List<Auto>();

        //    using (var connection = _dataContext.GetConnection())
        //    {
        //        connection.Open();
        //        string query = "SELECT * FROM autos";
        //        var command = new MySqlCommand(query, connection);
        //        var reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            Auto auto = new Auto
        //            {
        //                IdAuto = reader.GetInt32("id_auto"),
        //                Marca = reader.GetString("marca"),
        //                Modelo = reader.GetString("modelo"),
        //                Placa = reader.GetString("placa"),
        //                Tipo = reader.GetString("tipo"),
        //                Estado = reader.GetString("estado"),
        //                Price = reader.GetDouble("costo_dia"),
        //                Imagen = reader["imagen"] as byte[] // Asignamos los datos BLOB de la imagen
        //            };
        //            autos.Add(auto);
        //        }
        //    }

        //    return View(autos); // Pasamos la lista de autos a la vista
        //}

        //--------------------------------------sirve---------------









        // POST: Procesar el formulario y agregar el alumno a la base de datos
        //[HttpPost]
        //public IActionResult Registrar(Auto nuevoAuto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (var connection = _dataContext.GetConnection())
        //            {
        //                connection.Open();
        //                string query = "INSERT INTO autos (marca, modelo, placa, tipo, estado, costo_dia) VALUES (@marca, @modelo, @placa, @tipo,@estado,@costo_dia)";
        //                var command = new MySqlCommand(query, connection);


        //                command.Parameters.AddWithValue("@marca", nuevoAuto.Marca);  // Cambié 'nuevoCliente' por 'nuevoAuto' ya que es un auto
        //                command.Parameters.AddWithValue("@modelo", nuevoAuto.Modelo);
        //                command.Parameters.AddWithValue("@placa", nuevoAuto.Placa);
        //                command.Parameters.AddWithValue("@tipo", nuevoAuto.Tipo);
        //                command.Parameters.AddWithValue("@estado", nuevoAuto.Estado);
        //                command.Parameters.AddWithValue("@costo_dia", nuevoAuto.Price);



        //                int result = command.ExecuteNonQuery();

        //                if (result > 0)
        //                {
        //                    TempData["SuccessMessage"] = "Auto agregado exitosamente";
        //                    return RedirectToAction("Registrar");
        //                }
        //                else
        //                {
        //                    ViewBag.ErrorMessage = "No se pudo agregar";
        //                }
        //            }

        //            // Redirige a la lista de alumnos después de agregar
        //            //return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest($"Error al agregar el cliente: {ex.Message}");
        //        }
        //    }
        //    return View(nuevoAuto);
        //}
    }

}
