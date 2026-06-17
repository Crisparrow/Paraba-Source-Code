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

        public bool Verificado { get; set; }

        public bool Activo { get; set; }
    }

    public class DriverDocumentResponse
    {
        public int IdDocumentoConductor { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string EstadoVerificacion { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public string Observacion { get; set; } = string.Empty;
    }

    public class DriverTripResponse
    {
        public int IdViaje { get; set; }

        public int IdPasajero { get; set; }

        public int IdConductor { get; set; }

        public int IdVehiculo { get; set; }

        public int IdTipoServicio { get; set; }

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

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
    }

    public class DriverCounterOfferRequest
    {
        public decimal TarifaContraoferta { get; set; }
    }

    public class DriverCancelTripRequest
    {
        public string Motivo { get; set; } = string.Empty;
    }
}
