using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ViajeRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Viaje> Listar()
        {
            List<Viaje> lista = new List<Viaje>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdViaje,
                    IdPasajero,
                    IdConductor,
                    IdVehiculo,
                    IdTipoServicio,
                    IdEstadoViaje,
                    Origen,
                    Destino,
                    TarifaEstimada,
                    TarifaFinal,
                    TarifaSugerida,
                    TarifaOfertada,
                    TarifaContraoferta,
                    TarifaAceptada,
                    FechaSolicitud,
                    FechaInicio,
                    FechaFin
                FROM Viajes
                ORDER BY IdViaje";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Viaje
                {
                    IdViaje = Convert.ToInt32(dr["IdViaje"]),
                    IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                    IdConductor = Convert.ToInt32(dr["IdConductor"]),
                    IdVehiculo = Convert.ToInt32(dr["IdVehiculo"]),
                    IdTipoServicio = Convert.ToInt32(dr["IdTipoServicio"]),
                    IdEstadoViaje = Convert.ToInt32(dr["IdEstadoViaje"]),
                    Origen = dr["Origen"].ToString() ?? string.Empty,
                    Destino = dr["Destino"].ToString() ?? string.Empty,
                    TarifaEstimada = Convert.ToDecimal(dr["TarifaEstimada"]),
                    TarifaFinal = Convert.ToDecimal(dr["TarifaFinal"]),
                    TarifaSugerida = Convert.ToDecimal(dr["TarifaSugerida"]),
                    TarifaOfertada = Convert.ToDecimal(dr["TarifaOfertada"]),
                    TarifaContraoferta = dr["TarifaContraoferta"] == DBNull.Value ? null : Convert.ToDecimal(dr["TarifaContraoferta"]),
                    TarifaAceptada = dr["TarifaAceptada"] == DBNull.Value ? null : Convert.ToDecimal(dr["TarifaAceptada"]),
                    FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                    FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                    FechaFin = dr["FechaFin"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaFin"])
                });
            }

            return lista;
        }

        public void RegistrarSolicitud(Viaje viaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                INSERT INTO Viajes
                (
                    IdPasajero,
                    IdConductor,
                    IdVehiculo,
                    IdTipoServicio,
                    IdEstadoViaje,
                    Origen,
                    Destino,
                    TarifaEstimada,
                    TarifaFinal,
                    TarifaSugerida,
                    TarifaOfertada,
                    TarifaContraoferta,
                    TarifaAceptada,
                    FechaSolicitud,
                    FechaInicio,
                    FechaFin
                )
                VALUES
                (
                    @IdPasajero,
                    @IdConductor,
                    @IdVehiculo,
                    @IdTipoServicio,
                    @IdEstadoViaje,
                    @Origen,
                    @Destino,
                    @TarifaEstimada,
                    @TarifaFinal,
                    @TarifaSugerida,
                    @TarifaOfertada,
                    @TarifaContraoferta,
                    @TarifaAceptada,
                    @FechaSolicitud,
                    @FechaInicio,
                    @FechaFin
                )";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdPasajero", viaje.IdPasajero);
            cmd.Parameters.AddWithValue("@IdConductor", viaje.IdConductor);
            cmd.Parameters.AddWithValue("@IdVehiculo", viaje.IdVehiculo);
            cmd.Parameters.AddWithValue("@IdTipoServicio", viaje.IdTipoServicio);
            cmd.Parameters.AddWithValue("@IdEstadoViaje", viaje.IdEstadoViaje);
            cmd.Parameters.AddWithValue("@Origen", viaje.Origen);
            cmd.Parameters.AddWithValue("@Destino", viaje.Destino);
            cmd.Parameters.AddWithValue("@TarifaEstimada", viaje.TarifaEstimada);
            cmd.Parameters.AddWithValue("@TarifaFinal", viaje.TarifaFinal);
            cmd.Parameters.AddWithValue("@TarifaSugerida", viaje.TarifaSugerida);
            cmd.Parameters.AddWithValue("@TarifaOfertada", viaje.TarifaOfertada);
            cmd.Parameters.AddWithValue("@TarifaContraoferta", viaje.TarifaContraoferta ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TarifaAceptada", viaje.TarifaAceptada ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaSolicitud", viaje.FechaSolicitud);
            cmd.Parameters.AddWithValue("@FechaInicio", viaje.FechaInicio ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaFin", viaje.FechaFin ?? (object)DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void RegistrarContraoferta(int idViaje, decimal tarifaContraoferta)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Viajes
                SET
                    TarifaContraoferta = @TarifaContraoferta,
                    TarifaAceptada = NULL,
                    TarifaFinal = 0,
                    IdEstadoViaje = 1
                WHERE IdViaje = @IdViaje";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);
            cmd.Parameters.AddWithValue("@TarifaContraoferta", tarifaContraoferta);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void AceptarContraoferta(int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Viajes
                SET
                    TarifaAceptada = TarifaContraoferta,
                    TarifaFinal = TarifaContraoferta,
                    IdEstadoViaje = 2
                WHERE
                    IdViaje = @IdViaje
                    AND TarifaContraoferta IS NOT NULL";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void IniciarViaje(int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Viajes
                SET
                    IdEstadoViaje = 3,
                    FechaInicio = ISNULL(FechaInicio, GETDATE())
                WHERE
                    IdViaje = @IdViaje
                    AND IdEstadoViaje = 2";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void FinalizarViaje(int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Viajes
                SET
                    IdEstadoViaje = 4,
                    FechaFin = ISNULL(FechaFin, GETDATE())
                WHERE
                    IdViaje = @IdViaje
                    AND IdEstadoViaje = 3";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void CancelarViaje(int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Viajes
                SET
                    IdEstadoViaje = 5,
                    FechaFin = ISNULL(FechaFin, GETDATE())
                WHERE
                    IdViaje = @IdViaje
                    AND IdEstadoViaje IN (1, 2, 3)";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
