namespace Paraba.UI.ViewModels
{
    public class UsuarioAdminViewModel
    {
        public int IdUsuarioAdmin { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public int IntentosFallidos { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
