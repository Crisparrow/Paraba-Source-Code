using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services;

public sealed class PartnerMobilityService
{
    private readonly PartnerMobilityRepository repository = new();
    private static readonly HashSet<string> PaymentStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pendiente", "Pagado", "Vencido"
    };

    public List<RutaMicrobus> ListRoutes() => repository.ListRoutes();
    public List<AsociacionMototaxi> ListAssociations() => repository.ListAssociations();

    public int SubscribeDriverToRoute(int routeId, int driverId, DateTime periodStart, string paymentStatus)
    {
        ValidatePaymentStatus(paymentStatus);
        return Execute(() => repository.SubscribeDriverToRoute(routeId, driverId, periodStart, paymentStatus));
    }

    public int AssignAssociationSlot(int associationId, int driverId, int slotNumber, DateTime periodStart, string paymentStatus)
    {
        ValidatePaymentStatus(paymentStatus);
        return Execute(() => repository.AssignAssociationSlot(associationId, driverId, slotNumber, periodStart, paymentStatus));
    }

    private static void ValidatePaymentStatus(string paymentStatus)
    {
        if (!PaymentStatuses.Contains(paymentStatus ?? string.Empty))
        {
            throw new ArgumentException("El estado de pago debe ser Pendiente, Pagado o Vencido.");
        }
    }

    private static int Execute(Func<int> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex) when (ex.GetType().Name == "SqlException")
        {
            throw new ArgumentException(ex.Message, ex);
        }
    }
}
