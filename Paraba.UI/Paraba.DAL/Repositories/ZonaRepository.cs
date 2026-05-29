using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories
{
    public class ZonaRepository
    {
        private readonly ConexionDAL conexion = new ConexionDAL();

        public List<Zona> Listar()
        {
            List<Zona> lista = new List<Zona>();

            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                SELECT
                    IdZona,
                    IdCiudad,
                    Nombre,
                    Descripcion,
                    Estado,
                    CoberturaActiva,
                    EsZonaRiesgo,
                    AltaDemanda,
                    ObservacionOperativa,
                    FechaRegistro
                FROM Zonas
                ORDER BY IdZona";

            using SqlCommand cmd = new SqlCommand(query, cn);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Zona
                {
                    IdZona = Convert.ToInt32(dr["IdZona"]),
                    IdCiudad = Convert.ToInt32(dr["IdCiudad"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Descripcion = dr["Descripcion"].ToString() ?? string.Empty,
                    Estado = Convert.ToBoolean(dr["Estado"]),
                    CoberturaActiva = Convert.ToBoolean(dr["CoberturaActiva"]),
                    EsZonaRiesgo = Convert.ToBoolean(dr["EsZonaRiesgo"]),
                    AltaDemanda = Convert.ToBoolean(dr["AltaDemanda"]),
                    ObservacionOperativa = dr["ObservacionOperativa"].ToString() ?? string.Empty,
                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                });
            }

            return lista;
        }

        public void ActualizarOperacion(Zona zona)
        {
            using SqlConnection cn = conexion.ObtenerConexion();

            string query = @"
                UPDATE Zonas
                SET
                    CoberturaActiva = @CoberturaActiva,
                    EsZonaRiesgo = @EsZonaRiesgo,
                    AltaDemanda = @AltaDemanda,
                    ObservacionOperativa = @ObservacionOperativa
                WHERE IdZona = @IdZona";

            using SqlCommand cmd = new SqlCommand(query, cn);
            cmd.Parameters.AddWithValue("@IdZona", zona.IdZona);
            cmd.Parameters.AddWithValue("@CoberturaActiva", zona.CoberturaActiva);
            cmd.Parameters.AddWithValue("@EsZonaRiesgo", zona.EsZonaRiesgo);
            cmd.Parameters.AddWithValue("@AltaDemanda", zona.AltaDemanda);
            cmd.Parameters.AddWithValue("@ObservacionOperativa", zona.ObservacionOperativa);

            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
