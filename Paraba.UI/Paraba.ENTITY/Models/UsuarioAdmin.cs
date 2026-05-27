namespace Paraba.ENTITY.Models
{
    public class UsuarioAdmin
    {
        public int IdUsuarioAdmin { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public int PasswordIterations { get; set; }

        public bool Estado { get; set; }

        public int IntentosFallidos { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        public DateTime FechaRegistro { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
