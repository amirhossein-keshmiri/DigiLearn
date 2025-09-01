using Common.Application;

namespace UserModule.Core.Commands.Notifications.SeenAll
{
  public record SeenAllNotificationsCommand(Guid UserId) : IBaseCommand;
}
