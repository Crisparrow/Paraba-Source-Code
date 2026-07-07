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

                return Ok(DriverRegistrationResponse.FromEntity(solicitud));
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

                return Ok(DriverRegistrationResponse.FromEntity(solicitud));
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

                return Ok(DriverRegistrationResponse.FromEntity(solicitud));
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
