using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class AuditoriaConductorFiltroViewModel
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public int? IdConductor { get; set; }

        public string Accion { get; set; } = string.Empty;

        public List<SelectListItem> Conductores { get; set; } = new List<SelectListItem>();

        public List<AuditoriaConductorViewModel> Auditorias { get; set; } = new List<AuditoriaConductorViewModel>();
    }
}
