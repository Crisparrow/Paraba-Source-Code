using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
    public class DepartamentoController : Controller
    {
        private readonly DepartamentoService departamentoService = new DepartamentoService();
        private readonly PaisService paisService = new PaisService();

        public IActionResult Index()
        {
            var departamentos = departamentoService.ListarDepartamentos();
            var paises = paisService.ListarPaises();
            var departamentosViewModel = departamentos.Select(departamento => new DepartamentoViewModel
            {
                IdDepartamento = departamento.IdDepartamento,
                Pais = paises.FirstOrDefault(item => item.IdPais == departamento.IdPais)?.Nombre ?? "Pais no identificado",
                Nombre = departamento.Nombre,
                Estado = departamento.Estado
            }).ToList();

            return View(departamentosViewModel);
        }
    }
}
