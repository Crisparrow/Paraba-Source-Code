using Microsoft.Data.SqlClient;

namespace Paraba.DAL.Connections
{
    public class ConexionDAL
    {
        private readonly string cadenaConexion =
            Environment.GetEnvironmentVariable("PARABA_DB_CONNECTION")
            ?? "Server=localhost\\SQLEXPRESS;Database=ParabaDB;Trusted_Connection=True;TrustServerCertificate=True;";
        
        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}
