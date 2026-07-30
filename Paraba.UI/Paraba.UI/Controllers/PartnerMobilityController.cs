using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers;

[Authorize(Roles = "SuperAdmin,Operaciones,Finanzas")]
public sealed class PartnerMobilityController : Controller
{
    private readonly PartnerMobilityService service = new();

    public IActionResult Index()
    {
        return View(new PartnerMobilityViewModel
        {
            RutasMicrobus = service.ListRoutes(),
            AsociacionesMototaxi = service.ListAssociations()
        });
    }
}
