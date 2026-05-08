using Microsoft.AspNetCore.SignalR;

namespace StarshipForge.Api.Hubs;

/// <summary>
/// SignalR hub for streaming 3D state updates from the AI agent to the frontend.
/// </summary>
public class DesignHub : Hub
{
    /// <summary>
    /// Sends a 3D state update to all connected clients.
    /// </summary>
    public async Task SendStateUpdate(object state)
    {
        await Clients.All.SendAsync("ReceiveStateUpdate", state);
    }
}