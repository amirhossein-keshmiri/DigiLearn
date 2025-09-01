using Common.Application;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Notifications.Seen
{
  public class SeenNotificationCommandHandler : IBaseCommandHandler<SeenNotificationCommand>
  {
    private UserContext _userContext;
    public SeenNotificationCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(SeenNotificationCommand request, CancellationToken cancellationToken)
    {
      var notification = await _userContext.UserNotifications
                              .FirstOrDefaultAsync(f => f.Id == request.NotificationId && f.IsSeen == false && f.UserId == request.UserId , cancellationToken);
      if (notification == null)
      {
        return OperationResult.NotFound();
      }
      notification.IsSeen = true;
      _userContext.Update(notification);
      await _userContext.SaveChangesAsync(cancellationToken);

      return OperationResult.Success();
    }
  }
}
