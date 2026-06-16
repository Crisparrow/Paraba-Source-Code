using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class AuditoriaViajeRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<AuditoriaViaje> Listar()
        {
            List<AuditoriaViaje> lista = new List<AuditoriaViaje>();

            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_AuditoriaViajes_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new AuditoriaViaje
                {
                    IdAuditoriaViaje = Convert.ToInt32(dr["IdAuditoriaViaje"]),
                    IdViaje = Convert.ToInt32(dr["IdViaje"]),
                    Accion = dr["Accion"].ToString() ?? string.Empty,
                    EstadoAnterior = dr["EstadoAnterior"].ToString() ?? string.Empty,
                    EstadoNuevo = dr["EstadoNuevo"].ToString() ?? string.Empty,
                    TarifaAnterior = dr["TarifaAnterior"] == DBNull.Value ? null : Convert.ToDecimal(dr["TarifaAnterior"]),
                    TarifaNueva = dr["TarifaNueva"] == DBNull.Value ? null : Convert.ToDecimal(dr["TarifaNueva"]),
                    UsuarioSistema = dr["UsuarioSistema"].ToString() ?? string.Empty,
                    Observacion = dr["Observacion"].ToString() ?? string.Empty,
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public void Registrar(AuditoriaViaje auditoria)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_AuditoriaViajes_Registrar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdViaje", auditoria.IdViaje);
            cmd.Parameters.AddWithValue("@Accion", auditoria.Accion);
            cmd.Parameters.AddWithValue("@EstadoAnterior", auditoria.EstadoAnterior);
            cmd.Parameters.AddWithValue("@EstadoNuevo", auditoria.EstadoNuevo);
            cmd.Parameters.AddWithValue("@TarifaAnterior", auditoria.TarifaAnterior ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TarifaNueva", auditoria.TarifaNueva ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UsuarioSistema", auditoria.UsuarioSistema);
            cmd.Parameters.AddWithValue("@Observacion", auditoria.Observacion);
            cmd.Parameters.AddWithValue("@FechaRegistro", auditoria.FechaRegistro);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
