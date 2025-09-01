using Common.Application;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Notifications.Delete
{
  public class DeleteNotificationCommandHandler : IBaseCommandHandler<DeleteNotificationCommand>
  {
    private readonly UserContext _userContext;

    public DeleteNotificationCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
      var notification = await _userContext.UserNotifications.FirstOrDefaultAsync(f => f.UserId == request.UserId && f.Id == request.NotificationId, cancellationToken);
      if (notification == null)
      {
        return OperationResult.NotFound();
      }

      _userContext.UserNotifications.Remove(notification);
      await _userContext.SaveChangesAsync(cancellationToken);
      return OperationResult.Success();
    }
  }
}
