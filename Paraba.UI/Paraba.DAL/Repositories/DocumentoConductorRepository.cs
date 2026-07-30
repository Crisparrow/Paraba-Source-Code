using System.Data;
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

            using SqlCommand cmd = new SqlCommand("dbo.sp_DocumentosConductor_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

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
                    EsVigente = Convert.ToBoolean(dr["EsVigente"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public DocumentoConductor? ObtenerPorId(int idDocumentoConductor)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_DocumentosConductor_ObtenerPorId", cn);
            cmd.CommandType = CommandType.StoredProcedure;
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
                EsVigente = Convert.ToBoolean(dr["EsVigente"]),
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
            };
        }

        public bool ActualizarEstadoVerificacion(int idDocumentoConductor, string estadoVerificacion, string observacion)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_DocumentosConductor_ActualizarEstadoVerificacion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdDocumentoConductor", idDocumentoConductor);
            cmd.Parameters.AddWithValue("@EstadoVerificacion", estadoVerificacion);
            cmd.Parameters.AddWithValue("@Observacion", observacion);

            cn.Open();

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());

            return filasAfectadas > 0;
        }

        public int Crear(DocumentoConductor documento)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_DocumentosConductor_Crear", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@IdConductor", documento.IdConductor);
            cmd.Parameters.AddWithValue("@TipoDocumento", documento.TipoDocumento);
            cmd.Parameters.AddWithValue("@NumeroDocumento", documento.NumeroDocumento);
            cmd.Parameters.AddWithValue("@UrlArchivo", documento.UrlArchivo);
            cmd.Parameters.AddWithValue("@FechaVencimiento", (object?)documento.FechaVencimiento ?? DBNull.Value);

            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
