using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.API.Services;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;

namespace Paraba.API.Controllers;

[ApiController]
[Route("api/viajes")]
public sealed class PassengerTripsController : ControllerBase
{
    private readonly ViajeAppService viajeAppService = new();
    private readonly TripRealtimePublisher realtimePublisher;

    public PassengerTripsController(TripRealtimePublisher realtimePublisher)
    {
        this.realtimePublisher = realtimePublisher;
    }

    [HttpPost("solicitudes")]
    public async Task<IActionResult> CreateRequest(PassengerTripRequest request)
    {
        try
        {
            int idViaje = viajeAppService.RegistrarSolicitud(new Viaje
            {
                IdPasajero = request.IdPasajero,
                IdConductor = request.IdConductor,
                IdVehiculo = request.IdVehiculo,
                IdTipoServicio = request.IdTipoServicio,
                Origen = request.Origen,
                Destino = request.Destino,
                TarifaSugerida = request.TarifaSugerida,
                TarifaOfertada = request.TarifaOfertada ?? request.TarifaSugerida
            });

            await realtimePublisher.PublishAsync(request.IdConductor, idViaje, "SolicitudCreada");

            return CreatedAtAction(nameof(GetTrip), new { idViaje }, new
            {
                idViaje,
                estado = "Solicitado",
                mensaje = "Solicitud enviada al conductor disponible."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{idViaje:int}")]
    public IActionResult GetTrip(int idViaje)
    {
        try
        {
            return Ok(viajeAppService.ObtenerViaje(idViaje));
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{idViaje:int}/contraoferta/respuesta")]
    public async Task<IActionResult> AnswerCounterOffer(int idViaje, PassengerCounterOfferDecisionRequest request)
    {
        try
        {
            Viaje trip = viajeAppService.ObtenerViaje(idViaje);

            if (!request.Aceptada)
            {
                viajeAppService.CancelarViaje(
                    trip.IdConductor,
                    idViaje,
                    string.IsNullOrWhiteSpace(request.Motivo)
                        ? "Contraoferta rechazada por el pasajero."
                        : request.Motivo);
                await realtimePublisher.PublishAsync(trip.IdConductor, idViaje, "ContraofertaRechazada");
                return Ok(new { mensaje = "Contraoferta rechazada y viaje cancelado." });
            }

            viajeAppService.AceptarContraofertaPasajero(trip.IdConductor, idViaje);
            await realtimePublisher.PublishAsync(trip.IdConductor, idViaje, "ContraofertaAceptada");
            return Ok(new { mensaje = "Contraoferta aceptada." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
