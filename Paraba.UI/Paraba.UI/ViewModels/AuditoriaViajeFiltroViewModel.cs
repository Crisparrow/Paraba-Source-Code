namespace Paraba.UI.ViewModels
{
    public class AuditoriaViajeFiltroViewModel
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public int? IdViaje { get; set; }

        public string Accion { get; set; } = string.Empty;

        public List<AuditoriaViajeViewModel> Auditorias { get; set; } = new List<AuditoriaViajeViewModel>();
    }
}
