using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Soporte")]
    public class CalificacionController : Controller
    {
        private readonly CalificacionService calificacionService = new CalificacionService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly ConductorService conductorService = new ConductorService();

        public IActionResult Index()
        {
            var pasajeros = pasajeroService.ListarPasajeros();
            var conductores = conductorService.ListarConductores();
            var calificaciones = calificacionService.ListarCalificaciones();
            var calificacionesViewModel = calificaciones.Select(calificacion => new CalificacionViewModel
            {
                IdCalificacion = calificacion.IdCalificacion,
                IdViaje = calificacion.IdViaje,
                Pasajero = pasajeros.FirstOrDefault(item => item.IdPasajero == calificacion.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                Conductor = conductores.FirstOrDefault(item => item.IdConductor == calificacion.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                Puntaje = calificacion.Puntaje,
                Comentario = calificacion.Comentario,
                Estado = calificacion.Estado,
                FechaRegistro = calificacion.FechaRegistro
            }).ToList();

            return View(calificacionesViewModel);
        }
    }
}
