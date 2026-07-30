using System.Text.Json.Serialization;

namespace Paraba.DriverApp.Models;

public sealed class DriverProfileResponse
{
    [JsonPropertyName("idConductor")] public int IdConductor { get; set; }
    [JsonPropertyName("nombreCompleto")] public string NombreCompleto { get; set; } = string.Empty;
    [JsonPropertyName("telefono")] public string Telefono { get; set; } = string.Empty;
    [JsonPropertyName("correo")] public string Correo { get; set; } = string.Empty;
    [JsonPropertyName("disponible")] public bool Disponible { get; set; }
    [JsonPropertyName("verificado")] public bool Verificado { get; set; }
    [JsonPropertyName("activo")] public bool Activo { get; set; }
    [JsonPropertyName("estadoAprobacion")] public string EstadoAprobacion { get; set; } = string.Empty;
    [JsonPropertyName("puedeTrabajar")] public bool PuedeTrabajar { get; set; }
    [JsonPropertyName("vehiculos")] public List<DriverVehicleResponse> Vehiculos { get; set; } = new();
    [JsonPropertyName("documentos")] public List<DriverDocumentResponse> Documentos { get; set; } = new();
}

public sealed class DriverVehicleResponse
{
    [JsonPropertyName("idVehiculo")] public int IdVehiculo { get; set; }
    [JsonPropertyName("idTipoServicio")] public int IdTipoServicio { get; set; }
    [JsonPropertyName("tipoServicio")] public string TipoServicio { get; set; } = string.Empty;
    [JsonPropertyName("categoriaVehiculo")] public string CategoriaVehiculo { get; set; } = string.Empty;
    [JsonPropertyName("placa")] public string Placa { get; set; } = string.Empty;
    [JsonPropertyName("marca")] public string Marca { get; set; } = string.Empty;
    [JsonPropertyName("modelo")] public string Modelo { get; set; } = string.Empty;
    [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
    [JsonPropertyName("anio")] public int Anio { get; set; }
    [JsonPropertyName("estadoVerificacion")] public string EstadoVerificacion { get; set; } = string.Empty;
    [JsonPropertyName("observacion")] public string Observacion { get; set; } = string.Empty;
    [JsonPropertyName("verificado")] public bool Verificado { get; set; }
    [JsonPropertyName("activo")] public bool Activo { get; set; }
}

public sealed class DriverDocumentResponse
{
    [JsonPropertyName("idDocumentoConductor")] public int IdDocumentoConductor { get; set; }
    [JsonPropertyName("tipoDocumento")] public string TipoDocumento { get; set; } = string.Empty;
    [JsonPropertyName("numeroDocumento")] public string NumeroDocumento { get; set; } = string.Empty;
    [JsonPropertyName("urlArchivo")] public string UrlArchivo { get; set; } = string.Empty;
    [JsonPropertyName("estadoVerificacion")] public string EstadoVerificacion { get; set; } = string.Empty;
    [JsonPropertyName("fechaVencimiento")] public DateTime? FechaVencimiento { get; set; }
    [JsonPropertyName("observacion")] public string Observacion { get; set; } = string.Empty;
}

public sealed class DriverServiceTypeResponse
{
    [JsonPropertyName("idTipoServicio")] public int IdTipoServicio { get; set; }
    [JsonPropertyName("nombre")] public string Nombre { get; set; } = string.Empty;
    [JsonPropertyName("categoriaVehiculo")] public string CategoriaVehiculo { get; set; } = string.Empty;
    public override string ToString() => $"{Nombre} ({CategoriaVehiculo})";
}

public sealed class DriverVehicleCreateRequest
{
    [JsonPropertyName("idTipoServicio")] public int IdTipoServicio { get; set; }
    [JsonPropertyName("placa")] public string Placa { get; set; } = string.Empty;
    [JsonPropertyName("marca")] public string Marca { get; set; } = string.Empty;
    [JsonPropertyName("modelo")] public string Modelo { get; set; } = string.Empty;
    [JsonPropertyName("color")] public string Color { get; set; } = string.Empty;
    [JsonPropertyName("anio")] public int Anio { get; set; }
}
