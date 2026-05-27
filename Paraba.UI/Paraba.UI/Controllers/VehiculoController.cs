using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador,Soporte")]
    public class VehiculoController : Controller
    {
        private readonly VehiculoService vehiculoService = new VehiculoService();

        public IActionResult Index()
        {
            var vehiculos = vehiculoService.ListarVehiculos();

            return View(vehiculos);
        }
    }
}
