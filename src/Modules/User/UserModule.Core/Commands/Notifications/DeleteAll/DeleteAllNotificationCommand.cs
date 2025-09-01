using Common.Application;
using MediatR;

namespace UserModule.Core.Commands.Notifications.DeleteAll
{
  public record DeleteAllNotificationCommand(Guid UserId) : IBaseCommand;
}
