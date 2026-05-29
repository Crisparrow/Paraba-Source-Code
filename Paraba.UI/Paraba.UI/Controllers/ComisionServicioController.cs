using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finanzas")]
    public class ComisionServicioController : Controller
    {
        private readonly ComisionServicioService comisionServicioService = new ComisionServicioService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();

        public IActionResult Index()
        {
            var tiposServicio = tipoServicioService.ListarTiposServicio();

            var comisiones = comisionServicioService.ListarComisiones()
                .Select(item => new ComisionServicioViewModel
                {
                    IdComisionServicio = item.IdComisionServicio,
                    TipoServicio = tiposServicio.FirstOrDefault(tipo => tipo.IdTipoServicio == item.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                    PorcentajeComision = item.PorcentajeComision,
                    FechaInicioVigencia = item.FechaInicioVigencia,
                    FechaFinVigencia = item.FechaFinVigencia,
                    Estado = item.Estado
                })
                .ToList();

            return View(comisiones);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(int idComisionServicio, decimal porcentajeComision)
        {
            try
            {
                comisionServicioService.ActualizarPorcentaje(idComisionServicio, porcentajeComision);
            }
            catch (ArgumentException ex)
            {
                TempData["MensajeComision"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
