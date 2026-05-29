using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;
using Paraba.UI.ViewModels;
using System.Text;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Operaciones,Soporte,Finanzas")]
    public class ReporteViajesController : Controller
    {
        private readonly ViajeService viajeService = new ViajeService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly EstadoViajeService estadoViajeService = new EstadoViajeService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index(ReporteViajesViewModel filtros)
        {
            PrepararReporte(filtros);

            return View(filtros);
        }

        public IActionResult ExportarCsv(ReporteViajesViewModel filtros)
        {
            PrepararReporte(filtros);
            auditoriaAdministrativaService.Registrar("Reportes", "Exportacion CSV", "ReporteViajes", null, User.Identity?.Name ?? "Admin PARABA", "Exportacion de reporte de viajes.");

            var csv = new StringBuilder();
            csv.AppendLine("IdViaje,Pasajero,Conductor,Servicio,Ruta,Estado,TarifaSugerida,TarifaOfertada,TarifaAceptada,TarifaFinal,FechaSolicitud");

            foreach (var viaje in filtros.Viajes)
            {
                csv.AppendLine(string.Join(",",
                    viaje.IdViaje,
                    Csv(viaje.Pasajero),
                    Csv(viaje.Conductor),
                    Csv(viaje.TipoServicio),
                    Csv(viaje.Ruta),
                    Csv(viaje.Estado),
                    viaje.TarifaSugerida.ToString("0.00"),
                    viaje.TarifaOfertada.ToString("0.00"),
                    viaje.TarifaAceptada?.ToString("0.00") ?? string.Empty,
                    viaje.TarifaFinal.ToString("0.00"),
                    Csv(viaje.FechaSolicitud.ToString("dd/MM/yyyy HH:mm"))));
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"reporte-viajes-{DateTime.Now:yyyyMMddHHmm}.csv");
        }

        private void PrepararReporte(ReporteViajesViewModel filtros)
        {
            var pasajeros = pasajeroService.ListarPasajeros();
            var conductores = conductorService.ListarConductores();
            var tiposServicio = tipoServicioService.ListarTiposServicio();
            var estadosViaje = estadoViajeService.ListarEstadosViaje();

            var viajes = viajeService.ListarViajes().AsEnumerable();

            if (filtros.FechaDesde != null)
            {
                viajes = viajes.Where(item => item.FechaSolicitud.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                viajes = viajes.Where(item => item.FechaSolicitud.Date <= filtros.FechaHasta.Value.Date);
            }

            if (filtros.IdEstadoViaje != null)
            {
                viajes = viajes.Where(item => item.IdEstadoViaje == filtros.IdEstadoViaje.Value);
            }

            if (filtros.IdTipoServicio != null)
            {
                viajes = viajes.Where(item => item.IdTipoServicio == filtros.IdTipoServicio.Value);
            }

            if (filtros.IdConductor != null)
            {
                viajes = viajes.Where(item => item.IdConductor == filtros.IdConductor.Value);
            }

            var viajesFiltrados = viajes.OrderByDescending(item => item.FechaSolicitud).ToList();

            filtros.EstadosViaje = CrearOpcionesEstados(estadosViaje);
            filtros.TiposServicio = CrearOpcionesTiposServicio(tiposServicio);
            filtros.Conductores = CrearOpcionesConductores(conductores);
            filtros.TotalViajes = viajesFiltrados.Count;
            filtros.TotalFinalizados = viajesFiltrados.Count(item => item.IdEstadoViaje == 4);
            filtros.TotalCancelados = viajesFiltrados.Count(item => item.IdEstadoViaje == 5);
            filtros.IngresosFinalizados = viajesFiltrados
                .Where(item => item.IdEstadoViaje == 4)
                .Sum(item => item.TarifaFinal);
            filtros.PromedioTarifaAceptada = viajesFiltrados.Any(item => item.TarifaAceptada != null)
                ? Math.Round(viajesFiltrados.Where(item => item.TarifaAceptada != null).Average(item => item.TarifaAceptada!.Value), 2)
                : 0;

            filtros.Viajes = viajesFiltrados.Select(viaje => new ReporteViajeItemViewModel
            {
                IdViaje = viaje.IdViaje,
                Pasajero = pasajeros.FirstOrDefault(item => item.IdPasajero == viaje.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                Conductor = conductores.FirstOrDefault(item => item.IdConductor == viaje.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                TipoServicio = tiposServicio.FirstOrDefault(item => item.IdTipoServicio == viaje.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                Ruta = $"{viaje.Origen} -> {viaje.Destino}",
                Estado = estadosViaje.FirstOrDefault(item => item.IdEstadoViaje == viaje.IdEstadoViaje)?.Nombre ?? "Estado no identificado",
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaAceptada = viaje.TarifaAceptada,
                TarifaFinal = viaje.TarifaFinal,
                FechaSolicitud = viaje.FechaSolicitud
            }).ToList();
        }

        private static List<SelectListItem> CrearOpcionesEstados(List<EstadoViaje> estados)
        {
            var opciones = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty)
            };

            opciones.AddRange(estados.Select(item => new SelectListItem(item.Nombre, item.IdEstadoViaje.ToString())));

            return opciones;
        }

        private static List<SelectListItem> CrearOpcionesTiposServicio(List<TipoServicio> tiposServicio)
        {
            var opciones = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty)
            };

            opciones.AddRange(tiposServicio.Select(item => new SelectListItem(item.Nombre, item.IdTipoServicio.ToString())));

            return opciones;
        }

        private static List<SelectListItem> CrearOpcionesConductores(List<Conductor> conductores)
        {
            var opciones = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty)
            };

            opciones.AddRange(conductores.Select(item => new SelectListItem(item.NombreCompleto, item.IdConductor.ToString())));

            return opciones;
        }

        private static string Csv(string valor)
        {
            return $"\"{valor.Replace("\"", "\"\"")}\"";
        }
    }
}
