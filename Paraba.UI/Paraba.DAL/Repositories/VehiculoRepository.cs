using System.Data;
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

            using SqlCommand cmd = new SqlCommand("dbo.sp_Vehiculos_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

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
                    EstadoVerificacion = dr["EstadoVerificacion"].ToString() ?? "Pendiente",
                    Observacion = dr["Observacion"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public int Crear(Vehiculo vehiculo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Vehiculos_Crear", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdConductor", vehiculo.IdConductor);
            cmd.Parameters.AddWithValue("@IdTipoServicio", vehiculo.IdTipoServicio);
            cmd.Parameters.AddWithValue("@Placa", vehiculo.Placa);
            cmd.Parameters.AddWithValue("@Marca", vehiculo.Marca);
            cmd.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
            cmd.Parameters.AddWithValue("@Color", vehiculo.Color);
            cmd.Parameters.AddWithValue("@Anio", vehiculo.Anio);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool ActualizarEstadoVerificacion(int idVehiculo, string estadoVerificacion, string observacion)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Vehiculos_ActualizarEstadoVerificacion", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdVehiculo", idVehiculo);
            cmd.Parameters.AddWithValue("@EstadoVerificacion", estadoVerificacion);
            cmd.Parameters.AddWithValue("@Observacion", observacion);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
