using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class DocumentoConductorRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<DocumentoConductor> Listar()
        {
            List<DocumentoConductor> lista = new List<DocumentoConductor>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdDocumentoConductor,
                    IdConductor,
                    TipoDocumento,
                    NumeroDocumento,
                    UrlArchivo,
                    FechaVencimiento,
                    EstadoVerificacion,
                    Observacion,
                    FechaRegistro
                FROM DocumentosConductor
                ORDER BY IdDocumentoConductor";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new DocumentoConductor
                {
                    IdDocumentoConductor = Convert.ToInt32(dr["IdDocumentoConductor"]),
                    IdConductor = Convert.ToInt32(dr["IdConductor"]),
                    TipoDocumento = dr["TipoDocumento"].ToString() ?? string.Empty,
                    NumeroDocumento = dr["NumeroDocumento"].ToString() ?? string.Empty,
                    UrlArchivo = dr["UrlArchivo"].ToString() ?? string.Empty,
                    FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaVencimiento"]),
                    EstadoVerificacion = dr["EstadoVerificacion"].ToString() ?? string.Empty,
                    Observacion = dr["Observacion"].ToString() ?? string.Empty,
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public DocumentoConductor? ObtenerPorId(int idDocumentoConductor)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdDocumentoConductor,
                    IdConductor,
                    TipoDocumento,
                    NumeroDocumento,
                    UrlArchivo,
                    FechaVencimiento,
                    EstadoVerificacion,
                    Observacion,
                    FechaRegistro
                FROM DocumentosConductor
                WHERE IdDocumentoConductor = @IdDocumentoConductor";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdDocumentoConductor", idDocumentoConductor);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return new DocumentoConductor
            {
                IdDocumentoConductor = Convert.ToInt32(dr["IdDocumentoConductor"]),
                IdConductor = Convert.ToInt32(dr["IdConductor"]),
                TipoDocumento = dr["TipoDocumento"].ToString() ?? string.Empty,
                NumeroDocumento = dr["NumeroDocumento"].ToString() ?? string.Empty,
                UrlArchivo = dr["UrlArchivo"].ToString() ?? string.Empty,
                FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaVencimiento"]),
                EstadoVerificacion = dr["EstadoVerificacion"].ToString() ?? string.Empty,
                Observacion = dr["Observacion"].ToString() ?? string.Empty,
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
            };
        }

        public bool ActualizarEstadoVerificacion(int idDocumentoConductor, string estadoVerificacion, string observacion)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE DocumentosConductor
                SET
                    EstadoVerificacion = @EstadoVerificacion,
                    Observacion = @Observacion
                WHERE IdDocumentoConductor = @IdDocumentoConductor";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdDocumentoConductor", idDocumentoConductor);
            cmd.Parameters.AddWithValue("@EstadoVerificacion", estadoVerificacion);
            cmd.Parameters.AddWithValue("@Observacion", observacion);

            cn.Open();

            int filasAfectadas = cmd.ExecuteNonQuery();

            return filasAfectadas > 0;
        }
    }
}
