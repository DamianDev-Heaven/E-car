using Microsoft.AspNetCore.Mvc;
using ProyectoFinalTecnicas.Data;

namespace ProyectoFinalTecnicas.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;

        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            string connectionStatus;

            // Intentar abrir la conexi�n
            using (var connection = _dataContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    connectionStatus = "Conexi�n exitosa";
                }
                catch (Exception ex)
                {
                    connectionStatus = "Error de conexi�n: " + ex.Message;
                }
            }

            ViewBag.ConnectionStatus = connectionStatus;

            return View();
        }
    }
}
