using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class CiudadController : Controller
    {
        private readonly CiudadService ciudadService = new CiudadService();
        private readonly DepartamentoService departamentoService = new DepartamentoService();

        public IActionResult Index()
        {
            var ciudades = ciudadService.ListarCiudades();
            var departamentos = departamentoService.ListarDepartamentos();
            var ciudadesViewModel = ciudades.Select(ciudad => new CiudadViewModel
            {
                IdCiudad = ciudad.IdCiudad,
                Departamento = departamentos.FirstOrDefault(item => item.IdDepartamento == ciudad.IdDepartamento)?.Nombre ?? "Departamento no identificado",
                Nombre = ciudad.Nombre,
                Estado = ciudad.Estado
            }).ToList();

            return View(ciudadesViewModel);
        }
    }
}
