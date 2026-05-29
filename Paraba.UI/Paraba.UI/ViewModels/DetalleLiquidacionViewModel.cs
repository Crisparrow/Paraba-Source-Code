namespace Paraba.UI.ViewModels
{
    public class DetalleLiquidacionViewModel
    {
        public int IdLiquidacionConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public decimal PorcentajeComision { get; set; }

        public decimal TotalBruto { get; set; }

        public decimal TotalComisionParaba { get; set; }

        public decimal TotalNetoConductor { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string UsuarioCierre { get; set; } = string.Empty;

        public DateTime FechaCierre { get; set; }

        public DateTime? FechaPago { get; set; }

        public string Observacion { get; set; } = string.Empty;

        public List<LiquidacionDetalleItemViewModel> Detalles { get; set; } = new List<LiquidacionDetalleItemViewModel>();
    }
}
