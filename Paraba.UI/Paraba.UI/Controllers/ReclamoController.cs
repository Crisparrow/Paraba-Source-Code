using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;
using Paraba.UI.ViewModels;
using System.Security.Claims;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Soporte,Operaciones")]
    public class ReclamoController : Controller
    {
        private readonly ReclamoService reclamoService = new ReclamoService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index()
        {
            var pasajeros = pasajeroService.ListarPasajeros();
            var conductores = conductorService.ListarConductores();
            var reclamos = reclamoService.ListarReclamos()
                .Select(item => new ReclamoViewModel
                {
                    IdReclamo = item.IdReclamo,
                    IdViaje = item.IdViaje,
                    IdPasajero = item.IdPasajero,
                    IdConductor = item.IdConductor,
                    TipoReclamo = item.TipoReclamo,
                    Descripcion = item.Descripcion,
                    Estado = item.Estado,
                    Prioridad = item.Prioridad,
                    Pasajero = item.IdPasajero == null ? "-" : pasajeros.FirstOrDefault(p => p.IdPasajero == item.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                    Conductor = item.IdConductor == null ? "-" : conductores.FirstOrDefault(c => c.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                    UsuarioRegistro = item.UsuarioRegistro,
                    FechaRegistro = item.FechaRegistro,
                    FechaCierre = item.FechaCierre,
                    ObservacionCierre = item.ObservacionCierre
                })
                .ToList();

            return View(reclamos);
        }

        public IActionResult Crear()
        {
            return View(new ReclamoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ReclamoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string usuario = ObtenerUsuario();

            try
            {
                reclamoService.Registrar(new Reclamo
                {
                    IdViaje = viewModel.IdViaje,
                    IdPasajero = viewModel.IdPasajero,
                    IdConductor = viewModel.IdConductor,
                    TipoReclamo = viewModel.TipoReclamo,
                    Descripcion = viewModel.Descripcion,
                    Prioridad = viewModel.Prioridad,
                    UsuarioRegistro = usuario
                });

                auditoriaAdministrativaService.Registrar("Soporte", "Reclamo creado", "Reclamo", null, usuario, viewModel.Descripcion);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cerrar(int id, string observacionCierre)
        {
            string usuario = ObtenerUsuario();
            reclamoService.Cerrar(id, "Cerrado", usuario, observacionCierre);
            auditoriaAdministrativaService.Registrar("Soporte", "Reclamo cerrado", "Reclamo", id, usuario, observacionCierre);

            return RedirectToAction(nameof(Index));
        }

        private string ObtenerUsuario()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin PARABA";
        }
    }
}
