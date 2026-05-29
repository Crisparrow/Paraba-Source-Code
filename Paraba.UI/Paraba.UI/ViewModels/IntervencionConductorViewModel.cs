using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class IntervencionConductorViewModel
    {
        public int IdConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string EstadoActual { get; set; } = string.Empty;

        public string Accion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar el motivo de la intervencion administrativa.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "El motivo debe tener entre 10 y 300 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
