using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class TipoViaRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<TipoVia> Listar()
        {
            List<TipoVia> lista = new List<TipoVia>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdTipoVia,
                    Nombre,
                    PorcentajeIncremento,
                    Estado,
                    FechaRegistro
                FROM TiposVia
                ORDER BY IdTipoVia";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new TipoVia
                {
                    IdTipoVia = Convert.ToInt32(dr["IdTipoVia"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    PorcentajeIncremento = Convert.ToDecimal(dr["PorcentajeIncremento"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
