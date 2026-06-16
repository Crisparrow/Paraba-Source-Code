using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Soporte,Operaciones")]
    public class SoporteController : Controller
    {
        private readonly ViajeAdminService viajeAdminService = new ViajeAdminService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly CalificacionService calificacionService = new CalificacionService();

        public IActionResult Index()
        {
            var viajes = viajeAdminService.ListarViajes();
            var pasajeros = pasajeroService.ListarPasajeros();
            var conductores = conductorService.ListarConductores();
            var calificaciones = calificacionService.ListarCalificaciones()
                .Where(item => item.Estado)
                .ToList();

            var casos = new List<SoporteCasoViewModel>();

            casos.AddRange(viajes
                .Where(item => item.IdEstadoViaje == 5)
                .Select(item => new SoporteCasoViewModel
                {
                    TipoCaso = "Viaje cancelado",
                    IdViaje = item.IdViaje,
                    Pasajero = pasajeros.FirstOrDefault(pasajero => pasajero.IdPasajero == item.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                    Conductor = conductores.FirstOrDefault(conductor => conductor.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                    Ruta = $"{item.Origen} -> {item.Destino}",
                    Detalle = "Revisar motivo, frecuencia y posible reclamo del usuario.",
                    Prioridad = "Media",
                    Fecha = item.FechaSolicitud
                }));

            casos.AddRange(calificaciones
                .Where(item => item.Puntaje <= 2)
                .Select(item =>
                {
                    var viaje = viajes.FirstOrDefault(viaje => viaje.IdViaje == item.IdViaje);

                    return new SoporteCasoViewModel
                    {
                        TipoCaso = "Calificacion baja",
                        IdViaje = item.IdViaje,
                        Pasajero = pasajeros.FirstOrDefault(pasajero => pasajero.IdPasajero == item.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                        Conductor = conductores.FirstOrDefault(conductor => conductor.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                        Ruta = viaje == null ? "-" : $"{viaje.Origen} -> {viaje.Destino}",
                        Detalle = string.IsNullOrWhiteSpace(item.Comentario) ? $"Puntaje {item.Puntaje}/5 sin comentario." : item.Comentario,
                        Prioridad = item.Puntaje == 1 ? "Alta" : "Media",
                        Fecha = item.FechaRegistro
                    };
                }));

            casos.AddRange(viajes
                .Where(item => item.IdEstadoViaje == 3 && item.FechaInicio != null && item.FechaInicio.Value < DateTime.Now.AddHours(-3))
                .Select(item => new SoporteCasoViewModel
                {
                    TipoCaso = "Viaje en curso prolongado",
                    IdViaje = item.IdViaje,
                    Pasajero = pasajeros.FirstOrDefault(pasajero => pasajero.IdPasajero == item.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                    Conductor = conductores.FirstOrDefault(conductor => conductor.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                    Ruta = $"{item.Origen} -> {item.Destino}",
                    Detalle = "Viaje en curso por mas de 3 horas. Confirmar estado real.",
                    Prioridad = "Alta",
                    Fecha = item.FechaInicio!.Value
                }));

            var viewModel = new SoporteOperativoViewModel
            {
                ViajesCancelados = viajes.Count(item => item.IdEstadoViaje == 5),
                CalificacionesBajas = calificaciones.Count(item => item.Puntaje <= 2),
                ViajesSinCierre = viajes.Count(item => item.IdEstadoViaje == 3 && item.FechaInicio != null && item.FechaInicio.Value < DateTime.Now.AddHours(-3)),
                ConductoresSuspendidos = conductores.Count(item => !item.Estado),
                Casos = casos
                    .OrderByDescending(item => item.Prioridad == "Alta")
                    .ThenByDescending(item => item.Fecha)
                    .ToList()
            };

            return View(viewModel);
        }
    }
}

