using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class PaisRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Pais> Listar()
        {
            List<Pais> lista = new List<Pais>();

            using SqlConnection cn = conexion.ObtenerConexion();
            string query = "SELECT IdPais, Nombre, CodigoIso, Estado, FechaRegistro FROM Paises ORDER BY IdPais";
            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Pais
                {
                    IdPais = Convert.ToInt32(dr["IdPais"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    CodigoIso = dr["CodigoIso"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
