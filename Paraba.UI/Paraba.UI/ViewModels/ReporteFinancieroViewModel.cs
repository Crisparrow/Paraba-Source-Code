namespace Paraba.UI.ViewModels
{
    public class ReporteFinancieroViewModel
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int ViajesFinalizados { get; set; }
        public decimal TotalBrutoViajes { get; set; }
        public decimal TotalComisionParaba { get; set; }
        public decimal TotalNetoConductores { get; set; }
        public decimal NetoPendientePago { get; set; }
        public decimal NetoPagado { get; set; }
        public int LiquidacionesCerradas { get; set; }
        public int LiquidacionesPagadas { get; set; }
        public List<ReporteFinancieroItemViewModel> Items { get; set; } = new List<ReporteFinancieroItemViewModel>();
    }
}
