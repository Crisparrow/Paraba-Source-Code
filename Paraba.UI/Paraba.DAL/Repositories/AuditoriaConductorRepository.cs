using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class AuditoriaConductorRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<AuditoriaConductor> Listar()
        {
            List<AuditoriaConductor> lista = new List<AuditoriaConductor>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdAuditoriaConductor,
                    IdConductor,
                    Accion,
                    EstadoAnterior,
                    EstadoNuevo,
                    UsuarioSistema,
                    Observacion,
                    FechaRegistro
                FROM AuditoriaConductores
                ORDER BY FechaRegistro DESC, IdAuditoriaConductor DESC";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new AuditoriaConductor
                {
                    IdAuditoriaConductor = Convert.ToInt32(dr["IdAuditoriaConductor"]),
                    IdConductor = Convert.ToInt32(dr["IdConductor"]),
                    Accion = dr["Accion"].ToString() ?? string.Empty,
                    EstadoAnterior = dr["EstadoAnterior"].ToString() ?? string.Empty,
                    EstadoNuevo = dr["EstadoNuevo"].ToString() ?? string.Empty,
                    UsuarioSistema = dr["UsuarioSistema"].ToString() ?? string.Empty,
                    Observacion = dr["Observacion"].ToString() ?? string.Empty,
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public void Registrar(AuditoriaConductor auditoria)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                INSERT INTO AuditoriaConductores
                (
                    IdConductor,
                    Accion,
                    EstadoAnterior,
                    EstadoNuevo,
                    UsuarioSistema,
                    Observacion,
                    FechaRegistro
                )
                VALUES
                (
                    @IdConductor,
                    @Accion,
                    @EstadoAnterior,
                    @EstadoNuevo,
                    @UsuarioSistema,
                    @Observacion,
                    @FechaRegistro
                )";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cmd.Parameters.AddWithValue("@IdConductor", auditoria.IdConductor);
            cmd.Parameters.AddWithValue("@Accion", auditoria.Accion);
            cmd.Parameters.AddWithValue("@EstadoAnterior", auditoria.EstadoAnterior);
            cmd.Parameters.AddWithValue("@EstadoNuevo", auditoria.EstadoNuevo);
            cmd.Parameters.AddWithValue("@UsuarioSistema", auditoria.UsuarioSistema);
            cmd.Parameters.AddWithValue("@Observacion", auditoria.Observacion);
            cmd.Parameters.AddWithValue("@FechaRegistro", auditoria.FechaRegistro);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
