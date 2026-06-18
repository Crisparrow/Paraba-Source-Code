using System.Text.Json.Serialization;

namespace Paraba.DriverApp.Models;

public class DriverProfileResponse
{
    [JsonPropertyName("idConductor")]
    public int IdConductor { get; set; }

    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto { get; set; } = string.Empty;

    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("disponible")]
    public bool Disponible { get; set; }

    [JsonPropertyName("verificado")]
    public bool Verificado { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }

    [JsonPropertyName("vehiculos")]
    public List<DriverVehicleResponse> Vehiculos { get; set; } = new();

    [JsonPropertyName("documentos")]
    public List<DriverDocumentResponse> Documentos { get; set; } = new();
}

public class DriverVehicleResponse
{
    [JsonPropertyName("idVehiculo")]
    public int IdVehiculo { get; set; }

    [JsonPropertyName("idTipoServicio")]
    public int IdTipoServicio { get; set; }

    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;

    [JsonPropertyName("marca")]
    public string Marca { get; set; } = string.Empty;

    [JsonPropertyName("modelo")]
    public string Modelo { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    [JsonPropertyName("verificado")]
    public bool Verificado { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }
}

public class DriverDocumentResponse
{
    [JsonPropertyName("idDocumentoConductor")]
    public int IdDocumentoConductor { get; set; }

    [JsonPropertyName("tipoDocumento")]
    public string TipoDocumento { get; set; } = string.Empty;

    [JsonPropertyName("estadoVerificacion")]
    public string EstadoVerificacion { get; set; } = string.Empty;
}

public class DriverTripResponse
{
    [JsonPropertyName("idViaje")]
    public int IdViaje { get; set; }

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

    [JsonPropertyName("estadoViaje")]
    public string EstadoViaje { get; set; } = string.Empty;
}
