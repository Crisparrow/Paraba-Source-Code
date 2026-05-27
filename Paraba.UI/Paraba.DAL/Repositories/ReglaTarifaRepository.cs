using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ReglaTarifaRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<ReglaTarifa> Listar()
        {
            List<ReglaTarifa> lista = new List<ReglaTarifa>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdReglaTarifa,
                    Nombre,
                    TipoRegla,
                    IdTipoServicio,
                    IdZona,
                    PorcentajeIncremento,
                    MontoIncremento,
                    HoraInicio,
                    HoraFin,
                    Prioridad,
                    Estado,
                    FechaRegistro
                FROM ReglasTarifa
                ORDER BY Prioridad, IdReglaTarifa";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new ReglaTarifa
                {
                    IdReglaTarifa = Convert.ToInt32(dr["IdReglaTarifa"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    TipoRegla = dr["TipoRegla"].ToString() ?? string.Empty,
                    IdTipoServicio = dr["IdTipoServicio"] == DBNull.Value ? null : Convert.ToInt32(dr["IdTipoServicio"]),
                    IdZona = dr["IdZona"] == DBNull.Value ? null : Convert.ToInt32(dr["IdZona"]),
                    PorcentajeIncremento = Convert.ToDecimal(dr["PorcentajeIncremento"]),
                    MontoIncremento = Convert.ToDecimal(dr["MontoIncremento"]),
                    HoraInicio = dr["HoraInicio"] == DBNull.Value ? null : (TimeSpan)dr["HoraInicio"],
                    HoraFin = dr["HoraFin"] == DBNull.Value ? null : (TimeSpan)dr["HoraFin"],
                    Prioridad = Convert.ToInt32(dr["Prioridad"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
