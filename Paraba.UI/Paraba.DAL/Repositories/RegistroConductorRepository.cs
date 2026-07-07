using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class RegistroConductorRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public SolicitudRegistroConductor? ObtenerPorTelefono(string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_ObtenerPorTelefono", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            return dr.Read() ? MapSolicitud(dr) : null;
        }

        public SolicitudRegistroConductor CrearOBuscarSolicitud(string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_CrearOBuscarSolicitud", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                throw new InvalidOperationException("No se pudo crear o recuperar la solicitud de registro.");
            }

            return MapSolicitud(dr);
        }

        public SolicitudRegistroConductor GuardarBorrador(SolicitudRegistroConductor solicitud)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_GuardarBorrador", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Telefono", solicitud.Telefono);
            cmd.Parameters.AddWithValue("@NombreCompleto", solicitud.NombreCompleto);
            cmd.Parameters.AddWithValue("@DocumentoIdentidad", solicitud.DocumentoIdentidad);
            cmd.Parameters.AddWithValue("@Correo", solicitud.Correo);
            cmd.Parameters.AddWithValue("@LicenciaConducir", solicitud.LicenciaConducir);
            cmd.Parameters.AddWithValue("@FechaVencimientoLicencia", solicitud.FechaVencimientoLicencia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdTipoServicio", solicitud.IdTipoServicio ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Placa", solicitud.Placa);
            cmd.Parameters.AddWithValue("@Marca", solicitud.Marca);
            cmd.Parameters.AddWithValue("@Modelo", solicitud.Modelo);
            cmd.Parameters.AddWithValue("@Color", solicitud.Color);
            cmd.Parameters.AddWithValue("@Anio", solicitud.Anio ?? (object)DBNull.Value);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                throw new InvalidOperationException("No se pudo guardar el borrador de registro.");
            }

            return MapSolicitud(dr);
        }

        public SolicitudRegistroConductor EnviarRevision(string telefono)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_EnviarRevision", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                throw new InvalidOperationException("No se pudo enviar la solicitud a revision.");
            }

            return MapSolicitud(dr);
        }

        public void RegistrarCodigo(string telefono, string codigo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_RegistrarCodigo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@Codigo", codigo);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool VerificarCodigo(string telefono, string codigo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_VerificarCodigo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@Codigo", codigo);

            cn.Open();

            object? result = cmd.ExecuteScalar();

            return result != null && Convert.ToBoolean(result);
        }

        public void CrearSesion(string telefono, string token)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_CrearSesion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@Token", token);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool ValidarSesion(string telefono, string token)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_RegistroConductor_ValidarSesion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@Token", token);

            cn.Open();

            object? result = cmd.ExecuteScalar();

            return result != null && Convert.ToBoolean(result);
        }

        private static SolicitudRegistroConductor MapSolicitud(SqlDataReader dr)
        {
            return new SolicitudRegistroConductor
            {
                IdSolicitudRegistroConductor = Convert.ToInt32(dr["IdSolicitudRegistroConductor"]),
                IdConductor = dr["IdConductor"] == DBNull.Value ? null : Convert.ToInt32(dr["IdConductor"]),
                Telefono = dr["Telefono"].ToString() ?? string.Empty,
                NombreCompleto = dr["NombreCompleto"].ToString() ?? string.Empty,
                DocumentoIdentidad = dr["DocumentoIdentidad"].ToString() ?? string.Empty,
                Correo = dr["Correo"].ToString() ?? string.Empty,
                LicenciaConducir = dr["LicenciaConducir"].ToString() ?? string.Empty,
                FechaVencimientoLicencia = dr["FechaVencimientoLicencia"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaVencimientoLicencia"]),
                IdTipoServicio = dr["IdTipoServicio"] == DBNull.Value ? null : Convert.ToInt32(dr["IdTipoServicio"]),
                Placa = dr["Placa"].ToString() ?? string.Empty,
                Marca = dr["Marca"].ToString() ?? string.Empty,
                Modelo = dr["Modelo"].ToString() ?? string.Empty,
                Color = dr["Color"].ToString() ?? string.Empty,
                Anio = dr["Anio"] == DBNull.Value ? null : Convert.ToInt32(dr["Anio"]),
                EstadoSolicitud = dr["EstadoSolicitud"].ToString() ?? string.Empty,
                ObservacionRevision = dr["ObservacionRevision"].ToString() ?? string.Empty,
                FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                FechaActualizacion = Convert.ToDateTime(dr["FechaActualizacion"]),
                FechaEnvio = dr["FechaEnvio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaEnvio"]),
                FechaRevision = dr["FechaRevision"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaRevision"])
            };
        }
    }
}
