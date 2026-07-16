using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ViajeRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public ResumenOperacionConductor? ObtenerResumenOperacion(int idConductor)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_ResumenOperacion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return MapResumenOperacion(dr);
        }

        public List<Viaje> Listar()
        {
            List<Viaje> lista = new List<Viaje>();

            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Viajes_ListarAdmin", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(MapViaje(dr));
            }

            return lista;
        }

        public List<Viaje> ListarDisponiblesPorConductor(int idConductor)
        {
            return ListarPorConductor("dbo.sp_Viajes_DisponiblesPorConductor", idConductor);
        }

        public List<Viaje> ListarActivosPorConductor(int idConductor)
        {
            return ListarPorConductor("dbo.sp_Viajes_ActivosPorConductor", idConductor);
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

        public void AceptarViaje(int idConductor, int idViaje)
        {
            EjecutarAccionConductor("dbo.sp_Viajes_Aceptar", idConductor, idViaje);
        }

        public int CrearSolicitudDemo(int idConductor, int? idTipoServicio)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Demo_Viajes_CrearSolicitudConductor", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@IdTipoServicio", idTipoServicio ?? (object)DBNull.Value);

            cn.Open();

            object? result = cmd.ExecuteScalar();

            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        public void RegistrarContraoferta(int idConductor, int idViaje, decimal tarifaContraoferta)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Viajes_Contraofertar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@IdViaje", idViaje);
            cmd.Parameters.AddWithValue("@TarifaContraoferta", tarifaContraoferta);

            cn.Open();
            cmd.ExecuteScalar();
        }

        public void AceptarContraofertaPasajero(int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Viajes_AceptarContraofertaPasajero", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);
            cmd.Parameters.AddWithValue("@UsuarioSistema", "Simulador Pasajero");

            cn.Open();
            cmd.ExecuteScalar();
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

        public void IniciarViaje(int idConductor, int idViaje)
        {
            EjecutarAccionConductor("dbo.sp_Viajes_Iniciar", idConductor, idViaje);
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

        public void FinalizarViaje(int idConductor, int idViaje)
        {
            EjecutarAccionConductor("dbo.sp_Viajes_Finalizar", idConductor, idViaje);
        }

        public void CancelarViaje(int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Viajes_CancelarAdmin", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdViaje", idViaje);

            cn.Open();
            cmd.ExecuteScalar();
        }

        public void CancelarViaje(int idConductor, int idViaje, string motivo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Viajes_Cancelar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@IdViaje", idViaje);
            cmd.Parameters.AddWithValue("@Motivo", motivo);

            cn.Open();
            cmd.ExecuteScalar();
        }

        private List<Viaje> ListarPorConductor(string storedProcedure, int idConductor)
        {
            List<Viaje> lista = new List<Viaje>();

            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand(storedProcedure, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdConductor", idConductor);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(MapViaje(dr));
            }

            return lista;
        }

        private void EjecutarAccionConductor(string storedProcedure, int idConductor, int idViaje)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand(storedProcedure, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@IdViaje", idViaje);

            cn.Open();
            cmd.ExecuteScalar();
        }

        private static Viaje MapViaje(SqlDataReader dr)
        {
            return new Viaje
            {
                IdViaje = Convert.ToInt32(dr["IdViaje"]),
                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                IdConductor = Convert.ToInt32(dr["IdConductor"]),
                IdVehiculo = Convert.ToInt32(dr["IdVehiculo"]),
                IdTipoServicio = Convert.ToInt32(dr["IdTipoServicio"]),
                TipoServicio = HasColumn(dr, "TipoServicio") ? dr["TipoServicio"].ToString() ?? string.Empty : string.Empty,
                IdEstadoViaje = Convert.ToInt32(dr["IdEstadoViaje"]),
                EstadoViaje = HasColumn(dr, "EstadoViaje") ? dr["EstadoViaje"].ToString() ?? string.Empty : string.Empty,
                Origen = dr["Origen"].ToString() ?? string.Empty,
                Destino = dr["Destino"].ToString() ?? string.Empty,
                TarifaEstimada = Convert.ToDecimal(dr["TarifaEstimada"]),
                TarifaFinal = Convert.ToDecimal(dr["TarifaFinal"]),
                TarifaSugerida = Convert.ToDecimal(dr["TarifaSugerida"]),
                TarifaOfertada = Convert.ToDecimal(dr["TarifaOfertada"]),
                TarifaContraoferta = dr["TarifaContraoferta"] == DBNull.Value ? null : Convert.ToDecimal(dr["TarifaContraoferta"]),
                TarifaAceptada = dr["TarifaAceptada"] == DBNull.Value ? null : Convert.ToDecimal(dr["TarifaAceptada"]),
                FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                FechaAceptacion = HasColumn(dr, "FechaAceptacion") && dr["FechaAceptacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaAceptacion"]) : null,
                FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                FechaFin = dr["FechaFin"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaFin"]),
                FechaCancelacion = HasColumn(dr, "FechaCancelacion") && dr["FechaCancelacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCancelacion"]) : null,
                MotivoCancelacion = HasColumn(dr, "MotivoCancelacion") ? dr["MotivoCancelacion"].ToString() ?? string.Empty : string.Empty
            };
        }

        private static ResumenOperacionConductor MapResumenOperacion(SqlDataReader dr)
        {
            return new ResumenOperacionConductor
            {
                IdConductor = Convert.ToInt32(dr["IdConductor"]),
                Conectado = Convert.ToBoolean(dr["Conectado"]),
                Prioridad = Convert.ToInt32(dr["Prioridad"]),
                PedidosDisponibles = Convert.ToInt32(dr["PedidosDisponibles"]),
                ViajesActivos = Convert.ToInt32(dr["ViajesActivos"]),
                ViajesHoy = Convert.ToInt32(dr["ViajesHoy"]),
                ViajesFinalizadosHoy = Convert.ToInt32(dr["ViajesFinalizadosHoy"]),
                GananciaHoy = Convert.ToDecimal(dr["GananciaHoy"]),
                ObjetivoTitulo = dr["ObjetivoTitulo"].ToString() ?? string.Empty,
                ObjetivoDetalle = dr["ObjetivoDetalle"].ToString() ?? string.Empty,
                ObjetivoActual = Convert.ToDecimal(dr["ObjetivoActual"]),
                ObjetivoMeta = Convert.ToDecimal(dr["ObjetivoMeta"]),
                EstadoOperativo = dr["EstadoOperativo"].ToString() ?? string.Empty
            };
        }

        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int index = 0; index < reader.FieldCount; index++)
            {
                if (string.Equals(reader.GetName(index), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
