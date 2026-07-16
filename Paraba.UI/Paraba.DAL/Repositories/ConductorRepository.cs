using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ConductorRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Conductor> Listar()
        {
            List<Conductor> lista = new List<Conductor>();

            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Conductor
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
                });
            }

            return lista;
        }

        public bool ActualizarVerificado(int idConductor, bool verificado)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_ActualizarVerificado", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@Verificado", verificado);

            cn.Open();

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());

            return filasAfectadas > 0;
        }

        public bool ActualizarEstado(int idConductor, bool estado)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_ActualizarEstado", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@Estado", estado);

            cn.Open();

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());

            return filasAfectadas > 0;
        }

        public bool ActualizarDisponibleApp(int idConductor, bool disponible)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Conductores_ActualizarDisponibleApp", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@Disponible", disponible);

            cn.Open();

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());

            return filasAfectadas > 0;
        }
    }
}
