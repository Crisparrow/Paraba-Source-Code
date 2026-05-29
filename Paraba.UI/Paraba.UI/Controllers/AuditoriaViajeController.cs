using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
    public class AuditoriaViajeController : Controller
    {
        private readonly AuditoriaViajeService auditoriaViajeService = new AuditoriaViajeService();

        public IActionResult Index(AuditoriaViajeFiltroViewModel filtros)
        {
            var auditorias = auditoriaViajeService.ListarAuditoriaViajes().AsEnumerable();

            if (filtros.FechaDesde != null)
            {
                auditorias = auditorias.Where(item => item.FechaRegistro.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                auditorias = auditorias.Where(item => item.FechaRegistro.Date <= filtros.FechaHasta.Value.Date);
            }

            if (filtros.IdViaje != null)
            {
                auditorias = auditorias.Where(item => item.IdViaje == filtros.IdViaje.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Accion))
            {
                auditorias = auditorias.Where(item => item.Accion.Contains(filtros.Accion.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            filtros.Auditorias = auditorias
                .Select(item => new AuditoriaViajeViewModel
                {
                    IdAuditoriaViaje = item.IdAuditoriaViaje,
                    IdViaje = item.IdViaje,
                    Accion = item.Accion,
                    EstadoAnterior = item.EstadoAnterior,
                    EstadoNuevo = item.EstadoNuevo,
                    TarifaAnterior = item.TarifaAnterior,
                    TarifaNueva = item.TarifaNueva,
                    UsuarioSistema = item.UsuarioSistema,
                    Observacion = item.Observacion,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            return View(filtros);
        }
    }
}
