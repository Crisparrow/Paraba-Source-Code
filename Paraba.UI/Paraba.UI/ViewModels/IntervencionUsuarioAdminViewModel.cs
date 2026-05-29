using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class IntervencionUsuarioAdminViewModel
    {
        public int IdUsuarioAdmin { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public string EstadoActual { get; set; } = string.Empty;

        public string Accion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar el motivo administrativo.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "El motivo debe tener entre 10 y 300 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
