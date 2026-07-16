using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ConductorAuthRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public void CrearOtp(string telefono, string codigoHash, string canal, DateTime fechaExpiracion)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_OtpVerificaciones_Crear", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@CodigoHash", codigoHash);
            cmd.Parameters.AddWithValue("@Canal", canal);
            cmd.Parameters.AddWithValue("@FechaExpiracion", fechaExpiracion);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public OtpVerificacion? ObtenerOtpActivo(string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_OtpVerificaciones_ObtenerActivo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return MapOtp(dr);
        }

        public OtpVerificacion? ObtenerOtpVerificadoReciente(string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_OtpVerificaciones_ObtenerVerificadoReciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return MapOtp(dr);
        }

        public void RegistrarIntentoOtp(int idOtpVerificacion, bool verificado)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_OtpVerificaciones_RegistrarIntento", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdOtpVerificacion", idOtpVerificacion);
            cmd.Parameters.AddWithValue("@Verificado", verificado);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void MarcarOtpUsado(int idOtpVerificacion)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_OtpVerificaciones_MarcarUsado", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdOtpVerificacion", idOtpVerificacion);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public Conductor? ObtenerConductorPorTelefono(string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_ObtenerPorTelefono", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return MapConductor(dr);
        }

        public int RegistrarConductorBasico(string nombreCompleto, string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_RegistrarBasico", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public string CrearSesionConductor(int idConductor, string dispositivo, DateTime fechaExpiracion)
        {
            string tokenSesion = Guid.NewGuid().ToString("N");

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_SesionesConductor_Crear", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@TokenSesion", tokenSesion);
            cmd.Parameters.AddWithValue("@Dispositivo", dispositivo);
            cmd.Parameters.AddWithValue("@FechaExpiracion", fechaExpiracion);

            cn.Open();
            cmd.ExecuteNonQuery();

            return tokenSesion;
        }

        private static OtpVerificacion MapOtp(SqlDataReader dr)
        {
            return new OtpVerificacion
            {
                IdOtpVerificacion = Convert.ToInt32(dr["IdOtpVerificacion"]),
                Telefono = dr["Telefono"].ToString() ?? string.Empty,
                CodigoHash = dr["CodigoHash"].ToString() ?? string.Empty,
                Canal = dr["Canal"].ToString() ?? string.Empty,
                FechaExpiracion = Convert.ToDateTime(dr["FechaExpiracion"]),
                Verificado = Convert.ToBoolean(dr["Verificado"]),
                Usado = Convert.ToBoolean(dr["Usado"]),
                Intentos = Convert.ToInt32(dr["Intentos"]),
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                FechaVerificacion = dr["FechaVerificacion"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaVerificacion"])
            };
        }

        private static Conductor MapConductor(SqlDataReader dr)
        {
            return new Conductor
            {
                IdConductor = Convert.ToInt32(dr["IdConductor"]),
                NombreCompleto = dr["NombreCompleto"].ToString() ?? string.Empty,
                DocumentoIdentidad = dr["DocumentoIdentidad"].ToString() ?? string.Empty,
                Telefono = dr["Telefono"].ToString() ?? string.Empty,
                Correo = dr["Correo"].ToString() ?? string.Empty,
                LicenciaConducir = dr["LicenciaConducir"].ToString() ?? string.Empty,
                FechaVencimientoLicencia = Convert.ToDateTime(dr["FechaVencimientoLicencia"]),
                Disponible = Convert.ToBoolean(dr["Disponible"]),
                Verificado = Convert.ToBoolean(dr["Verificado"]),
                Estado = Convert.ToBoolean(dr["Estado"]),
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
            };
        }
    }
}
