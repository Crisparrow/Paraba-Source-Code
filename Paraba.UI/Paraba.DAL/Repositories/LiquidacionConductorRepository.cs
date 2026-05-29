using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class LiquidacionConductorRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<LiquidacionConductor> Listar()
        {
            List<LiquidacionConductor> lista = new List<LiquidacionConductor>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdLiquidacionConductor,
                    IdConductor,
                    FechaDesde,
                    FechaHasta,
                    PorcentajeComision,
                    TotalBruto,
                    TotalComisionParaba,
                    TotalNetoConductor,
                    Estado,
                    UsuarioCierre,
                    FechaCierre,
                    FechaPago,
                    Observacion
                FROM LiquidacionesConductores
                ORDER BY FechaCierre DESC, IdLiquidacionConductor DESC";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new LiquidacionConductor
                {
                    IdLiquidacionConductor = Convert.ToInt32(dr["IdLiquidacionConductor"]),
                    IdConductor = Convert.ToInt32(dr["IdConductor"]),
                    FechaDesde = Convert.ToDateTime(dr["FechaDesde"]),
                    FechaHasta = Convert.ToDateTime(dr["FechaHasta"]),
                    PorcentajeComision = Convert.ToDecimal(dr["PorcentajeComision"]),
                    TotalBruto = Convert.ToDecimal(dr["TotalBruto"]),
                    TotalComisionParaba = Convert.ToDecimal(dr["TotalComisionParaba"]),
                    TotalNetoConductor = Convert.ToDecimal(dr["TotalNetoConductor"]),
                    Estado = dr["Estado"].ToString() ?? string.Empty,
                    UsuarioCierre = dr["UsuarioCierre"].ToString() ?? string.Empty,
                    FechaCierre = Convert.ToDateTime(dr["FechaCierre"]),
                    FechaPago = dr["FechaPago"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaPago"]),
                    Observacion = dr["Observacion"].ToString() ?? string.Empty
                });
            }

            return lista;
        }

        public List<int> ListarIdsViajesLiquidados()
        {
            List<int> idsViajes = new List<int>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT DISTINCT IdViaje
                FROM LiquidacionesConductoresDetalle";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                idsViajes.Add(Convert.ToInt32(dr["IdViaje"]));
            }

            return idsViajes;
        }

        public List<LiquidacionConductorDetalle> ListarDetalles(int idLiquidacionConductor)
        {
            List<LiquidacionConductorDetalle> lista = new List<LiquidacionConductorDetalle>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdLiquidacionConductorDetalle,
                    IdLiquidacionConductor,
                    IdViaje,
                    TarifaFinal,
                    ComisionParaba,
                    NetoConductor,
                    FechaRegistro
                FROM LiquidacionesConductoresDetalle
                WHERE IdLiquidacionConductor = @IdLiquidacionConductor
                ORDER BY IdViaje";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdLiquidacionConductor", idLiquidacionConductor);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new LiquidacionConductorDetalle
                {
                    IdLiquidacionConductorDetalle = Convert.ToInt32(dr["IdLiquidacionConductorDetalle"]),
                    IdLiquidacionConductor = Convert.ToInt32(dr["IdLiquidacionConductor"]),
                    IdViaje = Convert.ToInt32(dr["IdViaje"]),
                    TarifaFinal = Convert.ToDecimal(dr["TarifaFinal"]),
                    ComisionParaba = Convert.ToDecimal(dr["ComisionParaba"]),
                    NetoConductor = Convert.ToDecimal(dr["NetoConductor"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public void MarcarPagada(int idLiquidacionConductor, string observacion)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE LiquidacionesConductores
                SET
                    Estado = 'Pagada',
                    FechaPago = GETDATE(),
                    Observacion = @Observacion
                WHERE
                    IdLiquidacionConductor = @IdLiquidacionConductor
                    AND Estado = 'Cerrada'";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdLiquidacionConductor", idLiquidacionConductor);
            cmd.Parameters.AddWithValue("@Observacion", observacion);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Anular(int idLiquidacionConductor, string motivo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE LiquidacionesConductores
                SET
                    Estado = 'Anulada',
                    Observacion = @Observacion
                WHERE
                    IdLiquidacionConductor = @IdLiquidacionConductor
                    AND Estado = 'Cerrada'";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdLiquidacionConductor", idLiquidacionConductor);
            cmd.Parameters.AddWithValue("@Observacion", motivo);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public int Crear(LiquidacionConductor liquidacion, List<LiquidacionConductorDetalle> detalles)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            cn.Open();

            using SqlTransaction transaction = cn.BeginTransaction();

            try
            {
                string queryCabecera = @"
                    INSERT INTO LiquidacionesConductores
                    (
                        IdConductor,
                        FechaDesde,
                        FechaHasta,
                        PorcentajeComision,
                        TotalBruto,
                        TotalComisionParaba,
                        TotalNetoConductor,
                        Estado,
                        UsuarioCierre,
                        FechaCierre,
                        Observacion
                    )
                    OUTPUT INSERTED.IdLiquidacionConductor
                    VALUES
                    (
                        @IdConductor,
                        @FechaDesde,
                        @FechaHasta,
                        @PorcentajeComision,
                        @TotalBruto,
                        @TotalComisionParaba,
                        @TotalNetoConductor,
                        @Estado,
                        @UsuarioCierre,
                        @FechaCierre,
                        @Observacion
                    )";

                using SqlCommand cmdCabecera = new SqlCommand(queryCabecera, cn, transaction);
                cmdCabecera.Parameters.AddWithValue("@IdConductor", liquidacion.IdConductor);
                cmdCabecera.Parameters.AddWithValue("@FechaDesde", liquidacion.FechaDesde.Date);
                cmdCabecera.Parameters.AddWithValue("@FechaHasta", liquidacion.FechaHasta.Date);
                cmdCabecera.Parameters.AddWithValue("@PorcentajeComision", liquidacion.PorcentajeComision);
                cmdCabecera.Parameters.AddWithValue("@TotalBruto", liquidacion.TotalBruto);
                cmdCabecera.Parameters.AddWithValue("@TotalComisionParaba", liquidacion.TotalComisionParaba);
                cmdCabecera.Parameters.AddWithValue("@TotalNetoConductor", liquidacion.TotalNetoConductor);
                cmdCabecera.Parameters.AddWithValue("@Estado", liquidacion.Estado);
                cmdCabecera.Parameters.AddWithValue("@UsuarioCierre", liquidacion.UsuarioCierre);
                cmdCabecera.Parameters.AddWithValue("@FechaCierre", liquidacion.FechaCierre);
                cmdCabecera.Parameters.AddWithValue("@Observacion", liquidacion.Observacion);

                int idLiquidacion = Convert.ToInt32(cmdCabecera.ExecuteScalar());

                string queryDetalle = @"
                    INSERT INTO LiquidacionesConductoresDetalle
                    (
                        IdLiquidacionConductor,
                        IdViaje,
                        TarifaFinal,
                        ComisionParaba,
                        NetoConductor,
                        FechaRegistro
                    )
                    VALUES
                    (
                        @IdLiquidacionConductor,
                        @IdViaje,
                        @TarifaFinal,
                        @ComisionParaba,
                        @NetoConductor,
                        @FechaRegistro
                    )";

                foreach (var detalle in detalles)
                {
                    using SqlCommand cmdDetalle = new SqlCommand(queryDetalle, cn, transaction);
                    cmdDetalle.Parameters.AddWithValue("@IdLiquidacionConductor", idLiquidacion);
                    cmdDetalle.Parameters.AddWithValue("@IdViaje", detalle.IdViaje);
                    cmdDetalle.Parameters.AddWithValue("@TarifaFinal", detalle.TarifaFinal);
                    cmdDetalle.Parameters.AddWithValue("@ComisionParaba", detalle.ComisionParaba);
                    cmdDetalle.Parameters.AddWithValue("@NetoConductor", detalle.NetoConductor);
                    cmdDetalle.Parameters.AddWithValue("@FechaRegistro", detalle.FechaRegistro);
                    cmdDetalle.ExecuteNonQuery();
                }

                transaction.Commit();

                return idLiquidacion;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
