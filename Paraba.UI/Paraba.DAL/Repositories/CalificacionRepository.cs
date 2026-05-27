using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class CalificacionRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Calificacion> Listar()
        {
            List<Calificacion> lista = new List<Calificacion>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdCalificacion,
                    IdViaje,
                    IdPasajero,
                    IdConductor,
                    Puntaje,
                    Comentario,
                    Estado,
                    FechaRegistro
                FROM Calificaciones
                ORDER BY IdCalificacion";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Calificacion
                {
                    IdCalificacion = Convert.ToInt32(dr["IdCalificacion"]),
                    IdViaje = Convert.ToInt32(dr["IdViaje"]),
                    IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                    IdConductor = Convert.ToInt32(dr["IdConductor"]),
                    Puntaje = Convert.ToInt32(dr["Puntaje"]),
                    Comentario = dr["Comentario"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
