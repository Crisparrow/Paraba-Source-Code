using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Paraba.BLL.Services;
using Paraba.UI.Models;
using Paraba.UI.ViewModels;
using System.Diagnostics;

namespace Paraba.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ViajeService viajeService = new ViajeService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly DocumentoConductorService documentoConductorService = new DocumentoConductorService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly CalificacionService calificacionService = new CalificacionService();
        private readonly LiquidacionConductorService liquidacionConductorService = new LiquidacionConductorService();

        public IActionResult Index()
        {
            var viajes = viajeService.ListarViajes();
            var conductores = conductorService.ListarConductores();
            var documentos = documentoConductorService.ListarDocumentos();
            var pasajeros = pasajeroService.ListarPasajeros();
            var calificaciones = calificacionService.ListarCalificaciones()
                .Where(item => item.Estado)
                .ToList();
            var liquidaciones = liquidacionConductorService.ListarLiquidaciones();
            var idsViajesLiquidados = liquidacionConductorService.ListarIdsViajesLiquidados();
            var hoy = DateTime.Today;
            var viajesPendientesLiquidacion = viajes
                .Where(item => item.IdEstadoViaje == 4 &&
                    item.FechaFin != null &&
                    !idsViajesLiquidados.Contains(item.IdViaje))
                .ToList();

            var dashboard = new DashboardViewModel
            {
                TotalViajes = viajes.Count,
                ViajesSolicitados = viajes.Count(item => item.IdEstadoViaje == 1),
                ViajesAceptados = viajes.Count(item => item.IdEstadoViaje == 2),
                ViajesEnCurso = viajes.Count(item => item.IdEstadoViaje == 3),
                ViajesFinalizados = viajes.Count(item => item.IdEstadoViaje == 4),
                ViajesCancelados = viajes.Count(item => item.IdEstadoViaje == 5),
                ViajesHoy = viajes.Count(item => item.FechaSolicitud.Date == hoy),
                ViajesFinalizadosHoy = viajes.Count(item => item.IdEstadoViaje == 4 && item.FechaFin != null && item.FechaFin.Value.Date == hoy),
                ViajesCanceladosHoy = viajes.Count(item => item.IdEstadoViaje == 5 && item.FechaSolicitud.Date == hoy),
                IngresosFinalizados = viajes
                    .Where(item => item.IdEstadoViaje == 4)
                    .Sum(item => item.TarifaFinal),
                IngresosHoy = viajes
                    .Where(item => item.IdEstadoViaje == 4 && item.FechaFin != null && item.FechaFin.Value.Date == hoy)
                    .Sum(item => item.TarifaFinal),
                ConductoresActivos = conductores.Count(item => item.Estado),
                ConductoresVerificados = conductores.Count(item => item.Estado && item.Verificado),
                ConductoresPendientes = conductores.Count(item => item.Estado && !item.Verificado),
                DocumentosPendientes = documentos.Count(item => item.EstadoVerificacion == "Pendiente"),
                DocumentosRechazados = documentos.Count(item => item.EstadoVerificacion == "Rechazado"),
                DocumentosVencidos = documentos.Count(item => item.FechaVencimiento != null &&
                    item.FechaVencimiento.Value.Date < hoy &&
                    item.EstadoVerificacion != "Rechazado"),
                PasajerosActivos = pasajeros.Count(item => item.Estado),
                PromedioCalificacion = calificaciones.Count == 0
                    ? 0
                    : Math.Round((decimal)calificaciones.Average(item => item.Puntaje), 2),
                ViajesPendientesLiquidacion = viajesPendientesLiquidacion.Count,
                MontoPendienteLiquidacion = viajesPendientesLiquidacion.Sum(item => item.TarifaFinal),
                LiquidacionesPendientesPago = liquidaciones.Count(item => item.Estado == "Cerrada"),
                NetoPendientePago = liquidaciones
                    .Where(item => item.Estado == "Cerrada")
                    .Sum(item => item.TotalNetoConductor)
            };

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
