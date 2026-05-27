using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class TipoViaController : Controller
    {
        private readonly TipoViaService tipoViaService = new TipoViaService();

        public IActionResult Index()
        {
            var tiposVia = tipoViaService.ListarTiposVia();

            return View(tiposVia);
        }
    }
}
