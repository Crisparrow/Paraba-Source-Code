using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class VehiculoRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Vehiculo> Listar()
        {
            List<Vehiculo> lista = new List<Vehiculo>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdVehiculo,
                    IdConductor,
                    IdTipoServicio,
                    Placa,
                    Marca,
                    Modelo,
                    Color,
                    Anio,
                    Verificado,
                    Estado,
                    FechaRegistro
                FROM Vehiculos
                ORDER BY IdVehiculo";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Vehiculo
                {
                    IdVehiculo = Convert.ToInt32(dr["IdVehiculo"]),
                    IdConductor = Convert.ToInt32(dr["IdConductor"]),
                    IdTipoServicio = Convert.ToInt32(dr["IdTipoServicio"]),
                    Placa = dr["Placa"].ToString() ?? string.Empty,
                    Marca = dr["Marca"].ToString() ?? string.Empty,
                    Modelo = dr["Modelo"].ToString() ?? string.Empty,
                    Color = dr["Color"].ToString() ?? string.Empty,
                    Anio = Convert.ToInt32(dr["Anio"]),
                    Verificado = Convert.ToBoolean(dr["Verificado"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
