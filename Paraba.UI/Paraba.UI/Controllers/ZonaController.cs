using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class ZonaController : Controller
    {
        private readonly ZonaService zonaService = new ZonaService();
        private readonly CiudadService ciudadService = new CiudadService();

        public IActionResult Index()
        {
            var zonas = zonaService.ListarZonas();
            var ciudades = ciudadService.ListarCiudades();
            var zonasViewModel = zonas.Select(zona => new ZonaViewModel
            {
                IdZona = zona.IdZona,
                Ciudad = ciudades.FirstOrDefault(item => item.IdCiudad == zona.IdCiudad)?.Nombre ?? "Ciudad no identificada",
                Nombre = zona.Nombre,
                Descripcion = zona.Descripcion,
                Estado = zona.Estado,
                CoberturaActiva = zona.CoberturaActiva,
                EsZonaRiesgo = zona.EsZonaRiesgo,
                AltaDemanda = zona.AltaDemanda,
                ObservacionOperativa = zona.ObservacionOperativa,
                FechaRegistro = zona.FechaRegistro
            }).ToList();

            return View(zonasViewModel);
        }

        public IActionResult Operacion(int id)
        {
            var zona = zonaService.ListarZonas().FirstOrDefault(item => item.IdZona == id);
            var ciudades = ciudadService.ListarCiudades();

            if (zona == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new ZonaViewModel
            {
                IdZona = zona.IdZona,
                Ciudad = ciudades.FirstOrDefault(item => item.IdCiudad == zona.IdCiudad)?.Nombre ?? "Ciudad no identificada",
                Nombre = zona.Nombre,
                Descripcion = zona.Descripcion,
                Estado = zona.Estado,
                CoberturaActiva = zona.CoberturaActiva,
                EsZonaRiesgo = zona.EsZonaRiesgo,
                AltaDemanda = zona.AltaDemanda,
                ObservacionOperativa = zona.ObservacionOperativa,
                FechaRegistro = zona.FechaRegistro
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Operacion(ZonaViewModel viewModel)
        {
            zonaService.ActualizarOperacion(new Zona
            {
                IdZona = viewModel.IdZona,
                CoberturaActiva = viewModel.CoberturaActiva,
                EsZonaRiesgo = viewModel.EsZonaRiesgo,
                AltaDemanda = viewModel.AltaDemanda,
                ObservacionOperativa = viewModel.ObservacionOperativa
            });

            return RedirectToAction(nameof(Index));
        }
    }
}
