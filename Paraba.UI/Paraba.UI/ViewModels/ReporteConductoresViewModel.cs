namespace Paraba.UI.ViewModels
{
    public class ReporteConductoresViewModel
    {
        public bool? Verificado { get; set; }

        public bool? Disponible { get; set; }

        public bool? Estado { get; set; }

        public int TotalConductores { get; set; }

        public int TotalVerificados { get; set; }

        public int TotalDisponibles { get; set; }

        public int TotalSuspendidos { get; set; }

        public decimal IngresosFinalizados { get; set; }

        public decimal PromedioGeneralCalificacion { get; set; }

        public List<ReporteConductorItemViewModel> Conductores { get; set; } = new List<ReporteConductorItemViewModel>();
    }
}
