using System.Data;
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

            using SqlCommand cmd = new SqlCommand("dbo.sp_LiquidacionesConductores_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

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

            using SqlCommand cmd = new SqlCommand("dbo.sp_LiquidacionesConductores_ListarIdsViajesLiquidados", cn);
            cmd.CommandType = CommandType.StoredProcedure;

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

            using SqlCommand cmd = new SqlCommand("dbo.sp_LiquidacionesConductores_ListarDetalles", cn);
            cmd.CommandType = CommandType.StoredProcedure;
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

            using SqlCommand cmd = new SqlCommand("dbo.sp_LiquidacionesConductores_MarcarPagada", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdLiquidacionConductor", idLiquidacionConductor);
            cmd.Parameters.AddWithValue("@Observacion", observacion);

            cn.Open();
            cmd.ExecuteScalar();
        }

        public void Anular(int idLiquidacionConductor, string motivo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_LiquidacionesConductores_Anular", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdLiquidacionConductor", idLiquidacionConductor);
            cmd.Parameters.AddWithValue("@Observacion", motivo);

            cn.Open();
            cmd.ExecuteScalar();
        }

        public int Crear(LiquidacionConductor liquidacion, List<LiquidacionConductorDetalle> detalles)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            cn.Open();

            using SqlTransaction transaction = cn.BeginTransaction();

            try
            {
                using SqlCommand cmdCabecera = new SqlCommand("dbo.sp_LiquidacionesConductores_CrearCabecera", cn, transaction);
                cmdCabecera.CommandType = CommandType.StoredProcedure;
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

                foreach (var detalle in detalles)
                {
                    using SqlCommand cmdDetalle = new SqlCommand("dbo.sp_LiquidacionesConductores_CrearDetalle", cn, transaction);
                    cmdDetalle.CommandType = CommandType.StoredProcedure;
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
