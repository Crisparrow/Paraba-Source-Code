namespace Paraba.UI.ViewModels
{
    public class ReporteFinancieroItemViewModel
    {
        public int IdLiquidacionConductor { get; set; }
        public string Conductor { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCierre { get; set; }
        public decimal TotalBruto { get; set; }
        public decimal ComisionParaba { get; set; }
        public decimal NetoConductor { get; set; }
    }
}
