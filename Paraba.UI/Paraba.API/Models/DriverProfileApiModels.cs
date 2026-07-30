using Microsoft.AspNetCore.Http;

namespace Paraba.API.Models;

public sealed class DriverVehicleCreateRequest
{
    public int IdTipoServicio { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Anio { get; set; }
}

public sealed class DriverDocumentUploadRequest
{
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime? FechaVencimiento { get; set; }
    public IFormFile? Archivo { get; set; }
}

public sealed class DriverServiceTypeResponse
{
    public int IdTipoServicio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CategoriaVehiculo { get; set; } = string.Empty;
}
