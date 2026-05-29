using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class LiquidacionesViewModel
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public int? IdConductor { get; set; }

        public decimal PorcentajeComision { get; set; } = 10;

        public int TotalViajesFinalizados { get; set; }

        public decimal TotalBruto { get; set; }

        public decimal TotalComisionParaba { get; set; }

        public decimal TotalNetoConductores { get; set; }

        public List<SelectListItem> Conductores { get; set; } = new List<SelectListItem>();

        public List<LiquidacionItemViewModel> Liquidaciones { get; set; } = new List<LiquidacionItemViewModel>();
    }
}
