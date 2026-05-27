using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class DepartamentoController : Controller
    {
        private readonly DepartamentoService departamentoService = new DepartamentoService();

        public IActionResult Index()
        {
            var departamentos = departamentoService.ListarDepartamentos();
            var departamentosViewModel = departamentos.Select(departamento => new DepartamentoViewModel
            {
                IdDepartamento = departamento.IdDepartamento,
                Pais = ObtenerPais(departamento.IdPais),
                Nombre = departamento.Nombre,
                Estado = departamento.Estado
            }).ToList();

            return View(departamentosViewModel);
        }

        private static string ObtenerPais(int idPais)
        {
            return idPais == 1 ? "Bolivia" : "Pais no identificado";
        }
    }
}
