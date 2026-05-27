using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class PaisController : Controller
    {
        private readonly PaisService paisService = new PaisService();

        public IActionResult Index()
        {
            var paises = paisService.ListarPaises();

            return View(paises);
        }
    }
}
