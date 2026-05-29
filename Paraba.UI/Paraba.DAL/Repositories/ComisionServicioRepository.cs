using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ComisionServicioRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<ComisionServicio> Listar()
        {
            List<ComisionServicio> lista = new List<ComisionServicio>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdComisionServicio,
                    IdTipoServicio,
                    PorcentajeComision,
                    FechaInicioVigencia,
                    FechaFinVigencia,
                    Estado,
                    FechaRegistro
                FROM ComisionesServicio
                ORDER BY IdComisionServicio";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new ComisionServicio
                {
                    IdComisionServicio = Convert.ToInt32(dr["IdComisionServicio"]),
                    IdTipoServicio = Convert.ToInt32(dr["IdTipoServicio"]),
                    PorcentajeComision = Convert.ToDecimal(dr["PorcentajeComision"]),
                    FechaInicioVigencia = Convert.ToDateTime(dr["FechaInicioVigencia"]),
                    FechaFinVigencia = dr["FechaFinVigencia"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaFinVigencia"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public void ActualizarPorcentaje(int idComisionServicio, decimal porcentajeComision)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE ComisionesServicio
                SET PorcentajeComision = @PorcentajeComision
                WHERE IdComisionServicio = @IdComisionServicio";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdComisionServicio", idComisionServicio);
            cmd.Parameters.AddWithValue("@PorcentajeComision", porcentajeComision);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
