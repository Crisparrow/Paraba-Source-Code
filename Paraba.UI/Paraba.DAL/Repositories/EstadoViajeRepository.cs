using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class EstadoViajeRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<EstadoViaje> Listar()
        {
            List<EstadoViaje> lista = new List<EstadoViaje>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdEstadoViaje,
                    Nombre,
                    Estado
                FROM EstadosViaje
                ORDER BY IdEstadoViaje";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new EstadoViaje
                {
                    IdEstadoViaje = Convert.ToInt32(dr["IdEstadoViaje"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"])
                });
            }

            return lista;
        }
    }
}
