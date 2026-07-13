namespace Paraba.ENTITY.Models
{
    public class SolicitudRegistroConductor
    {
        public int IdSolicitudRegistroConductor { get; set; }

        public int? IdConductor { get; set; }

        public string Telefono { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string LicenciaConducir { get; set; } = string.Empty;

        public DateTime? FechaVencimientoLicencia { get; set; }

        public int? IdTipoServicio { get; set; }

        public string Placa { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int? Anio { get; set; }

        public string EstadoSolicitud { get; set; } = string.Empty;

        public string EstadoDatosConductor { get; set; } = string.Empty;

        public string EstadoDatosVehiculo { get; set; } = string.Empty;

        public string EstadoDocumentos { get; set; } = string.Empty;

        public string ObservacionRevision { get; set; } = string.Empty;

        public string ObservacionDatosConductor { get; set; } = string.Empty;

        public string ObservacionDatosVehiculo { get; set; } = string.Empty;

        public string ObservacionDocumentos { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public DateTime? FechaRevision { get; set; }
    }
}
