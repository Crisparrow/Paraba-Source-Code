using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class ReclamoViewModel
    {
        public int IdReclamo { get; set; }
        public int? IdViaje { get; set; }
        public int? IdPasajero { get; set; }
        public int? IdConductor { get; set; }
        [Required]
        public string TipoReclamo { get; set; } = string.Empty;
        [Required]
        [MinLength(10)]
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";
        public string Pasajero { get; set; } = string.Empty;
        public string Conductor { get; set; } = string.Empty;
        public string UsuarioRegistro { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string ObservacionCierre { get; set; } = string.Empty;
    }
}
