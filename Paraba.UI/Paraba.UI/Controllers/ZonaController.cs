using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class ZonaController : Controller
    {
        private readonly ZonaService zonaService = new ZonaService();

        public IActionResult Index()
        {
            var zonas = zonaService.ListarZonas();
            var zonasViewModel = zonas.Select(zona => new ZonaViewModel
            {
                IdZona = zona.IdZona,
                Ciudad = ObtenerCiudad(zona.IdCiudad),
                Nombre = zona.Nombre,
                Descripcion = zona.Descripcion,
                Estado = zona.Estado,
                FechaRegistro = zona.FechaRegistro
            }).ToList();

            return View(zonasViewModel);
        }

        private static string ObtenerCiudad(int idCiudad)
        {
            return idCiudad == 1 ? "Santa Cruz de la Sierra" : "Ciudad no identificada";
        }
    }
}
