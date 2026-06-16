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

            string query = @"
                UPDATE Conductores
                SET Verificado = @Verificado
                WHERE IdConductor = @IdConductor";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@Verificado", verificado);

            cn.Open();

            int filasAfectadas = cmd.ExecuteNonQuery();

            return filasAfectadas > 0;
        }

        public bool ActualizarEstado(int idConductor, bool estado)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Conductores
                SET
                    Estado = @Estado,
                    Disponible = CASE WHEN @Estado = 0 THEN 0 ELSE Disponible END
                WHERE IdConductor = @IdConductor";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdConductor", idConductor);
            cmd.Parameters.AddWithValue("@Estado", estado);

            cn.Open();

            int filasAfectadas = cmd.ExecuteNonQuery();

            return filasAfectadas > 0;
        }
    }
}
