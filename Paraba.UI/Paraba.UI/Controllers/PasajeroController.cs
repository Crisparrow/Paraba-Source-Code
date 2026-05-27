using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Soporte")]
    public class PasajeroController : Controller
    {
        private readonly PasajeroService pasajeroService = new PasajeroService();

        public IActionResult Index()
        {
            var pasajeros = pasajeroService.ListarPasajeros();

            return View(pasajeros);
        }
    }
}
