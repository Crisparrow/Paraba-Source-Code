namespace Paraba.ENTITY.Models
{
    public class AuditoriaAccesoAdmin
    {
        public int IdAuditoriaAccesoAdmin { get; set; }

        public int? IdUsuarioAdmin { get; set; }

        public string Correo { get; set; } = string.Empty;

        public string Accion { get; set; } = string.Empty;

        public bool Exitoso { get; set; }

        public string IpOrigen { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
    }
}
