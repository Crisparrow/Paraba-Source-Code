using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class CiudadRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Ciudad> Listar()
        {
            List<Ciudad> lista = new List<Ciudad>();

            using SqlConnection cn = conexion.ObtenerConexion();
            string query = "SELECT IdCiudad, IdDepartamento, Nombre, Estado, FechaRegistro FROM Ciudades ORDER BY IdCiudad";
            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Ciudad
                {
                    IdCiudad = Convert.ToInt32(dr["IdCiudad"]),
                    IdDepartamento = Convert.ToInt32(dr["IdDepartamento"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
