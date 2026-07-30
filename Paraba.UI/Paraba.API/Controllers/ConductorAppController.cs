using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.API.Services;
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
        private readonly TripRealtimePublisher realtimePublisher;

        public ConductorAppController(TripRealtimePublisher realtimePublisher)
        {
            this.realtimePublisher = realtimePublisher;
        }

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
                EstadoAprobacion = conductor.Verificado ? "Aprobado" : "Pendiente o requiere correccion",
                PuedeTrabajar = conductor.Verificado && vehiculoService.ListarVehiculos()
                    .Any(item => item.IdConductor == idConductor && item.Estado && item.EstadoVerificacion == "Aprobado"),
                Vehiculos = vehiculoService.ListarVehiculos()
                    .Where(item => item.IdConductor == idConductor)
                    .Select(MapVehicle)
                    .ToList(),
                Documentos = documentoConductorService.ListarDocumentos()
                    .Where(item => item.IdConductor == idConductor && item.EsVigente)
                    .Select(MapDocument)
                    .ToList()
            };

            return Ok(response);
        }

        [HttpGet("{idConductor:int}/operacion/resumen")]
        public IActionResult ObtenerResumenOperacion(int idConductor)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                return Ok(MapOperationsSummary(viajeAppService.ObtenerResumenOperacion(idConductor)));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{idConductor:int}/disponibilidad")]
        public IActionResult ActualizarDisponibilidad(int idConductor, DriverAvailabilityRequest request)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                conductorService.ActualizarDisponibilidadApp(idConductor, request.Disponible);
                return Ok(new { mensaje = request.Disponible ? "Conductor conectado." : "Conductor desconectado." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
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

            var viajes = viajeAppService.ListarViajesActivos(idConductor)
                .Select(MapTrip)
                .ToList();

            return Ok(viajes);
        }

        [HttpGet("{idConductor:int}/viajes/disponibles")]
        public IActionResult ListarViajesDisponibles(int idConductor)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            var viajes = viajeAppService.ListarViajesDisponibles(idConductor)
                .Select(MapTrip)
                .ToList();

            return Ok(viajes);
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/aceptar")]
        public async Task<IActionResult> AceptarViaje(int idConductor, int idViaje)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                viajeAppService.AceptarViaje(idConductor, idViaje);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "ViajeAceptado");
                return Ok(new { mensaje = "Viaje aceptado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/contraoferta")]
        public async Task<IActionResult> RegistrarContraoferta(int idConductor, int idViaje, DriverCounterOfferRequest request)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                viajeAppService.RegistrarContraoferta(idConductor, idViaje, request.TarifaContraoferta);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "ContraofertaCreada");
                return Ok(new { mensaje = "Contraoferta registrada correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/iniciar")]
        public async Task<IActionResult> IniciarViaje(int idConductor, int idViaje)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                viajeAppService.IniciarViaje(idConductor, idViaje);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "ViajeIniciado");
                return Ok(new { mensaje = "Viaje iniciado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/finalizar")]
        public async Task<IActionResult> FinalizarViaje(int idConductor, int idViaje)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                viajeAppService.FinalizarViaje(idConductor, idViaje);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "ViajeFinalizado");
                return Ok(new { mensaje = "Viaje finalizado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/cancelar")]
        public async Task<IActionResult> CancelarViaje(int idConductor, int idViaje, DriverCancelTripRequest request)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                viajeAppService.CancelarViaje(idConductor, idViaje, request.Motivo);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "ViajeCancelado");
                return Ok(new { mensaje = "Viaje cancelado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/viajes/{idViaje:int}/demo/aceptar-contraoferta")]
        public async Task<IActionResult> AceptarContraofertaPasajeroDemo(int idConductor, int idViaje)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                viajeAppService.AceptarContraofertaPasajero(idConductor, idViaje);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "ContraofertaAceptada");
                return Ok(new { mensaje = "El pasajero demo acepto la contraoferta." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("{idConductor:int}/demo/viajes")]
        public async Task<IActionResult> CrearViajeDemo(int idConductor, DriverDemoTripRequest request)
        {
            if (!ExisteConductor(idConductor))
            {
                return NotFound(new { mensaje = "Conductor no encontrado." });
            }

            try
            {
                int idViaje = viajeAppService.CrearSolicitudDemo(idConductor, request.IdTipoServicio);
                await realtimePublisher.PublishAsync(idConductor, idViaje, "SolicitudCreada");
                return Ok(new { mensaje = "Pedido demo creado correctamente.", idViaje });
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
            TipoServicio? tipoServicio = new TipoServicioService().ListarTiposServicio()
                .FirstOrDefault(item => item.IdTipoServicio == vehiculo.IdTipoServicio);
            return new DriverVehicleResponse
            {
                IdVehiculo = vehiculo.IdVehiculo,
                IdTipoServicio = vehiculo.IdTipoServicio,
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Color = vehiculo.Color,
                Anio = vehiculo.Anio,
                TipoServicio = tipoServicio?.Nombre ?? "Servicio no identificado",
                CategoriaVehiculo = tipoServicio?.CategoriaVehiculo ?? string.Empty,
                EstadoVerificacion = vehiculo.EstadoVerificacion,
                Observacion = vehiculo.Observacion,
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
                UrlArchivo = documento.UrlArchivo,
                EstadoVerificacion = documento.EstadoVerificacion,
                FechaVencimiento = documento.FechaVencimiento,
                Observacion = documento.Observacion
            };
        }

        private static DriverOperationsSummaryResponse MapOperationsSummary(ResumenOperacionConductor resumen)
        {
            return new DriverOperationsSummaryResponse
            {
                IdConductor = resumen.IdConductor,
                Conectado = resumen.Conectado,
                Prioridad = resumen.Prioridad,
                PedidosDisponibles = resumen.PedidosDisponibles,
                ViajesActivos = resumen.ViajesActivos,
                ViajesHoy = resumen.ViajesHoy,
                ViajesFinalizadosHoy = resumen.ViajesFinalizadosHoy,
                GananciaHoy = resumen.GananciaHoy,
                ObjetivoTitulo = resumen.ObjetivoTitulo,
                ObjetivoDetalle = resumen.ObjetivoDetalle,
                ObjetivoActual = resumen.ObjetivoActual,
                ObjetivoMeta = resumen.ObjetivoMeta,
                EstadoOperativo = resumen.EstadoOperativo
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
                TipoServicio = string.IsNullOrWhiteSpace(viaje.TipoServicio) ? "Servicio" : viaje.TipoServicio,
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaContraoferta = viaje.TarifaContraoferta,
                TarifaAceptada = viaje.TarifaAceptada,
                TarifaFinal = viaje.TarifaFinal,
                IdEstadoViaje = viaje.IdEstadoViaje,
                EstadoViaje = string.IsNullOrWhiteSpace(viaje.EstadoViaje) ? ObtenerEstadoViaje(viaje.IdEstadoViaje) : viaje.EstadoViaje,
                FechaSolicitud = viaje.FechaSolicitud,
                FechaAceptacion = viaje.FechaAceptacion,
                FechaInicio = viaje.FechaInicio,
                FechaFin = viaje.FechaFin,
                FechaCancelacion = viaje.FechaCancelacion,
                MotivoCancelacion = viaje.MotivoCancelacion
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
                6 => "Contraofertado",
                7 => "En camino al pasajero",
                _ => "Desconocido"
            };
        }
    }
}
