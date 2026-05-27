using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class ReglaTarifaController : Controller
    {
        private readonly ReglaTarifaService reglaTarifaService = new ReglaTarifaService();

        public IActionResult Index()
        {
            var reglas = reglaTarifaService.ListarReglasTarifa();
            var reglasViewModel = reglas.Select(regla => new ReglaTarifaViewModel
            {
                IdReglaTarifa = regla.IdReglaTarifa,
                Nombre = regla.Nombre,
                TipoRegla = regla.TipoRegla,
                TipoServicio = ObtenerTipoServicio(regla.IdTipoServicio),
                Zona = ObtenerZona(regla.IdZona),
                PorcentajeIncremento = regla.PorcentajeIncremento,
                MontoIncremento = regla.MontoIncremento,
                Horario = ObtenerHorario(regla.HoraInicio, regla.HoraFin),
                Prioridad = regla.Prioridad,
                Estado = regla.Estado
            }).ToList();

            return View(reglasViewModel);
        }

        private static string ObtenerTipoServicio(int? idTipoServicio)
        {
            return idTipoServicio switch
            {
                1 => "Taxi",
                2 => "Moto taxi",
                _ => "Todos"
            };
        }

        private static string ObtenerZona(int? idZona)
        {
            return idZona switch
            {
                1 => "Centro",
                2 => "Norte",
                3 => "Sur",
                4 => "Terminal",
                5 => "Universidad",
                _ => "Todas"
            };
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
