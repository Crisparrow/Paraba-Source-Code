using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.API.Services;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;

namespace Paraba.API.Controllers;

[ApiController]
[Route("api/conductores/{idConductor:int}")]
public sealed class DriverProfileController : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".pdf"
    };
    private static readonly HashSet<string> AllowedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "CedulaIdentidad", "LicenciaConducir", "FotoVerificacion", "DocumentoVehiculo", "RUAT", "SOAT", "DocumentoMicrobus"
    };

    private readonly DriverProfileService profileService = new();
    private readonly TipoServicioService serviceTypeService = new();
    private readonly DriverFileStorage fileStorage;

    public DriverProfileController(DriverFileStorage fileStorage)
    {
        this.fileStorage = fileStorage;
    }

    [HttpGet("vehiculos")]
    public IActionResult GetVehicles(int idConductor)
    {
        try
        {
            Dictionary<int, TipoServicio> serviceTypes = serviceTypeService.ListarTiposServicio()
                .ToDictionary(item => item.IdTipoServicio);
            return Ok(profileService.ListarVehiculos(idConductor).Select(item => MapVehicle(item, serviceTypes)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("vehiculos")]
    public IActionResult CreateVehicle(int idConductor, DriverVehicleCreateRequest request)
    {
        try
        {
            int idVehiculo = profileService.RegistrarVehiculo(new Vehiculo
            {
                IdConductor = idConductor,
                IdTipoServicio = request.IdTipoServicio,
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Color = request.Color,
                Anio = request.Anio
            });
            return Created($"/api/conductores/{idConductor}/vehiculos", new
            {
                idVehiculo,
                estadoVerificacion = "Pendiente",
                mensaje = "Vehiculo registrado y enviado a revision administrativa."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("documentos")]
    public IActionResult GetDocuments(int idConductor)
    {
        try
        {
            return Ok(profileService.ListarDocumentos(idConductor).Select(MapDocument));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("documentos")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UploadDocument(int idConductor, [FromForm] DriverDocumentUploadRequest request)
    {
        try
        {
            if (request.Archivo == null || request.Archivo.Length == 0)
            {
                return BadRequest(new { mensaje = "Debe adjuntar una foto o un documento." });
            }

            if (!AllowedDocumentTypes.Contains(request.TipoDocumento))
            {
                return BadRequest(new { mensaje = "El tipo de documento no esta permitido." });
            }

            if (request.Archivo.Length > 10_000_000)
            {
                return BadRequest(new { mensaje = "El archivo no puede superar 10 MB." });
            }

            string extension = Path.GetExtension(request.Archivo.FileName);
            if (!AllowedExtensions.Contains(extension))
            {
                return BadRequest(new { mensaje = "Solo se permiten archivos JPG, PNG o PDF." });
            }

            if (request.TipoDocumento.Equals("FotoVerificacion", StringComparison.OrdinalIgnoreCase) && extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { mensaje = "La verificacion por foto debe ser JPG o PNG." });
            }

            string url = await fileStorage.SaveAsync(idConductor, request.TipoDocumento, request.Archivo);
            int idDocumento = profileService.RegistrarDocumento(new DocumentoConductor
            {
                IdConductor = idConductor,
                TipoDocumento = request.TipoDocumento,
                NumeroDocumento = request.NumeroDocumento,
                FechaVencimiento = request.FechaVencimiento,
                UrlArchivo = url
            });

            return Created($"/api/conductores/{idConductor}/documentos", new
            {
                idDocumentoConductor = idDocumento,
                estadoVerificacion = "Pendiente",
                almacenamiento = fileStorage.UsesAzure ? "AzureBlobStorage" : "LocalDevelopment",
                urlArchivo = url,
                mensaje = "Documento guardado y enviado a revision administrativa."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Azure.RequestFailedException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { mensaje = $"Azure Blob Storage no esta disponible: {ex.Message}" });
        }
    }

    private static object MapVehicle(Vehiculo vehicle, IReadOnlyDictionary<int, TipoServicio> serviceTypes)
    {
        serviceTypes.TryGetValue(vehicle.IdTipoServicio, out TipoServicio? serviceType);
        return new
        {
            vehicle.IdVehiculo,
            vehicle.IdTipoServicio,
            tipoServicio = serviceType?.Nombre ?? "Servicio no identificado",
            categoriaVehiculo = serviceType?.CategoriaVehiculo ?? string.Empty,
            vehicle.Placa,
            vehicle.Marca,
            vehicle.Modelo,
            vehicle.Color,
            vehicle.Anio,
            vehicle.EstadoVerificacion,
            vehicle.Observacion,
            activo = vehicle.Estado,
            vehicle.FechaRegistro
        };
    }

    private static object MapDocument(DocumentoConductor document) => new
    {
        document.IdDocumentoConductor,
        document.TipoDocumento,
        document.NumeroDocumento,
        document.UrlArchivo,
        document.FechaVencimiento,
        document.EstadoVerificacion,
        document.Observacion,
        document.FechaRegistro
    };
}
