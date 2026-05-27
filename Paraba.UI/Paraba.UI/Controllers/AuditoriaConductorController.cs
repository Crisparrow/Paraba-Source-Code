using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
    public class AuditoriaConductorController : Controller
    {
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();
        private readonly ConductorService conductorService = new ConductorService();

        public IActionResult Index()
        {
            var conductores = conductorService.ListarConductores();

            var auditorias = auditoriaConductorService.ListarAuditoriaConductores()
                .Select(item => new AuditoriaConductorViewModel
                {
                    IdAuditoriaConductor = item.IdAuditoriaConductor,
                    IdConductor = item.IdConductor,
                    Conductor = conductores.FirstOrDefault(conductor => conductor.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                    Accion = item.Accion,
                    EstadoAnterior = item.EstadoAnterior,
                    EstadoNuevo = item.EstadoNuevo,
                    UsuarioSistema = item.UsuarioSistema,
                    Observacion = item.Observacion,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            return View(auditorias);
        }
    }
}
