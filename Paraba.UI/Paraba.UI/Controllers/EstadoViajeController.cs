using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class EstadoViajeController : Controller
    {
        private readonly EstadoViajeService estadoViajeService = new EstadoViajeService();

        public IActionResult Index()
        {
            var estadosViaje = estadoViajeService.ListarEstadosViaje();

            return View(estadosViaje);
        }
    }
}
