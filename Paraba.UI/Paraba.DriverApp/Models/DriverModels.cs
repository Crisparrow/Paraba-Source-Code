using System.Text.Json.Serialization;

namespace Paraba.DriverApp.Models;

public class DriverRequestCodeRequest
{
    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;
}

public class DriverVerifyCodeRequest
{
    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [JsonPropertyName("codigo")]
    public string Codigo { get; set; } = string.Empty;
}

public class DriverRegistrationDraftRequest
{
    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto { get; set; } = string.Empty;

    [JsonPropertyName("documentoIdentidad")]
    public string DocumentoIdentidad { get; set; } = string.Empty;

    [JsonPropertyName("correo")]
    public string Correo { get; set; } = string.Empty;

    [JsonPropertyName("licenciaConducir")]
    public string LicenciaConducir { get; set; } = string.Empty;

    [JsonPropertyName("fechaVencimientoLicencia")]
    public DateTime? FechaVencimientoLicencia { get; set; }

    [JsonPropertyName("idTipoServicio")]
    public int? IdTipoServicio { get; set; }

    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;

    [JsonPropertyName("marca")]
    public string Marca { get; set; } = string.Empty;

    [JsonPropertyName("modelo")]
    public string Modelo { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    [JsonPropertyName("anio")]
    public int? Anio { get; set; }
}

public class DriverRegistrationResponse
{
    [JsonPropertyName("idSolicitudRegistroConductor")]
    public int IdSolicitudRegistroConductor { get; set; }

    [JsonPropertyName("idConductor")]
    public int? IdConductor { get; set; }

    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [JsonPropertyName("nombreCompleto")]
    public string NombreCompleto { get; set; } = string.Empty;

    [JsonPropertyName("estadoSolicitud")]
    public string EstadoSolicitud { get; set; } = string.Empty;

    [JsonPropertyName("observacionRevision")]
    public string ObservacionRevision { get; set; } = string.Empty;

    [JsonPropertyName("datosConductorCompletos")]
    public bool DatosConductorCompletos { get; set; }

    [JsonPropertyName("datosVehiculoCompletos")]
    public bool DatosVehiculoCompletos { get; set; }

    [JsonPropertyName("documentosCompletos")]
    public bool DocumentosCompletos { get; set; }

    [JsonPropertyName("puedeOperar")]
    public bool PuedeOperar { get; set; }
}

public class DriverRequestCodeResponse
{
    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("canal")]
    public string Canal { get; set; } = string.Empty;

    [JsonPropertyName("codigoDemo")]
    public string? CodigoDemo { get; set; }

    [JsonPropertyName("solicitud")]
    public DriverRegistrationResponse Solicitud { get; set; } = new();
}

public class DriverVerifyCodeResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("solicitud")]
    public DriverRegistrationResponse Solicitud { get; set; } = new();
}
