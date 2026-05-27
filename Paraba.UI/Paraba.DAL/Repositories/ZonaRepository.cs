using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ZonaRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Zona> Listar()
        {
            List<Zona> lista = new List<Zona>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdZona,
                    IdCiudad,
                    Nombre,
                    Descripcion,
                    Estado,
                    FechaRegistro
                FROM Zonas
                ORDER BY IdZona";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Zona
                {
                    IdZona = Convert.ToInt32(dr["IdZona"]),
                    IdCiudad = Convert.ToInt32(dr["IdCiudad"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Descripcion = dr["Descripcion"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
