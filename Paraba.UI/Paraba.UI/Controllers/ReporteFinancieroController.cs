using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finanzas")]
    public class ReporteFinancieroController : Controller
    {
        private readonly ViajeService viajeService = new ViajeService();
        private readonly LiquidacionConductorService liquidacionConductorService = new LiquidacionConductorService();
        private readonly ConductorService conductorService = new ConductorService();

        public IActionResult Index(ReporteFinancieroViewModel filtros)
        {
            var viajes = viajeService.ListarViajes().Where(item => item.IdEstadoViaje == 4).AsEnumerable();
            var liquidaciones = liquidacionConductorService.ListarLiquidaciones().AsEnumerable();
            var conductores = conductorService.ListarConductores();

            if (filtros.FechaDesde != null)
            {
                viajes = viajes.Where(item => item.FechaFin != null && item.FechaFin.Value.Date >= filtros.FechaDesde.Value.Date);
                liquidaciones = liquidaciones.Where(item => item.FechaCierre.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                viajes = viajes.Where(item => item.FechaFin != null && item.FechaFin.Value.Date <= filtros.FechaHasta.Value.Date);
                liquidaciones = liquidaciones.Where(item => item.FechaCierre.Date <= filtros.FechaHasta.Value.Date);
            }

            var viajesLista = viajes.ToList();
            var liquidacionesLista = liquidaciones.ToList();

            filtros.ViajesFinalizados = viajesLista.Count;
            filtros.TotalBrutoViajes = viajesLista.Sum(item => item.TarifaFinal);
            filtros.TotalComisionParaba = liquidacionesLista.Where(item => item.Estado != "Anulada").Sum(item => item.TotalComisionParaba);
            filtros.TotalNetoConductores = liquidacionesLista.Where(item => item.Estado != "Anulada").Sum(item => item.TotalNetoConductor);
            filtros.NetoPendientePago = liquidacionesLista.Where(item => item.Estado == "Cerrada").Sum(item => item.TotalNetoConductor);
            filtros.NetoPagado = liquidacionesLista.Where(item => item.Estado == "Pagada").Sum(item => item.TotalNetoConductor);
            filtros.LiquidacionesCerradas = liquidacionesLista.Count(item => item.Estado == "Cerrada");
            filtros.LiquidacionesPagadas = liquidacionesLista.Count(item => item.Estado == "Pagada");
            filtros.Items = liquidacionesLista.Select(item => new ReporteFinancieroItemViewModel
            {
                IdLiquidacionConductor = item.IdLiquidacionConductor,
                Conductor = conductores.FirstOrDefault(c => c.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                Estado = item.Estado,
                FechaCierre = item.FechaCierre,
                TotalBruto = item.TotalBruto,
                ComisionParaba = item.TotalComisionParaba,
                NetoConductor = item.TotalNetoConductor
            }).ToList();

            return View(filtros);
        }
    }
}
