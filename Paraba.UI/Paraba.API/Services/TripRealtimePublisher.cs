using Microsoft.AspNetCore.SignalR;
using Paraba.API.Hubs;

namespace Paraba.API.Services;

public sealed class TripRealtimePublisher
{
    private readonly IHubContext<TripHub> hubContext;

    public TripRealtimePublisher(IHubContext<TripHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    public async Task PublishAsync(int idConductor, int idViaje, string eventType)
    {
        var payload = new
        {
            idViaje,
            idConductor,
            tipoEvento = eventType,
            fecha = DateTimeOffset.UtcNow
        };

        await Task.WhenAll(
            hubContext.Clients.Group(TripHub.DriverGroup(idConductor)).SendAsync("TripChanged", payload),
            hubContext.Clients.Group(TripHub.PanelGroup).SendAsync("TripChanged", payload));
    }
}
