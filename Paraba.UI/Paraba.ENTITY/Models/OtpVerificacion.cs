namespace Paraba.ENTITY.Models
{
    public class OtpVerificacion
    {
        public int IdOtpVerificacion { get; set; }

        public string Telefono { get; set; } = string.Empty;

        public string CodigoHash { get; set; } = string.Empty;

        public string Canal { get; set; } = string.Empty;

        public DateTime FechaExpiracion { get; set; }

        public bool Verificado { get; set; }

        public bool Usado { get; set; }

        public int Intentos { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaVerificacion { get; set; }
    }
}
