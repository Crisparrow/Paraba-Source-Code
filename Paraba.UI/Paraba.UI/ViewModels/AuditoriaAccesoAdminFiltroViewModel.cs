namespace Paraba.UI.ViewModels
{
    public class AuditoriaAccesoAdminFiltroViewModel
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public string Correo { get; set; } = string.Empty;

        public bool? Exitoso { get; set; }

        public List<AuditoriaAccesoAdminViewModel> Auditorias { get; set; } = new List<AuditoriaAccesoAdminViewModel>();
    }
}
