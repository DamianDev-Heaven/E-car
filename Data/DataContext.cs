using MySql.Data.MySqlClient;

namespace ProyectoFinalTecnicas.Data
{
    public class DataContext
    {
        private readonly string _connectionString;

        public DataContext(IConfiguration conf)
        {
            _connectionString = conf.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection GetConnection()
        {
            try
            {
                return new MySqlConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la conexión: " + ex.Message);
            }
        }
    }
}
