using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class HistorialLiquidacionesViewModel
    {
        public string Estado { get; set; } = string.Empty;

        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public int TotalLiquidaciones { get; set; }

        public int TotalCerradas { get; set; }

        public int TotalPagadas { get; set; }

        public int TotalAnuladas { get; set; }

        public decimal TotalNetoPendientePago { get; set; }

        public decimal TotalComisionParaba { get; set; }

        public List<SelectListItem> Estados { get; set; } = new List<SelectListItem>();

        public List<LiquidacionCerradaViewModel> Liquidaciones { get; set; } = new List<LiquidacionCerradaViewModel>();
    }
}
