using Common.Application;

namespace UserModule.Core.Commands.Notifications.Seen
{
  public record SeenNotificationCommand(Guid NotificationId, Guid UserId) : IBaseCommand
  {
  }
}
