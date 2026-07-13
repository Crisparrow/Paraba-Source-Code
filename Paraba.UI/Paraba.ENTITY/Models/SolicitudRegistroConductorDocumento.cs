namespace Paraba.ENTITY.Models
{
    public class SolicitudRegistroConductorDocumento
    {
        public int IdSolicitudRegistroConductorDocumento { get; set; }

        public int IdSolicitudRegistroConductor { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string UrlArchivo { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public bool EsOpcional { get; set; }

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaRevision { get; set; }
    }
}
