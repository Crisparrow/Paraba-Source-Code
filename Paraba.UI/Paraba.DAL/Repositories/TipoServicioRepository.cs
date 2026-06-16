using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class TipoServicioRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<TipoServicio> Listar()
        {
            List<TipoServicio> lista = new List<TipoServicio>();

            using SqlConnection cn = conexion.ObtenerConexion();

            using SqlCommand cmd = new SqlCommand("dbo.sp_TiposServicio_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new TipoServicio
                {
                    IdTipoServicio = Convert.ToInt32(dr["IdTipoServicio"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"])
                });
            }

            return lista;
        }
    }
}
