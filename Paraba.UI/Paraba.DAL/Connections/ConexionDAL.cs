using Microsoft.Data.SqlClient;

namespace Paraba.DAL.Connections
{
    public class ConexionDAL
    {
        private readonly string cadenaConexion =
            "Server=DESKTOP-CCSKIIN\\SQLEXPRESS01;Database=ParabaDB;Trusted_Connection=True;TrustServerCertificate=True;";
        
        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}
