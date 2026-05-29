namespace Paraba.UI.ViewModels
{
    public class DetalleConductorViewModel
    {
        public int IdConductor { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string LicenciaConducir { get; set; } = string.Empty;

        public DateTime FechaVencimientoLicencia { get; set; }

        public bool Disponible { get; set; }

        public bool Verificado { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string TipoServicio { get; set; } = string.Empty;

        public string Vehiculo { get; set; } = string.Empty;

        public string Placa { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public bool VehiculoVerificado { get; set; }

        public decimal PromedioCalificacion { get; set; }

        public List<DocumentoConductorViewModel> Documentos { get; set; } = new List<DocumentoConductorViewModel>();

        public List<CalificacionViewModel> Calificaciones { get; set; } = new List<CalificacionViewModel>();

        public List<AuditoriaConductorViewModel> Auditorias { get; set; } = new List<AuditoriaConductorViewModel>();
    }
}
