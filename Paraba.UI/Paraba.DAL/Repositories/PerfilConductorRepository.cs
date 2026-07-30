using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;

namespace Paraba.DAL.Repositories;

public sealed class PerfilConductorRepository
{
    private readonly ConexionDAL conexion = new();

    public bool RecalcularAprobacion(int idConductor)
    {
        using SqlConnection cn = conexion.ObtenerConexion();
        using SqlCommand cmd = new("dbo.sp_Conductores_RecalcularAprobacionPerfil", cn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@IdConductor", idConductor);
        cn.Open();
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }
}
