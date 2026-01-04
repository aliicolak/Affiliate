using Domain.Entities.Notification;

namespace Application.Abstractions.Services;

public interface IRealTimeService
{
    Task SendNotificationAsync(long userId, Notification notification, CancellationToken ct = default);
}
