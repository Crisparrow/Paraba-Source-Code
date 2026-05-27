using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class TarifaRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Tarifa> Listar()
        {
            List<Tarifa> lista = new List<Tarifa>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdTarifa,
                    IdTipoServicio,
                    TarifaBase,
                    CostoPorKilometro,
                    CostoPorMinuto,
                    TarifaMinima,
                    Estado,
                    FechaRegistro
                FROM Tarifas
                ORDER BY IdTarifa";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Tarifa
                {
                    IdTarifa = Convert.ToInt32(dr["IdTarifa"]),
                    IdTipoServicio = Convert.ToInt32(dr["IdTipoServicio"]),
                    TarifaBase = Convert.ToDecimal(dr["TarifaBase"]),
                    CostoPorKilometro = Convert.ToDecimal(dr["CostoPorKilometro"]),
                    CostoPorMinuto = Convert.ToDecimal(dr["CostoPorMinuto"]),
                    TarifaMinima = Convert.ToDecimal(dr["TarifaMinima"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
