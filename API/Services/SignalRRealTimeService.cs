using API.Hubs;
using Application.Abstractions.Services;
using Domain.Entities.Notification;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

public class SignalRRealTimeService : IRealTimeService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRRealTimeService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(long userId, Notification notification, CancellationToken ct = default)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification, ct);
    }
}
