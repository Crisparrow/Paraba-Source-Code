using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones,Soporte")]
    public class SimuladorTarifaController : Controller
    {
        private readonly TarifaService tarifaService = new TarifaService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly ZonaService zonaService = new ZonaService();
        private readonly TipoViaService tipoViaService = new TipoViaService();
        private readonly ReglaTarifaService reglaTarifaService = new ReglaTarifaService();
        private readonly CalculadoraTarifaService calculadoraTarifaService = new CalculadoraTarifaService();

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = CrearViewModelBase();
            viewModel.DistanciaKilometros = 5;
            viewModel.TiempoMinutos = 12;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SimuladorTarifaViewModel viewModel)
        {
            var tarifas = tarifaService.ListarTarifas();
            var reglas = reglaTarifaService.ListarReglasTarifa();
            var tiposVia = tipoViaService.ListarTiposVia();

            Tarifa? tarifa = tarifas.FirstOrDefault(item => item.IdTipoServicio == viewModel.IdTipoServicio && item.Estado);
            TipoVia? tipoVia = tiposVia.FirstOrDefault(item => item.IdTipoVia == viewModel.IdTipoVia && item.Estado);

            var resultado = CrearViewModelBase();
            resultado.IdTipoServicio = viewModel.IdTipoServicio;
            resultado.IdZona = viewModel.IdZona;
            resultado.IdTipoVia = viewModel.IdTipoVia;
            resultado.DistanciaKilometros = viewModel.DistanciaKilometros;
            resultado.TiempoMinutos = viewModel.TiempoMinutos;
            resultado.AplicaLluvia = viewModel.AplicaLluvia;
            resultado.AplicaAltaDemanda = viewModel.AplicaAltaDemanda;
            resultado.AplicaHorarioNocturno = viewModel.AplicaHorarioNocturno;

            if (tarifa == null || tipoVia == null)
            {
                return View(resultado);
            }

            decimal subtotal = calculadoraTarifaService.CalcularSubtotal(
                tarifa,
                viewModel.DistanciaKilometros,
                viewModel.TiempoMinutos);

            decimal incrementoReglas = CalcularIncrementoReglas(
                subtotal,
                reglas,
                viewModel.IdTipoServicio,
                viewModel.IdZona,
                viewModel.AplicaLluvia,
                viewModel.AplicaAltaDemanda,
                viewModel.AplicaHorarioNocturno);

            decimal incrementoTipoVia = calculadoraTarifaService.CalcularIncrementoPorcentual(
                subtotal,
                tipoVia.PorcentajeIncremento);

            decimal tarifaEstimada = subtotal + incrementoReglas + incrementoTipoVia;

            resultado.TarifaBase = tarifa.TarifaBase;
            resultado.CostoDistancia = tarifa.CostoPorKilometro * viewModel.DistanciaKilometros;
            resultado.CostoTiempo = tarifa.CostoPorMinuto * viewModel.TiempoMinutos;
            resultado.IncrementoReglas = incrementoReglas;
            resultado.IncrementoTipoVia = incrementoTipoVia;
            resultado.TarifaEstimada = calculadoraTarifaService.AplicarTarifaMinima(tarifaEstimada, tarifa.TarifaMinima);
            resultado.TieneResultado = true;

            return View(resultado);
        }

        private SimuladorTarifaViewModel CrearViewModelBase()
        {
            return new SimuladorTarifaViewModel
            {
                TiposServicio = tipoServicioService.ListarTiposServicio()
                    .Select(item => new SelectListItem(item.Nombre, item.IdTipoServicio.ToString()))
                    .ToList(),
                Zonas = zonaService.ListarZonas()
                    .Select(item => new SelectListItem(item.Nombre, item.IdZona.ToString()))
                    .ToList(),
                TiposVia = tipoViaService.ListarTiposVia()
                    .Select(item => new SelectListItem(item.Nombre, item.IdTipoVia.ToString()))
                    .ToList()
            };
        }

        private decimal CalcularIncrementoReglas(
            decimal subtotal,
            List<ReglaTarifa> reglas,
            int idTipoServicio,
            int idZona,
            bool aplicaLluvia,
            bool aplicaAltaDemanda,
            bool aplicaHorarioNocturno)
        {
            decimal totalIncremento = 0;

            foreach (var regla in reglas.Where(item => item.Estado))
            {
                bool aplica = regla.TipoRegla switch
                {
                    "Clima" => aplicaLluvia && (regla.IdTipoServicio == null || regla.IdTipoServicio == idTipoServicio),
                    "Demanda" => aplicaAltaDemanda,
                    "Horario" => aplicaHorarioNocturno,
                    "Zona" => regla.IdZona == idZona,
                    _ => false
                };

                if (!aplica)
                {
                    continue;
                }

                totalIncremento += calculadoraTarifaService.CalcularIncrementoPorcentual(
                    subtotal,
                    regla.PorcentajeIncremento);

                totalIncremento += regla.MontoIncremento;
            }

            return totalIncremento;
        }
    }
}
