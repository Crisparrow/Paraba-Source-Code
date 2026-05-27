using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class CiudadController : Controller
    {
        private readonly CiudadService ciudadService = new CiudadService();

        public IActionResult Index()
        {
            var ciudades = ciudadService.ListarCiudades();
            var ciudadesViewModel = ciudades.Select(ciudad => new CiudadViewModel
            {
                IdCiudad = ciudad.IdCiudad,
                Departamento = ObtenerDepartamento(ciudad.IdDepartamento),
                Nombre = ciudad.Nombre,
                Estado = ciudad.Estado
            }).ToList();

            return View(ciudadesViewModel);
        }

        private static string ObtenerDepartamento(int idDepartamento)
        {
            return idDepartamento == 1 ? "Santa Cruz" : "Departamento no identificado";
        }
    }
}
