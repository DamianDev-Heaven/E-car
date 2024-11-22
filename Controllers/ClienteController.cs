using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProyectoFinalTecnicas.Data;
using ProyectoFinalTecnicas.Models;
using System.Collections.Generic;
using System;

namespace ProyectoFinalTecnicas.Controllers
{
    public class ClienteController : Controller
    {
        private readonly DataContext _dataContext;

        public ClienteController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult MostrarClientes()
        {
            List<Cliente> cliente = new List<Cliente>();

            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión abierta exitosamente.");
                    string query = "SELECT * FROM clientes";
                    var command = new MySqlCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"Cliente encontrado: {reader["nombre"]}");
                        cliente.Add(new Cliente
                        {
                            IdCliente = reader.GetInt32("id_cliente"),
                            Nombre = reader.GetString("nombre"),
                            Direccion = reader.GetString("direccion"),
                            Telefono = reader.GetString("telefono"),
                            DUI = reader.GetString("dui"),
                            Email = reader.GetString("email"),

                            Contrasena = reader["contrasena"] == DBNull.Value ? null : reader.GetString("contrasena") // Manejo de nulos
                        });
                    }
                    Console.WriteLine("Datos cargados correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    ViewBag.ErrorMessage = "Error al obtener los clientes: " + ex.Message;
                }

            }

            return View(cliente);
        }


        public IActionResult Delete(int id)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM clientes WHERE id_cliente = @id_cliente"; // Asegúrate de que el nombre coincida
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_cliente", id);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Cliente eliminado exitosamente";
                        return RedirectToAction("MostrarClientes");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No se pudo eliminar el Cliente. Intenta nuevamente.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al eliminar el Cliente: " + ex.Message;
                }
            }

            return RedirectToAction("MostrarClientes");
        }





        public IActionResult Edit(int id)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM clientes WHERE id_cliente = @id_cliente";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_cliente", id);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var cliente = new Cliente
                        {
                            IdCliente = Convert.ToInt32(reader["id_cliente"]),
                            Nombre = reader["nombre"].ToString(),
                            Direccion = reader["direccion"].ToString(),
                            Telefono = reader["telefono"].ToString(),
                            DUI = reader["dui"].ToString(),
                            Email = reader["email"].ToString(),
                            Contrasena = reader["contrasena"].ToString()
                        };

                        return View(cliente); // Devuelve el modelo cliente a la vista Edit
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Cliente no encontrado";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al cargar el cliente: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }


        [HttpPost]
        public IActionResult Update(Cliente cliente)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE clientes SET nombre = @nombre, direccion = @direccion, telefono = @telefono, dui = @dui, email = @email, contrasena = @contrasena WHERE id_cliente = @id_cliente";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@direccion", cliente.Direccion);
                    command.Parameters.AddWithValue("@telefono", cliente.Telefono);
                    command.Parameters.AddWithValue("@dui", cliente.DUI);
                    command.Parameters.AddWithValue("@email", cliente.Email);
                    command.Parameters.AddWithValue("@contrasena", cliente.Contrasena);
                    command.Parameters.AddWithValue("@id_cliente", cliente.IdCliente);

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Cliente actualizado exitosamente";
                        return RedirectToAction("MostrarClientes");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No se pudo actualizar el cliente. Intenta nuevamente.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al actualizar el cliente: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }



        public IActionResult BuscarClientes(string criterio)
        {
            List<Cliente> clientes = new List<Cliente>();

            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión abierta exitosamente.");

                    // Construir la consulta SQL con parámetros para evitar inyección de SQL
                    string query = @"
                            SELECT * FROM clientes 
                            WHERE nombre LIKE @Criterio 
                               OR dui LIKE @Criterio 
                               OR email LIKE @Criterio";

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Criterio", $"%{criterio}%");

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine($"Cliente encontrado: {reader["nombre"]}");
                        clientes.Add(new Cliente
                        {
                            IdCliente = reader.GetInt32("id_cliente"),
                            Nombre = reader.GetString("nombre"),
                            Direccion = reader.GetString("direccion"),
                            Telefono = reader.GetString("telefono"),
                            DUI = reader.GetString("dui"),
                            Email = reader.GetString("email"),
                            Contrasena = reader["contrasena"] == DBNull.Value ? null : reader.GetString("contrasena") // Manejo de nulos
                        });
                    }
                    Console.WriteLine("Búsqueda completada correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    ViewBag.ErrorMessage = "Error al buscar clientes: " + ex.Message;
                }
            }

            return View("MostrarClientes", clientes); // Usa la misma vista para mostrar resultados
        }


        [HttpPost]
        public IActionResult AgregarCliente(Cliente nuevoCliente)
        {
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión abierta exitosamente.");

                    // Consulta SQL para insertar un nuevo cliente
                    string query = @"
                INSERT INTO clientes (nombre, direccion, telefono, dui, email, contrasena) 
                VALUES (@Nombre, @Direccion, @Telefono, @DUI, @Email, @Contrasena)";

                    var command = new MySqlCommand(query, connection);

                    // Asignar valores a los parámetros de la consulta
                    command.Parameters.AddWithValue("@Nombre", nuevoCliente.Nombre);
                    command.Parameters.AddWithValue("@Direccion", nuevoCliente.Direccion);
                    command.Parameters.AddWithValue("@Telefono", nuevoCliente.Telefono);
                    command.Parameters.AddWithValue("@DUI", nuevoCliente.DUI);
                    command.Parameters.AddWithValue("@Email", nuevoCliente.Email);
                    command.Parameters.AddWithValue("@Contrasena", nuevoCliente.Contrasena);

                    // Ejecutar la consulta
                    int filasAfectadas = command.ExecuteNonQuery();

                    Console.WriteLine($"Cliente agregado correctamente. Filas afectadas: {filasAfectadas}");
                    TempData["SuccessMessage"] = "Cliente agregado exitosamente.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    ViewBag.ErrorMessage = "Error al agregar el cliente: " + ex.Message;
                }
            }

            // Redirigir a la vista de MostrarClientes (o cualquier otra vista)
            return RedirectToAction("MostrarClientes");
        }




    }
}
