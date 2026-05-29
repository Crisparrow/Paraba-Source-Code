using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
    public class AuditoriaConductorController : Controller
    {
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();
        private readonly ConductorService conductorService = new ConductorService();

        public IActionResult Index(AuditoriaConductorFiltroViewModel filtros)
        {
            var conductores = conductorService.ListarConductores();
            var auditorias = auditoriaConductorService.ListarAuditoriaConductores().AsEnumerable();

            if (filtros.FechaDesde != null)
            {
                auditorias = auditorias.Where(item => item.FechaRegistro.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                auditorias = auditorias.Where(item => item.FechaRegistro.Date <= filtros.FechaHasta.Value.Date);
            }

            if (filtros.IdConductor != null)
            {
                auditorias = auditorias.Where(item => item.IdConductor == filtros.IdConductor.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Accion))
            {
                auditorias = auditorias.Where(item => item.Accion.Contains(filtros.Accion.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            filtros.Conductores = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty)
            };
            filtros.Conductores.AddRange(conductores.Select(item => new SelectListItem(item.NombreCompleto, item.IdConductor.ToString())));

            filtros.Auditorias = auditorias
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

            return View(filtros);
        }
    }
}
