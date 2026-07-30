using Microsoft.AspNetCore.Mvc;
using Paraba.API.Models;
using Paraba.BLL.Services;

namespace Paraba.API.Controllers;

[ApiController]
[Route("api/tipos-servicio")]
public sealed class ServiceTypesApiController : ControllerBase
{
    private readonly TipoServicioService service = new();

    [HttpGet]
    public IActionResult Get() => Ok(service.ListarTiposServicio()
        .Where(item => item.Estado)
        .Select(item => new DriverServiceTypeResponse
        {
            IdTipoServicio = item.IdTipoServicio,
            Nombre = item.Nombre,
            CategoriaVehiculo = item.CategoriaVehiculo
        }));
}
