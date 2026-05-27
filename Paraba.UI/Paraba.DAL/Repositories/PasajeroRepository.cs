using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class PasajeroRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Pasajero> Listar()
        {
            List<Pasajero> lista = new List<Pasajero>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdPasajero,
                    NombreCompleto,
                    DocumentoIdentidad,
                    Telefono,
                    Correo,
                    Verificado,
                    Estado,
                    FechaRegistro
                FROM Pasajeros
                ORDER BY IdPasajero";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Pasajero
                {
                    IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                    NombreCompleto = dr["NombreCompleto"].ToString() ?? string.Empty,
                    DocumentoIdentidad = dr["DocumentoIdentidad"].ToString() ?? string.Empty,
                    Telefono = dr["Telefono"].ToString() ?? string.Empty,
                    Correo = dr["Correo"].ToString() ?? string.Empty,
                    Verificado = Convert.ToBoolean(dr["Verificado"]),
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
