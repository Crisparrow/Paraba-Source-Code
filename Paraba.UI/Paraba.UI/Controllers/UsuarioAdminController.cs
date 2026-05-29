using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UsuarioAdminController : Controller
    {
        private readonly UsuarioAdminService usuarioAdminService = new UsuarioAdminService();

        public IActionResult Index()
        {
            var usuarios = usuarioAdminService.ListarUsuarios()
                .Select(usuario => new UsuarioAdminViewModel
                {
                    IdUsuarioAdmin = usuario.IdUsuarioAdmin,
                    NombreCompleto = usuario.NombreCompleto,
                    Correo = usuario.Correo,
                    Roles = string.Join(", ", usuario.Roles),
                    Estado = usuario.Estado,
                    IntentosFallidos = usuario.IntentosFallidos,
                    UltimoAcceso = usuario.UltimoAcceso,
                    FechaRegistro = usuario.FechaRegistro
                })
                .ToList();

            return View(usuarios);
        }

        public IActionResult Detalle(int id)
        {
            var usuario = usuarioAdminService.ListarUsuarios()
                .FirstOrDefault(item => item.IdUsuarioAdmin == id);

            if (usuario == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var auditorias = usuarioAdminService.ListarAuditoriaAccesos()
                .Where(item => item.IdUsuarioAdmin == usuario.IdUsuarioAdmin || item.Correo == usuario.Correo)
                .OrderByDescending(item => item.FechaRegistro)
                .Take(50)
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

            var viewModel = new DetalleUsuarioAdminViewModel
            {
                IdUsuarioAdmin = usuario.IdUsuarioAdmin,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                Roles = string.Join(", ", usuario.Roles),
                Estado = usuario.Estado,
                IntentosFallidos = usuario.IntentosFallidos,
                UltimoAcceso = usuario.UltimoAcceso,
                FechaRegistro = usuario.FechaRegistro,
                EsUsuarioActual = usuario.IdUsuarioAdmin == ObtenerIdUsuarioActual(),
                Auditorias = auditorias
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(CrearViewModelBase());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(CrearUsuarioAdminViewModel viewModel)
        {
            try
            {
                usuarioAdminService.CrearUsuario(
                    viewModel.NombreCompleto,
                    viewModel.Correo,
                    viewModel.Password,
                    viewModel.IdRolAdmin);
            }
            catch (ArgumentException ex)
            {
                var resultado = CrearViewModelBase();
                resultado.NombreCompleto = viewModel.NombreCompleto;
                resultado.Correo = viewModel.Correo;
                resultado.IdRolAdmin = viewModel.IdRolAdmin;

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(resultado);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Suspender(int id)
        {
            var viewModel = CrearIntervencionUsuarioViewModel(id, "Suspender");

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Intervencion", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarSuspension(IntervencionUsuarioAdminViewModel viewModel)
        {
            return ConfirmarIntervencion(viewModel, "Suspender");
        }

        public IActionResult Reactivar(int id)
        {
            var viewModel = CrearIntervencionUsuarioViewModel(id, "Reactivar");

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Intervencion", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarReactivacion(IntervencionUsuarioAdminViewModel viewModel)
        {
            return ConfirmarIntervencion(viewModel, "Reactivar");
        }

        private IActionResult ConfirmarIntervencion(IntervencionUsuarioAdminViewModel viewModel, string accion)
        {
            var datos = CrearIntervencionUsuarioViewModel(viewModel.IdUsuarioAdmin, accion);

            if (datos == null)
            {
                return RedirectToAction(nameof(Index));
            }

            viewModel.NombreCompleto = datos.NombreCompleto;
            viewModel.Correo = datos.Correo;
            viewModel.Roles = datos.Roles;
            viewModel.EstadoActual = datos.EstadoActual;
            viewModel.Accion = datos.Accion;

            if (!ModelState.IsValid)
            {
                return View("Intervencion", viewModel);
            }

            try
            {
                if (accion == "Suspender")
                {
                    usuarioAdminService.SuspenderUsuario(viewModel.IdUsuarioAdmin, ObtenerIdUsuarioActual(), viewModel.Motivo);
                }
                else
                {
                    usuarioAdminService.ReactivarUsuario(viewModel.IdUsuarioAdmin, viewModel.Motivo);
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Intervencion", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        private IntervencionUsuarioAdminViewModel? CrearIntervencionUsuarioViewModel(int idUsuarioAdmin, string accion)
        {
            var usuario = usuarioAdminService.ListarUsuarios()
                .FirstOrDefault(item => item.IdUsuarioAdmin == idUsuarioAdmin);

            if (usuario == null)
            {
                return null;
            }

            if (accion == "Suspender" && !usuario.Estado)
            {
                return null;
            }

            if (accion == "Reactivar" && usuario.Estado)
            {
                return null;
            }

            return new IntervencionUsuarioAdminViewModel
            {
                IdUsuarioAdmin = usuario.IdUsuarioAdmin,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                Roles = string.Join(", ", usuario.Roles),
                EstadoActual = usuario.Estado ? "Activo" : "Suspendido",
                Accion = accion
            };
        }

        private CrearUsuarioAdminViewModel CrearViewModelBase()
        {
            return new CrearUsuarioAdminViewModel
            {
                Roles = usuarioAdminService.ListarRoles()
                    .Select(rol => new SelectListItem(rol.Nombre, rol.IdRolAdmin.ToString()))
                    .ToList()
            };
        }

        private int ObtenerIdUsuarioActual()
        {
            string? valor = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(valor, out int idUsuarioAdmin) ? idUsuarioAdmin : 0;
        }
    }
}
