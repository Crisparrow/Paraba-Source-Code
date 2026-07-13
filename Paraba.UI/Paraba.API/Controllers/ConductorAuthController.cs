using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;

namespace Paraba.API.Controllers
{
    [ApiController]
    [Route("api/conductores/auth")]
    public class ConductorAuthController : ControllerBase
    {
        private readonly RegistroConductorService registroConductorService = new RegistroConductorService();
        private readonly IWebHostEnvironment webHostEnvironment;

        public ConductorAuthController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("solicitar-codigo")]
        public IActionResult SolicitarCodigo(DriverRequestCodeRequest request)
        {
            try
            {
                var result = registroConductorService.SolicitarCodigo(request.Telefono);

                return Ok(new DriverRequestCodeResponse
                {
                    Mensaje = "Codigo de verificacion generado correctamente.",
                    Canal = "Demo",
                    CodigoDemo = result.CodigoDemo,
                    Solicitud = DriverRegistrationResponse.FromEntity(result.Solicitud)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("verificar-codigo")]
        public IActionResult VerificarCodigo(DriverVerifyCodeRequest request)
        {
            try
            {
                var result = registroConductorService.VerificarCodigo(request.Telefono, request.Codigo);

                return Ok(new DriverVerifyCodeResponse
                {
                    Token = result.Token,
                    Solicitud = DriverRegistrationResponse.FromEntity(result.Solicitud)
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("solicitud")]
        public IActionResult ObtenerSolicitud([FromQuery] string telefono)
        {
            try
            {
                SolicitudRegistroConductor solicitud = registroConductorService.ObtenerSolicitud(telefono, ObtenerToken());

                var documentos = registroConductorService.ListarDocumentos(solicitud.IdSolicitudRegistroConductor);

                return Ok(DriverRegistrationResponse.FromEntity(solicitud, documentos));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        [HttpPut("solicitud/borrador")]
        public IActionResult GuardarBorrador(DriverRegistrationDraftRequest request)
        {
            try
            {
                SolicitudRegistroConductor solicitud = registroConductorService.GuardarBorrador(
                    new SolicitudRegistroConductor
                    {
                        Telefono = request.Telefono,
                        NombreCompleto = request.NombreCompleto,
                        DocumentoIdentidad = request.DocumentoIdentidad,
                        Correo = request.Correo,
                        LicenciaConducir = request.LicenciaConducir,
                        FechaVencimientoLicencia = request.FechaVencimientoLicencia,
                        IdTipoServicio = request.IdTipoServicio,
                        Placa = request.Placa,
                        Marca = request.Marca,
                        Modelo = request.Modelo,
                        Color = request.Color,
                        Anio = request.Anio
                    },
                    ObtenerToken());

                var documentos = registroConductorService.ListarDocumentos(solicitud.IdSolicitudRegistroConductor);

                return Ok(DriverRegistrationResponse.FromEntity(solicitud, documentos));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        [HttpPost("solicitud/enviar")]
        public IActionResult EnviarRevision(DriverRequestCodeRequest request)
        {
            try
            {
                SolicitudRegistroConductor solicitud = registroConductorService.EnviarRevision(request.Telefono, ObtenerToken());

                var documentos = registroConductorService.ListarDocumentos(solicitud.IdSolicitudRegistroConductor);

                return Ok(DriverRegistrationResponse.FromEntity(solicitud, documentos));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        [HttpPost("solicitud/documentos")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> GuardarDocumento([FromForm] DriverRegistrationDocumentUploadRequest request)
        {
            try
            {
                if (request.Archivo == null || request.Archivo.Length == 0)
                {
                    return BadRequest(new { mensaje = "Debe adjuntar un archivo." });
                }

                string extension = Path.GetExtension(request.Archivo.FileName).ToLowerInvariant();
                string[] extensionesPermitidas = [".jpg", ".jpeg", ".png", ".pdf"];

                if (!extensionesPermitidas.Contains(extension))
                {
                    return BadRequest(new { mensaje = "Solo se permiten archivos JPG, PNG o PDF." });
                }

                SolicitudRegistroConductor solicitud = registroConductorService.ObtenerSolicitud(request.Telefono, ObtenerToken());
                string fileName = $"{request.TipoDocumento}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
                string relativeDirectory = Path.Combine("uploads", "conductores", solicitud.IdSolicitudRegistroConductor.ToString());
                string webRootPath = webHostEnvironment.WebRootPath ?? Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");
                string absoluteDirectory = Path.Combine(webRootPath, relativeDirectory);

                Directory.CreateDirectory(absoluteDirectory);

                string absolutePath = Path.Combine(absoluteDirectory, fileName);

                await using (FileStream stream = System.IO.File.Create(absolutePath))
                {
                    await request.Archivo.CopyToAsync(stream);
                }

                string relativeUrl = "/" + Path.Combine(relativeDirectory, fileName).Replace("\\", "/");
                var documentos = registroConductorService.GuardarDocumento(
                    request.Telefono,
                    ObtenerToken(),
                    request.TipoDocumento,
                    request.NumeroDocumento,
                    relativeUrl,
                    request.FechaVencimiento);

                return Ok(documentos.Select(DriverRegistrationDocumentResponse.FromEntity).ToList());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        private string ObtenerToken()
        {
            string authorization = Request.Headers.Authorization.ToString();

            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authorization["Bearer ".Length..].Trim();
            }

            return string.Empty;
        }
    }
}
