using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class IntervencionViajeViewModel
    {
        public int IdViaje { get; set; }

        public string Pasajero { get; set; } = string.Empty;

        public string Conductor { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public string EstadoViaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar el motivo de la intervencion administrativa.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "El motivo debe tener entre 10 y 300 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
