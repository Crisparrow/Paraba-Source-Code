using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;
using System.Data;

namespace Paraba.DAL.Repositories
{
    public class ReclamoRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Reclamo> Listar()
        {
            List<Reclamo> lista = new List<Reclamo>();
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Reclamos_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Reclamo
                {
                    IdReclamo = Convert.ToInt32(dr["IdReclamo"]),
                    IdViaje = dr["IdViaje"] == DBNull.Value ? null : Convert.ToInt32(dr["IdViaje"]),
                    IdPasajero = dr["IdPasajero"] == DBNull.Value ? null : Convert.ToInt32(dr["IdPasajero"]),
                    IdConductor = dr["IdConductor"] == DBNull.Value ? null : Convert.ToInt32(dr["IdConductor"]),
                    TipoReclamo = dr["TipoReclamo"].ToString() ?? string.Empty,
                    Descripcion = dr["Descripcion"].ToString() ?? string.Empty,
                    Estado = dr["Estado"].ToString() ?? string.Empty,
                    Prioridad = dr["Prioridad"].ToString() ?? string.Empty,
                    UsuarioRegistro = dr["UsuarioRegistro"].ToString() ?? string.Empty,
                    UsuarioCierre = dr["UsuarioCierre"].ToString() ?? string.Empty,
                    ObservacionCierre = dr["ObservacionCierre"].ToString() ?? string.Empty,
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                    FechaCierre = dr["FechaCierre"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaCierre"])
                });
            }

            return lista;
        }

        public void Registrar(Reclamo reclamo)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Reclamos_Registrar", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdViaje", reclamo.IdViaje == null ? DBNull.Value : reclamo.IdViaje);
            cmd.Parameters.AddWithValue("@IdPasajero", reclamo.IdPasajero == null ? DBNull.Value : reclamo.IdPasajero);
            cmd.Parameters.AddWithValue("@IdConductor", reclamo.IdConductor == null ? DBNull.Value : reclamo.IdConductor);
            cmd.Parameters.AddWithValue("@TipoReclamo", reclamo.TipoReclamo);
            cmd.Parameters.AddWithValue("@Descripcion", reclamo.Descripcion);
            cmd.Parameters.AddWithValue("@Prioridad", reclamo.Prioridad);
            cmd.Parameters.AddWithValue("@UsuarioRegistro", reclamo.UsuarioRegistro);

            cn.Open();
            cmd.ExecuteScalar();
        }

        public void Cerrar(int idReclamo, string estado, string usuarioCierre, string observacionCierre)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("dbo.sp_Reclamos_Cerrar", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdReclamo", idReclamo);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@UsuarioCierre", usuarioCierre);
            cmd.Parameters.AddWithValue("@ObservacionCierre", observacionCierre);

            cn.Open();
            cmd.ExecuteScalar();
        }
    }
}
