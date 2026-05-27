using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UsuarioAdminService usuarioAdminService = new UsuarioAdminService();

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View(new LoginAdminViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAdminViewModel viewModel, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(viewModel.Correo) || string.IsNullOrWhiteSpace(viewModel.Password))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar correo y password.");
                return View(viewModel);
            }

            string ipOrigen = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "No identificada";
            var usuario = usuarioAdminService.ValidarLogin(viewModel.Correo, viewModel.Password, ipOrigen);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales invalidas o usuario inactivo.");
                return View(viewModel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuarioAdmin.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Correo)
            };

            foreach (string rol in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = viewModel.Recordarme,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            string correo = User.FindFirstValue(ClaimTypes.Email) ?? "No identificado";
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int idUsuarioAdmin);
            string ipOrigen = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "No identificada";

            usuarioAdminService.RegistrarLogout(idUsuarioAdmin, correo, ipOrigen);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
