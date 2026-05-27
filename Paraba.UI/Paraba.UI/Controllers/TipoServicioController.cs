using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class TipoServicioController : Controller
    {
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();

        public IActionResult Index()
        {
            var tiposServicio = tipoServicioService.ListarTiposServicio();

            return View(tiposServicio);
        }
    }
}
