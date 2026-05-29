using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class IntervencionLiquidacionViewModel
    {
        public int IdLiquidacionConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public decimal TotalNetoConductor { get; set; }

        public string Accion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar una observacion administrativa.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "La observacion debe tener entre 10 y 300 caracteres.")]
        public string Observacion { get; set; } = string.Empty;
    }
}
