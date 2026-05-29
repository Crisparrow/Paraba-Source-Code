using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones,Finanzas")]
    public class TarifaController : Controller
    {
        private readonly TarifaService tarifaService = new TarifaService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();

        public IActionResult Index()
        {
            var tarifas = tarifaService.ListarTarifas();
            var tiposServicio = tipoServicioService.ListarTiposServicio();
            var tarifasViewModel = tarifas.Select(tarifa => new TarifaViewModel
            {
                IdTarifa = tarifa.IdTarifa,
                TipoServicio = tiposServicio.FirstOrDefault(item => item.IdTipoServicio == tarifa.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                TarifaBase = tarifa.TarifaBase,
                CostoPorKilometro = tarifa.CostoPorKilometro,
                CostoPorMinuto = tarifa.CostoPorMinuto,
                TarifaMinima = tarifa.TarifaMinima,
                Estado = tarifa.Estado
            }).ToList();

            return View(tarifasViewModel);
        }
    }
}
