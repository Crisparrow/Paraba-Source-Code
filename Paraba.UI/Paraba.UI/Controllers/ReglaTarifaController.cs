using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class ReglaTarifaController : Controller
    {
        private readonly ReglaTarifaService reglaTarifaService = new ReglaTarifaService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly ZonaService zonaService = new ZonaService();

        public IActionResult Index()
        {
            var reglas = reglaTarifaService.ListarReglasTarifa();
            var tiposServicio = tipoServicioService.ListarTiposServicio();
            var zonas = zonaService.ListarZonas();
            var reglasViewModel = reglas.Select(regla => new ReglaTarifaViewModel
            {
                IdReglaTarifa = regla.IdReglaTarifa,
                Nombre = regla.Nombre,
                TipoRegla = regla.TipoRegla,
                TipoServicio = regla.IdTipoServicio == null
                    ? "Todos"
                    : tiposServicio.FirstOrDefault(item => item.IdTipoServicio == regla.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                Zona = regla.IdZona == null
                    ? "Todas"
                    : zonas.FirstOrDefault(item => item.IdZona == regla.IdZona)?.Nombre ?? "Zona no identificada",
                PorcentajeIncremento = regla.PorcentajeIncremento,
                MontoIncremento = regla.MontoIncremento,
                Horario = ObtenerHorario(regla.HoraInicio, regla.HoraFin),
                Prioridad = regla.Prioridad,
                Estado = regla.Estado
            }).ToList();

            return View(reglasViewModel);
        }

        private static string ObtenerHorario(TimeSpan? horaInicio, TimeSpan? horaFin)
        {
            if (horaInicio == null || horaFin == null)
            {
                return "Todo el dia";
            }

            return $"{horaInicio:hh\\:mm} - {horaFin:hh\\:mm}";
        }
    }
}
