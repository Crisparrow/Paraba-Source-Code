using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class UsuarioAdminRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<UsuarioAdmin> Listar()
        {
            List<UsuarioAdmin> usuarios = new List<UsuarioAdmin>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdUsuarioAdmin,
                    NombreCompleto,
                    Correo,
                    PasswordHash,
                    PasswordSalt,
                    PasswordIterations,
                    Estado,
                    IntentosFallidos,
                    UltimoAcceso,
                    FechaRegistro
                FROM UsuariosAdmin
                ORDER BY IdUsuarioAdmin";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                usuarios.Add(new UsuarioAdmin
                {
                    IdUsuarioAdmin = Convert.ToInt32(dr["IdUsuarioAdmin"]),
                    NombreCompleto = dr["NombreCompleto"].ToString() ?? string.Empty,
                    Correo = dr["Correo"].ToString() ?? string.Empty,
                    PasswordHash = dr["PasswordHash"].ToString() ?? string.Empty,
                    PasswordSalt = dr["PasswordSalt"].ToString() ?? string.Empty,
                    PasswordIterations = Convert.ToInt32(dr["PasswordIterations"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    IntentosFallidos = Convert.ToInt32(dr["IntentosFallidos"]),
                    UltimoAcceso = dr["UltimoAcceso"] == DBNull.Value ? null : Convert.ToDateTime(dr["UltimoAcceso"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return usuarios;
        }

        public UsuarioAdmin? ObtenerPorCorreo(string correo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdUsuarioAdmin,
                    NombreCompleto,
                    Correo,
                    PasswordHash,
                    PasswordSalt,
                    PasswordIterations,
                    Estado,
                    IntentosFallidos,
                    UltimoAcceso,
                    FechaRegistro
                FROM UsuariosAdmin
                WHERE Correo = @Correo";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@Correo", correo);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return new UsuarioAdmin
            {
                IdUsuarioAdmin = Convert.ToInt32(dr["IdUsuarioAdmin"]),
                NombreCompleto = dr["NombreCompleto"].ToString() ?? string.Empty,
                Correo = dr["Correo"].ToString() ?? string.Empty,
                PasswordHash = dr["PasswordHash"].ToString() ?? string.Empty,
                PasswordSalt = dr["PasswordSalt"].ToString() ?? string.Empty,
                PasswordIterations = Convert.ToInt32(dr["PasswordIterations"]),
                Estado = Convert.ToBoolean(dr["Estado"]),
                IntentosFallidos = Convert.ToInt32(dr["IntentosFallidos"]),
                UltimoAcceso = dr["UltimoAcceso"] == DBNull.Value ? null : Convert.ToDateTime(dr["UltimoAcceso"]),
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
            };
        }

        public List<string> ListarRoles(int idUsuarioAdmin)
        {
            List<string> roles = new List<string>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT r.Nombre
                FROM UsuariosAdminRoles ur
                INNER JOIN RolesAdmin r ON ur.IdRolAdmin = r.IdRolAdmin
                WHERE ur.IdUsuarioAdmin = @IdUsuarioAdmin
                    AND r.Estado = 1
                ORDER BY r.Nombre";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdUsuarioAdmin", idUsuarioAdmin);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                roles.Add(dr["Nombre"].ToString() ?? string.Empty);
            }

            return roles;
        }

        public List<RolAdmin> ListarRolesAdmin()
        {
            List<RolAdmin> roles = new List<RolAdmin>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdRolAdmin,
                    Nombre,
                    Descripcion,
                    Estado,
                    FechaRegistro
                FROM RolesAdmin
                WHERE Estado = 1
                ORDER BY IdRolAdmin";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                roles.Add(new RolAdmin
                {
                    IdRolAdmin = Convert.ToInt32(dr["IdRolAdmin"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Descripcion = dr["Descripcion"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return roles;
        }

        public int Crear(UsuarioAdmin usuario)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                INSERT INTO UsuariosAdmin
                (
                    NombreCompleto,
                    Correo,
                    PasswordHash,
                    PasswordSalt,
                    PasswordIterations,
                    Estado,
                    FechaRegistro
                )
                OUTPUT INSERTED.IdUsuarioAdmin
                VALUES
                (
                    @NombreCompleto,
                    @Correo,
                    @PasswordHash,
                    @PasswordSalt,
                    @PasswordIterations,
                    @Estado,
                    @FechaRegistro
                )";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
            cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
            cmd.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", usuario.PasswordSalt);
            cmd.Parameters.AddWithValue("@PasswordIterations", usuario.PasswordIterations);
            cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
            cmd.Parameters.AddWithValue("@FechaRegistro", usuario.FechaRegistro);

            cn.Open();

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AsignarRol(int idUsuarioAdmin, int idRolAdmin)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                IF NOT EXISTS (
                    SELECT 1
                    FROM UsuariosAdminRoles
                    WHERE IdUsuarioAdmin = @IdUsuarioAdmin
                        AND IdRolAdmin = @IdRolAdmin
                )
                BEGIN
                    INSERT INTO UsuariosAdminRoles
                    (
                        IdUsuarioAdmin,
                        IdRolAdmin,
                        FechaRegistro
                    )
                    VALUES
                    (
                        @IdUsuarioAdmin,
                        @IdRolAdmin,
                        GETDATE()
                    )
                END";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdUsuarioAdmin", idUsuarioAdmin);
            cmd.Parameters.AddWithValue("@IdRolAdmin", idRolAdmin);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void ActualizarEstado(int idUsuarioAdmin, bool estado)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE UsuariosAdmin
                SET Estado = @Estado
                WHERE IdUsuarioAdmin = @IdUsuarioAdmin";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdUsuarioAdmin", idUsuarioAdmin);
            cmd.Parameters.AddWithValue("@Estado", estado);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void RegistrarAcceso(AuditoriaAccesoAdmin auditoria)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                INSERT INTO AuditoriaAccesosAdmin
                (
                    IdUsuarioAdmin,
                    Correo,
                    Accion,
                    Exitoso,
                    IpOrigen,
                    Observacion,
                    FechaRegistro
                )
                VALUES
                (
                    @IdUsuarioAdmin,
                    @Correo,
                    @Accion,
                    @Exitoso,
                    @IpOrigen,
                    @Observacion,
                    @FechaRegistro
                )";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdUsuarioAdmin", auditoria.IdUsuarioAdmin ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Correo", auditoria.Correo);
            cmd.Parameters.AddWithValue("@Accion", auditoria.Accion);
            cmd.Parameters.AddWithValue("@Exitoso", auditoria.Exitoso);
            cmd.Parameters.AddWithValue("@IpOrigen", auditoria.IpOrigen);
            cmd.Parameters.AddWithValue("@Observacion", auditoria.Observacion);
            cmd.Parameters.AddWithValue("@FechaRegistro", auditoria.FechaRegistro);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void MarcarAccesoCorrecto(int idUsuarioAdmin)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE UsuariosAdmin
                SET
                    UltimoAcceso = GETDATE(),
                    IntentosFallidos = 0
                WHERE IdUsuarioAdmin = @IdUsuarioAdmin";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdUsuarioAdmin", idUsuarioAdmin);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void RegistrarIntentoFallido(int idUsuarioAdmin)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE UsuariosAdmin
                SET IntentosFallidos = IntentosFallidos + 1
                WHERE IdUsuarioAdmin = @IdUsuarioAdmin";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdUsuarioAdmin", idUsuarioAdmin);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
