namespace Paraba.ENTITY.Models
{
    public class Pasajero
    {
        public int IdPasajero { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public bool Verificado { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
