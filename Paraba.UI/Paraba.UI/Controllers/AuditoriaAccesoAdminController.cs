using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AuditoriaAccesoAdminController : Controller
    {
        private readonly UsuarioAdminService usuarioAdminService = new UsuarioAdminService();

        public IActionResult Index(AuditoriaAccesoAdminFiltroViewModel filtros)
        {
            var auditorias = usuarioAdminService.ListarAuditoriaAccesos().AsEnumerable();

            if (filtros.FechaDesde != null)
            {
                auditorias = auditorias.Where(item => item.FechaRegistro.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                auditorias = auditorias.Where(item => item.FechaRegistro.Date <= filtros.FechaHasta.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(filtros.Correo))
            {
                auditorias = auditorias.Where(item => item.Correo.Contains(filtros.Correo.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (filtros.Exitoso != null)
            {
                auditorias = auditorias.Where(item => item.Exitoso == filtros.Exitoso.Value);
            }

            filtros.Auditorias = auditorias
                .Select(item => new AuditoriaAccesoAdminViewModel
                {
                    IdAuditoriaAccesoAdmin = item.IdAuditoriaAccesoAdmin,
                    IdUsuarioAdmin = item.IdUsuarioAdmin,
                    Correo = item.Correo,
                    Accion = item.Accion,
                    Exitoso = item.Exitoso,
                    IpOrigen = item.IpOrigen,
                    Observacion = item.Observacion,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            return View(filtros);
        }
    }
}
