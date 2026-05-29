using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class AuditoriaAdministrativaRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<AuditoriaAdministrativa> Listar()
        {
            List<AuditoriaAdministrativa> lista = new List<AuditoriaAdministrativa>();

            using SqlConnection cn = conexion.ObtenerConexion();
            string query = @"
                SELECT IdAuditoriaAdministrativa, Modulo, Accion, Entidad, IdEntidad, UsuarioSistema, Observacion, FechaRegistro
                FROM AuditoriaAdministrativa
                ORDER BY FechaRegistro DESC, IdAuditoriaAdministrativa DESC";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new AuditoriaAdministrativa
                {
                    IdAuditoriaAdministrativa = Convert.ToInt32(dr["IdAuditoriaAdministrativa"]),
                    Modulo = dr["Modulo"].ToString() ?? string.Empty,
                    Accion = dr["Accion"].ToString() ?? string.Empty,
                    Entidad = dr["Entidad"].ToString() ?? string.Empty,
                    IdEntidad = dr["IdEntidad"] == DBNull.Value ? null : Convert.ToInt32(dr["IdEntidad"]),
                    UsuarioSistema = dr["UsuarioSistema"].ToString() ?? string.Empty,
                    Observacion = dr["Observacion"].ToString() ?? string.Empty,
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public void Registrar(AuditoriaAdministrativa auditoria)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            string query = @"
                INSERT INTO AuditoriaAdministrativa (Modulo, Accion, Entidad, IdEntidad, UsuarioSistema, Observacion, FechaRegistro)
                VALUES (@Modulo, @Accion, @Entidad, @IdEntidad, @UsuarioSistema, @Observacion, GETDATE())";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@Modulo", auditoria.Modulo);
            cmd.Parameters.AddWithValue("@Accion", auditoria.Accion);
            cmd.Parameters.AddWithValue("@Entidad", auditoria.Entidad);
            cmd.Parameters.AddWithValue("@IdEntidad", auditoria.IdEntidad == null ? DBNull.Value : auditoria.IdEntidad);
            cmd.Parameters.AddWithValue("@UsuarioSistema", auditoria.UsuarioSistema);
            cmd.Parameters.AddWithValue("@Observacion", auditoria.Observacion);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
