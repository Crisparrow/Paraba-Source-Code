namespace Paraba.API.Models;

public sealed class PassengerTripRequest
{
    public int IdPasajero { get; set; }
    public int IdConductor { get; set; }
    public int IdVehiculo { get; set; }
    public int IdTipoServicio { get; set; }
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public decimal TarifaSugerida { get; set; }
    public decimal? TarifaOfertada { get; set; }
}

public sealed class PassengerCounterOfferDecisionRequest
{
    public bool Aceptada { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
