using Microsoft.AspNetCore.SignalR;

namespace Paraba.API.Hubs;

public sealed class TripHub : Hub
{
    public const string PanelGroup = "panel:viajes";

    public override async Task OnConnectedAsync()
    {
        string? driverId = Context.GetHttpContext()?.Request.Query["idConductor"];

        if (int.TryParse(driverId, out int parsedDriverId) && parsedDriverId > 0)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, DriverGroup(parsedDriverId));
        }

        await base.OnConnectedAsync();
    }

    public Task SubscribeDriver(int idConductor)
    {
        if (idConductor <= 0)
        {
            throw new HubException("El conductor no es valido.");
        }

        return Groups.AddToGroupAsync(Context.ConnectionId, DriverGroup(idConductor));
    }

    public Task SubscribePanel()
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, PanelGroup);
    }

    public static string DriverGroup(int idConductor) => $"conductor:{idConductor}";
}
