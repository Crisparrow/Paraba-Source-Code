using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class DepartamentoRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Departamento> Listar()
        {
            List<Departamento> lista = new List<Departamento>();

            using SqlConnection cn = conexion.ObtenerConexion();
            string query = "SELECT IdDepartamento, IdPais, Nombre, Estado, FechaRegistro FROM Departamentos ORDER BY IdDepartamento";
            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Departamento
                {
                    IdDepartamento = Convert.ToInt32(dr["IdDepartamento"]),
                    IdPais = Convert.ToInt32(dr["IdPais"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }
    }
}
