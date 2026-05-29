using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;
using System.Text;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Operaciones,Soporte,Verificador")]
    public class ReporteConductoresController : Controller
    {
        private readonly ConductorService conductorService = new ConductorService();
        private readonly ViajeService viajeService = new ViajeService();
        private readonly CalificacionService calificacionService = new CalificacionService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index(ReporteConductoresViewModel filtros)
        {
            PrepararReporte(filtros);

            return View(filtros);
        }

        public IActionResult ExportarCsv(ReporteConductoresViewModel filtros)
        {
            PrepararReporte(filtros);
            auditoriaAdministrativaService.Registrar("Reportes", "Exportacion CSV", "ReporteConductores", null, User.Identity?.Name ?? "Admin PARABA", "Exportacion de reporte de conductores.");

            var csv = new StringBuilder();
            csv.AppendLine("IdConductor,Conductor,Telefono,Correo,Verificado,Disponible,Estado,Viajes,Finalizados,Cancelados,IngresosFinalizados,Calificacion");

            foreach (var conductor in filtros.Conductores)
            {
                csv.AppendLine(string.Join(",",
                    conductor.IdConductor,
                    Csv(conductor.NombreCompleto),
                    Csv(conductor.Telefono),
                    Csv(conductor.Correo),
                    conductor.Verificado ? "Si" : "No",
                    conductor.Disponible ? "Si" : "No",
                    conductor.Estado ? "Activo" : "Suspendido",
                    conductor.TotalViajes,
                    conductor.ViajesFinalizados,
                    conductor.ViajesCancelados,
                    conductor.IngresosFinalizados.ToString("0.00"),
                    conductor.PromedioCalificacion.ToString("0.00")));
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"reporte-conductores-{DateTime.Now:yyyyMMddHHmm}.csv");
        }

        private void PrepararReporte(ReporteConductoresViewModel filtros)
        {
            var viajes = viajeService.ListarViajes();
            var calificaciones = calificacionService.ListarCalificaciones()
                .Where(item => item.Estado)
                .ToList();

            var conductores = conductorService.ListarConductores().AsEnumerable();

            if (filtros.Verificado != null)
            {
                conductores = conductores.Where(item => item.Verificado == filtros.Verificado.Value);
            }

            if (filtros.Disponible != null)
            {
                conductores = conductores.Where(item => item.Disponible == filtros.Disponible.Value);
            }

            if (filtros.Estado != null)
            {
                conductores = conductores.Where(item => item.Estado == filtros.Estado.Value);
            }

            filtros.Conductores = conductores
                .OrderBy(item => item.NombreCompleto)
                .Select(conductor =>
                {
                    var viajesConductor = viajes
                        .Where(viaje => viaje.IdConductor == conductor.IdConductor)
                        .ToList();

                    var calificacionesConductor = calificaciones
                        .Where(calificacion => calificacion.IdConductor == conductor.IdConductor)
                        .ToList();

                    return new ReporteConductorItemViewModel
                    {
                        IdConductor = conductor.IdConductor,
                        NombreCompleto = conductor.NombreCompleto,
                        Telefono = conductor.Telefono,
                        Correo = conductor.Correo,
                        Verificado = conductor.Verificado,
                        Disponible = conductor.Disponible,
                        Estado = conductor.Estado,
                        TotalViajes = viajesConductor.Count,
                        ViajesFinalizados = viajesConductor.Count(viaje => viaje.IdEstadoViaje == 4),
                        ViajesCancelados = viajesConductor.Count(viaje => viaje.IdEstadoViaje == 5),
                        IngresosFinalizados = viajesConductor
                            .Where(viaje => viaje.IdEstadoViaje == 4)
                            .Sum(viaje => viaje.TarifaFinal),
                        PromedioCalificacion = calificacionesConductor.Count == 0
                            ? 0
                            : Math.Round((decimal)calificacionesConductor.Average(calificacion => calificacion.Puntaje), 2)
                    };
                })
                .ToList();

            filtros.TotalConductores = filtros.Conductores.Count;
            filtros.TotalVerificados = filtros.Conductores.Count(item => item.Verificado);
            filtros.TotalDisponibles = filtros.Conductores.Count(item => item.Disponible);
            filtros.TotalSuspendidos = filtros.Conductores.Count(item => !item.Estado);
            filtros.IngresosFinalizados = filtros.Conductores.Sum(item => item.IngresosFinalizados);
            filtros.PromedioGeneralCalificacion = filtros.Conductores.Any(item => item.PromedioCalificacion > 0)
                ? Math.Round(filtros.Conductores.Where(item => item.PromedioCalificacion > 0).Average(item => item.PromedioCalificacion), 2)
                : 0;
        }

        private static string Csv(string valor)
        {
            return $"\"{valor.Replace("\"", "\"\"")}\"";
        }
    }
}
