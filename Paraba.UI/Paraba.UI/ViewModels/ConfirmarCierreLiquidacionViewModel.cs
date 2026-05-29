using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class ConfirmarCierreLiquidacionViewModel
    {
        [Required]
        public DateTime? FechaDesde { get; set; }

        [Required]
        public DateTime? FechaHasta { get; set; }

        [Required]
        public int? IdConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public int TotalViajesFinalizados { get; set; }

        public decimal TotalBruto { get; set; }

        public decimal TotalComisionParaba { get; set; }

        public decimal TotalNetoConductor { get; set; }

        [StringLength(300, ErrorMessage = "La observacion no debe superar 300 caracteres.")]
        public string ObservacionCierre { get; set; } = string.Empty;

        public List<LiquidacionItemViewModel> Liquidaciones { get; set; } = new List<LiquidacionItemViewModel>();
    }
}
