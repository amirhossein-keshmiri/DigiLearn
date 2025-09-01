using Common.Application;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Notifications.DeleteAll
{
  public class DeleteAllNotificationCommandHandler : IBaseCommandHandler<DeleteAllNotificationCommand>
  {
    private readonly UserContext _userContext;

    public DeleteAllNotificationCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(DeleteAllNotificationCommand request, CancellationToken cancellationToken)
    {
      var notifications = await _userContext.UserNotifications.Where(f => f.UserId == request.UserId).ToListAsync(cancellationToken);
      if (notifications.Any() || notifications.Count > 0)
      {
        _userContext.UserNotifications.RemoveRange(notifications);
        await _userContext.SaveChangesAsync(cancellationToken);
      }

      return OperationResult.Success();
    }
  }
}
