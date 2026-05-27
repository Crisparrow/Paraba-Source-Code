using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Suspender(int id)
        {
            usuarioAdminService.SuspenderUsuario(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            usuarioAdminService.ReactivarUsuario(id);

            return RedirectToAction(nameof(Index));
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
    }
}
