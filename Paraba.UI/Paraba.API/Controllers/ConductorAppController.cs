using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;

namespace Paraba.API.Controllers
{
    [ApiController]
    [Route("api/conductores")]
    public class ConductorAppController : ControllerBase
    {
        private readonly ConductorService conductorService = new ConductorService();
        private readonly VehiculoService vehiculoService = new VehiculoService();
        private readonly DocumentoConductorService documentoConductorService = new DocumentoConductorService();
        private readonly ViajeAppService viajeAppService = new ViajeAppService();

        [HttpGet("{idConductor:int}/perfil")]
        public IActionResult ObtenerPerfil(int idConductor)
        {
            Conductor? conductor = conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == idConductor);

            if (conductor == null)
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            var response = new DriverProfileResponse
            {
                IdConductor = conductor.IdConductor,
                NombreCompleto = conductor.NombreCompleto,
                Telefono = conductor.Telefono,
                Correo = conductor.Correo,
                Disponible = conductor.Disponible,
                Verificado = conductor.Verificado,
                Activo = conductor.Estado,
                Vehiculos = vehiculoService.ListarVehiculos()
                    .Where(item => item.IdConductor == idConductor)
                    .Select(MapVehicle)
                    .ToList(),
                Documentos = documentoConductorService.ListarDocumentos()
                    .Where(item => item.IdConductor == idConductor)
                    .Select(MapDocument)
                    .ToList()
            };

            return Ok(response);
        }

        [HttpGet("{idConductor:int}/viajes")]
        public IActionResult ListarViajes(int idConductor)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            var viajes = viajeAppService.ListarViajes()
                .Where(item => item.IdConductor == idConductor)
                .OrderByDescending(item => item.FechaSolicitud)
                .Select(MapTrip)
                .ToList();

            return Ok(viajes);
        }

        [HttpGet("{idConductor:int}/viajes/activos")]
        public IActionResult ListarViajesActivos(int idConductor)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            var viajes = viajeAppService.ListarViajes()
                .Where(item => item.IdConductor == idConductor &&
                    item.IdEstadoViaje != 4 &&
                    item.IdEstadoViaje != 5)
                .OrderByDescending(item => item.FechaSolicitud)
                .Select(MapTrip)
                .ToList();

            return Ok(viajes);
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/contraoferta")]
        public IActionResult RegistrarContraoferta(int idConductor, int idViaje, DriverCounterOfferRequest request)
        {
            IActionResult? validation = ValidarViajeDelConductor(idConductor, idViaje);

            if (validation != null)
            {
                return validation;
            }

            try
            {
                viajeAppService.RegistrarContraoferta(idViaje, request.TarifaContraoferta);
                return Ok(new { mensaje = "Contraoferta registrada correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/iniciar")]
        public IActionResult IniciarViaje(int idConductor, int idViaje)
        {
            IActionResult? validation = ValidarViajeDelConductor(idConductor, idViaje);

            if (validation != null)
            {
                return validation;
            }

            try
            {
                viajeAppService.IniciarViaje(idViaje);
                return Ok(new { mensaje = "Viaje iniciado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/finalizar")]
        public IActionResult FinalizarViaje(int idConductor, int idViaje)
        {
            IActionResult? validation = ValidarViajeDelConductor(idConductor, idViaje);

            if (validation != null)
            {
                return validation;
            }

            try
            {
                viajeAppService.FinalizarViaje(idViaje);
                return Ok(new { mensaje = "Viaje finalizado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/cancelar")]
        public IActionResult CancelarViaje(int idConductor, int idViaje, DriverCancelTripRequest request)
        {
            IActionResult? validation = ValidarViajeDelConductor(idConductor, idViaje);

            if (validation != null)
            {
                return validation;
            }

            try
            {
                viajeAppService.CancelarViaje(idViaje, request.Motivo);
                return Ok(new { mensaje = "Viaje cancelado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        private IActionResult? ValidarViajeDelConductor(int idConductor, int idViaje)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            Viaje? viaje = viajeAppService.ListarViajes()
                .FirstOrDefault(item => item.IdViaje == idViaje);

            if (viaje == null)
            {
                return NotFound(new { mensaje = "Viaje no encontrado." });
            }

            if (viaje.IdConductor != idConductor)
            {
                return Forbid();
            }

            return null;
        }

        private bool ExisteConductor(int idConductor)
        {
            return conductorService.ListarConductores()
                .Any(item => item.IdConductor == idConductor);
        }

        private static DriverVehicleResponse MapVehicle(Vehiculo vehiculo)
        {
            return new DriverVehicleResponse
            {
                IdVehiculo = vehiculo.IdVehiculo,
                IdTipoServicio = vehiculo.IdTipoServicio,
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Color = vehiculo.Color,
                Verificado = vehiculo.Verificado,
                Activo = vehiculo.Estado
            };
        }

        private static DriverDocumentResponse MapDocument(DocumentoConductor documento)
        {
            return new DriverDocumentResponse
            {
                IdDocumentoConductor = documento.IdDocumentoConductor,
                TipoDocumento = documento.TipoDocumento,
                NumeroDocumento = documento.NumeroDocumento,
                EstadoVerificacion = documento.EstadoVerificacion,
                FechaVencimiento = documento.FechaVencimiento,
                Observacion = documento.Observacion
            };
        }

        private static DriverTripResponse MapTrip(Viaje viaje)
        {
            return new DriverTripResponse
            {
                IdViaje = viaje.IdViaje,
                IdPasajero = viaje.IdPasajero,
                IdConductor = viaje.IdConductor,
                IdVehiculo = viaje.IdVehiculo,
                IdTipoServicio = viaje.IdTipoServicio,
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaContraoferta = viaje.TarifaContraoferta,
                TarifaAceptada = viaje.TarifaAceptada,
                TarifaFinal = viaje.TarifaFinal,
                IdEstadoViaje = viaje.IdEstadoViaje,
                EstadoViaje = ObtenerEstadoViaje(viaje.IdEstadoViaje),
                FechaSolicitud = viaje.FechaSolicitud,
                FechaInicio = viaje.FechaInicio,
                FechaFin = viaje.FechaFin
            };
        }

        private static string ObtenerEstadoViaje(int idEstadoViaje)
        {
            return idEstadoViaje switch
            {
                1 => "Solicitado",
                2 => "Aceptado",
                3 => "En curso",
                4 => "Finalizado",
                5 => "Cancelado",
                _ => "Desconocido"
            };
        }
    }
}
