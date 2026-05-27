using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
    public class AuditoriaViajeController : Controller
    {
        private readonly AuditoriaViajeService auditoriaViajeService = new AuditoriaViajeService();

        public IActionResult Index()
        {
            var auditorias = auditoriaViajeService.ListarAuditoriaViajes()
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

            return View(auditorias);
        }
    }
}
