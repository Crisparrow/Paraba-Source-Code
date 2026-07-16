using System.Text.Json.Serialization;

namespace Paraba.DriverApp.Models;

public class DriverOperationsSummaryResponse
{
    [JsonPropertyName("idConductor")]
    public int IdConductor { get; set; }

    [JsonPropertyName("conectado")]
    public bool Conectado { get; set; }

    [JsonPropertyName("prioridad")]
    public int Prioridad { get; set; }

    [JsonPropertyName("pedidosDisponibles")]
    public int PedidosDisponibles { get; set; }

    [JsonPropertyName("viajesActivos")]
    public int ViajesActivos { get; set; }

    [JsonPropertyName("viajesHoy")]
    public int ViajesHoy { get; set; }

    [JsonPropertyName("viajesFinalizadosHoy")]
    public int ViajesFinalizadosHoy { get; set; }

    [JsonPropertyName("gananciaHoy")]
    public decimal GananciaHoy { get; set; }

    [JsonPropertyName("objetivoTitulo")]
    public string ObjetivoTitulo { get; set; } = string.Empty;

    [JsonPropertyName("objetivoDetalle")]
    public string ObjetivoDetalle { get; set; } = string.Empty;

    [JsonPropertyName("objetivoActual")]
    public decimal ObjetivoActual { get; set; }

    [JsonPropertyName("objetivoMeta")]
    public decimal ObjetivoMeta { get; set; }

    [JsonPropertyName("estadoOperativo")]
    public string EstadoOperativo { get; set; } = string.Empty;
}

public class DriverTripResponse
{
    [JsonPropertyName("idViaje")]
    public int IdViaje { get; set; }

    [JsonPropertyName("idPasajero")]
    public int IdPasajero { get; set; }

    [JsonPropertyName("idConductor")]
    public int IdConductor { get; set; }

    [JsonPropertyName("idVehiculo")]
    public int IdVehiculo { get; set; }

    [JsonPropertyName("idTipoServicio")]
    public int IdTipoServicio { get; set; }

    [JsonPropertyName("tipoServicio")]
    public string TipoServicio { get; set; } = string.Empty;

    [JsonPropertyName("origen")]
    public string Origen { get; set; } = string.Empty;

    [JsonPropertyName("destino")]
    public string Destino { get; set; } = string.Empty;

    [JsonPropertyName("tarifaSugerida")]
    public decimal TarifaSugerida { get; set; }

    [JsonPropertyName("tarifaOfertada")]
    public decimal TarifaOfertada { get; set; }

    [JsonPropertyName("tarifaContraoferta")]
    public decimal? TarifaContraoferta { get; set; }

    [JsonPropertyName("tarifaAceptada")]
    public decimal? TarifaAceptada { get; set; }

    [JsonPropertyName("tarifaFinal")]
    public decimal TarifaFinal { get; set; }

    [JsonPropertyName("idEstadoViaje")]
    public int IdEstadoViaje { get; set; }

    [JsonPropertyName("estadoViaje")]
    public string EstadoViaje { get; set; } = string.Empty;

    [JsonPropertyName("fechaSolicitud")]
    public DateTime FechaSolicitud { get; set; }

    [JsonPropertyName("fechaAceptacion")]
    public DateTime? FechaAceptacion { get; set; }

    [JsonPropertyName("fechaInicio")]
    public DateTime? FechaInicio { get; set; }

    [JsonPropertyName("fechaFin")]
    public DateTime? FechaFin { get; set; }

    [JsonPropertyName("fechaCancelacion")]
    public DateTime? FechaCancelacion { get; set; }

    [JsonPropertyName("motivoCancelacion")]
    public string MotivoCancelacion { get; set; } = string.Empty;
}

public class DriverCounterOfferRequest
{
    [JsonPropertyName("tarifaContraoferta")]
    public decimal TarifaContraoferta { get; set; }
}

public class DriverCancelTripRequest
{
    [JsonPropertyName("motivo")]
    public string Motivo { get; set; } = string.Empty;
}

public class DriverDemoTripRequest
{
    [JsonPropertyName("idTipoServicio")]
    public int? IdTipoServicio { get; set; }
}

public class DriverAvailabilityRequest
{
    [JsonPropertyName("disponible")]
    public bool Disponible { get; set; }
}
