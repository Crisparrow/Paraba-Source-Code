using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.BLL.Services;

namespace Paraba.API.Controllers;

[ApiController]
[Route("api/servicios-aliados")]
public sealed class PartnerMobilityController : ControllerBase
{
    private readonly PartnerMobilityService service = new();

    [HttpGet("microbuses/rutas")]
    public IActionResult ListRoutes() => Ok(service.ListRoutes());

    [HttpPost("microbuses/rutas/{routeId:int}/suscripciones")]
    public IActionResult SubscribeDriver(int routeId, MonthlyDriverSubscriptionRequest request)
    {
        try
        {
            int id = service.SubscribeDriverToRoute(routeId, request.IdConductor, request.PeriodoInicio, request.EstadoPago);
            return Ok(new { idSuscripcion = id, montoMensualUsd = 50m, mensaje = "Suscripcion mensual registrada." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("mototaxis/asociaciones")]
    public IActionResult ListAssociations() => Ok(service.ListAssociations());

    [HttpPost("mototaxis/asociaciones/{associationId:int}/ranuras")]
    public IActionResult AssignSlot(int associationId, MototaxiSlotRequest request)
    {
        try
        {
            int id = service.AssignAssociationSlot(
                associationId,
                request.IdConductor,
                request.NumeroRanura,
                request.PeriodoInicio,
                request.EstadoPago);
            return Ok(new { idAsignacion = id, costoMensualUsd = 50m, mensaje = "Ranura de mototaxi asignada." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
