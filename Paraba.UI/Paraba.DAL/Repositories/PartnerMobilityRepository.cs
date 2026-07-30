using System.Data;
using Microsoft.Data.SqlClient;
using Paraba.DAL.Connections;
using Paraba.ENTITY.Models;

namespace Paraba.DAL.Repositories;

public sealed class PartnerMobilityRepository
{
    private readonly ConexionDAL conexion = new();

    public List<RutaMicrobus> ListRoutes()
    {
        var result = new List<RutaMicrobus>();

        using SqlConnection connection = conexion.ObtenerConexion();
        using SqlCommand command = new("dbo.sp_RutasMicrobus_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new RutaMicrobus
            {
                IdRutaMicrobus = Convert.ToInt32(reader["IdRutaMicrobus"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Origen = reader["Origen"].ToString() ?? string.Empty,
                Destino = reader["Destino"].ToString() ?? string.Empty,
                Recorrido = reader["Recorrido"].ToString() ?? string.Empty,
                TarifaPasajeBs = Convert.ToDecimal(reader["TarifaPasajeBs"]),
                SuscripcionMensualChoferUsd = Convert.ToDecimal(reader["SuscripcionMensualChoferUsd"]),
                ChoferesSuscritos = Convert.ToInt32(reader["ChoferesSuscritos"]),
                Estado = Convert.ToBoolean(reader["Estado"]),
                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
            });
        }

        return result;
    }

    public List<AsociacionMototaxi> ListAssociations()
    {
        var result = new List<AsociacionMototaxi>();

        using SqlConnection connection = conexion.ObtenerConexion();
        using SqlCommand command = new("dbo.sp_AsociacionesMototaxi_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new AsociacionMototaxi
            {
                IdAsociacionMototaxi = Convert.ToInt32(reader["IdAsociacionMototaxi"]),
                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                Parada = reader["Parada"].ToString() ?? string.Empty,
                CostoMensualUsd = Convert.ToDecimal(reader["CostoMensualUsd"]),
                CuposTotales = Convert.ToInt32(reader["CuposTotales"]),
                CuposOcupados = Convert.ToInt32(reader["CuposOcupados"]),
                Estado = Convert.ToBoolean(reader["Estado"]),
                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
            });
        }

        return result;
    }

    public int SubscribeDriverToRoute(int routeId, int driverId, DateTime periodStart, string paymentStatus)
    {
        return ExecuteScalar(
            "dbo.sp_Microbus_SuscribirConductor",
            ("@IdRutaMicrobus", routeId),
            ("@IdConductor", driverId),
            ("@PeriodoInicio", periodStart.Date),
            ("@EstadoPago", paymentStatus));
    }

    public int AssignAssociationSlot(int associationId, int driverId, int slotNumber, DateTime periodStart, string paymentStatus)
    {
        return ExecuteScalar(
            "dbo.sp_AsociacionMototaxi_AsignarRanura",
            ("@IdAsociacionMototaxi", associationId),
            ("@IdConductor", driverId),
            ("@NumeroRanura", slotNumber),
            ("@PeriodoInicio", periodStart.Date),
            ("@EstadoPago", paymentStatus));
    }

    private int ExecuteScalar(string storedProcedure, params (string Name, object Value)[] parameters)
    {
        using SqlConnection connection = conexion.ObtenerConexion();
        using SqlCommand command = new(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        foreach ((string name, object value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        connection.Open();
        object? result = command.ExecuteScalar();
        return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
    }
}
