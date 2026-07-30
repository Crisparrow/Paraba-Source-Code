namespace Paraba.API.Models
{
    public class DriverProfileResponse
    {
        public int IdConductor { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public bool Disponible { get; set; }

        public bool Verificado { get; set; }

        public bool Activo { get; set; }

        public string EstadoAprobacion { get; set; } = string.Empty;

        public bool PuedeTrabajar { get; set; }

        public List<DriverVehicleResponse> Vehiculos { get; set; } = new();

        public List<DriverDocumentResponse> Documentos { get; set; } = new();
    }

    public class DriverVehicleResponse
    {
        public int IdVehiculo { get; set; }

        public int IdTipoServicio { get; set; }

        public string Placa { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int Anio { get; set; }

        public string TipoServicio { get; set; } = string.Empty;

        public string CategoriaVehiculo { get; set; } = string.Empty;

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public bool Verificado { get; set; }

        public bool Activo { get; set; }
    }

    public class DriverDocumentResponse
    {
        public int IdDocumentoConductor { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string UrlArchivo { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public string Observacion { get; set; } = string.Empty;
    }

    public class DriverOperationsSummaryResponse
    {
        public int IdConductor { get; set; }

        public bool Conectado { get; set; }

        public int Prioridad { get; set; }

        public int PedidosDisponibles { get; set; }

        public int ViajesActivos { get; set; }

        public int ViajesHoy { get; set; }

        public int ViajesFinalizadosHoy { get; set; }

        public decimal GananciaHoy { get; set; }

        public string ObjetivoTitulo { get; set; } = string.Empty;

        public string ObjetivoDetalle { get; set; } = string.Empty;

        public decimal ObjetivoActual { get; set; }

        public decimal ObjetivoMeta { get; set; }

        public string EstadoOperativo { get; set; } = string.Empty;
    }

    public class DriverTripResponse
    {
        public int IdViaje { get; set; }

        public int IdPasajero { get; set; }

        public int IdConductor { get; set; }

        public int IdVehiculo { get; set; }

        public int IdTipoServicio { get; set; }

        public string TipoServicio { get; set; } = string.Empty;

        public string Origen { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;

        public decimal TarifaSugerida { get; set; }

        public decimal TarifaOfertada { get; set; }

        public decimal? TarifaContraoferta { get; set; }

        public decimal? TarifaAceptada { get; set; }

        public decimal TarifaFinal { get; set; }

        public int IdEstadoViaje { get; set; }

        public string EstadoViaje { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public DateTime? FechaAceptacion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        public string MotivoCancelacion { get; set; } = string.Empty;
    }

    public class DriverCounterOfferRequest
    {
        public decimal TarifaContraoferta { get; set; }
    }

    public class DriverCancelTripRequest
    {
        public string Motivo { get; set; } = string.Empty;
    }

    public class DriverDemoTripRequest
    {
        public int? IdTipoServicio { get; set; }
    }

    public class DriverAvailabilityRequest
    {
        public bool Disponible { get; set; }
    }
}
