namespace Paraba.UI.ViewModels
{
    public class DetalleViajeViewModel
    {
        public int IdViaje { get; set; }

        public string EstadoViaje { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Origen { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;

        public decimal TarifaSugerida { get; set; }

        public decimal TarifaOfertada { get; set; }

        public decimal? TarifaContraoferta { get; set; }

        public decimal? TarifaAceptada { get; set; }

        public decimal TarifaFinal { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        public string MotivoCancelacion { get; set; } = string.Empty;

        public string Pasajero { get; set; } = string.Empty;

        public string DocumentoPasajero { get; set; } = string.Empty;

        public string TelefonoPasajero { get; set; } = string.Empty;

        public string CorreoPasajero { get; set; } = string.Empty;

        public string Conductor { get; set; } = string.Empty;

        public string DocumentoConductor { get; set; } = string.Empty;

        public string TelefonoConductor { get; set; } = string.Empty;

        public string CorreoConductor { get; set; } = string.Empty;

        public string Vehiculo { get; set; } = string.Empty;

        public string Placa { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public bool PuedeCancelar { get; set; }

        public List<AuditoriaViajeViewModel> Auditorias { get; set; } = new List<AuditoriaViajeViewModel>();
    }
}
