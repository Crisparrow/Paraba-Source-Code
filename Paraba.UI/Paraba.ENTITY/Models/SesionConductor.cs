namespace Paraba.ENTITY.Models
{
    public class SesionConductor
    {
        public int IdSesionConductor { get; set; }

        public int IdConductor { get; set; }

        public string TokenSesion { get; set; } = string.Empty;

        public string Dispositivo { get; set; } = string.Empty;

        public DateTime FechaExpiracion { get; set; }

        public bool Activa { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
