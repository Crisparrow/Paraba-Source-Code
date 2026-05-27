using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Soporte")]
    public class CalificacionController : Controller
    {
        private readonly CalificacionService calificacionService = new CalificacionService();

        public IActionResult Index()
        {
            var calificaciones = calificacionService.ListarCalificaciones();
            var calificacionesViewModel = calificaciones.Select(calificacion => new CalificacionViewModel
            {
                IdCalificacion = calificacion.IdCalificacion,
                IdViaje = calificacion.IdViaje,
                Pasajero = ObtenerNombrePasajero(calificacion.IdPasajero),
                Conductor = ObtenerNombreConductor(calificacion.IdConductor),
                Puntaje = calificacion.Puntaje,
                Comentario = calificacion.Comentario,
                Estado = calificacion.Estado,
                FechaRegistro = calificacion.FechaRegistro
            }).ToList();

            return View(calificacionesViewModel);
        }

        private static string ObtenerNombrePasajero(int idPasajero)
        {
            return idPasajero switch
            {
                1 => "Mariana Vargas",
                2 => "Jorge Salinas",
                _ => "Pasajero no identificado"
            };
        }

        private static string ObtenerNombreConductor(int idConductor)
        {
            return idConductor switch
            {
                1 => "Carlos Mendoza",
                2 => "Ana Rojas",
                _ => "Conductor no identificado"
            };
        }
    }
}
