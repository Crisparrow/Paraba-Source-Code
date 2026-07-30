namespace Paraba.API.Models;

public class MonthlyDriverSubscriptionRequest
{
    public int IdConductor { get; set; }
    public DateTime PeriodoInicio { get; set; } = DateTime.Today;
    public string EstadoPago { get; set; } = "Pendiente";
}

public sealed class MototaxiSlotRequest : MonthlyDriverSubscriptionRequest
{
    public int NumeroRanura { get; set; }
}
