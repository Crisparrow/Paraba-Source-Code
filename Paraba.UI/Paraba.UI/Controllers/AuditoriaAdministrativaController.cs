using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AuditoriaAdministrativaController : Controller
    {
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index()
        {
            return View(auditoriaAdministrativaService.ListarAuditorias());
        }
    }
}
