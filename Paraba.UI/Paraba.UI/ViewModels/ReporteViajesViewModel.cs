using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class ReporteViajesViewModel
    {
        public DateTime? FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public int? IdEstadoViaje { get; set; }

        public int? IdTipoServicio { get; set; }

        public int? IdConductor { get; set; }

        public int TotalViajes { get; set; }

        public int TotalFinalizados { get; set; }

        public int TotalCancelados { get; set; }

        public decimal IngresosFinalizados { get; set; }

        public decimal PromedioTarifaAceptada { get; set; }

        public List<SelectListItem> EstadosViaje { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> TiposServicio { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Conductores { get; set; } = new List<SelectListItem>();

        public List<ReporteViajeItemViewModel> Viajes { get; set; } = new List<ReporteViajeItemViewModel>();
    }
}
