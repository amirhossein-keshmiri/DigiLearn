using Common.Application;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Notifications.SeenAll
{
  public class SeenAllNotificationsCommandHandler : IBaseCommandHandler<SeenAllNotificationsCommand>
  {
    private readonly UserContext _userContext;

    public SeenAllNotificationsCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(SeenAllNotificationsCommand request, CancellationToken cancellationToken)
    {
      await MarkNotificationsAsSeenAsync(request.UserId, cancellationToken);

      return OperationResult.Success();
    }

    private async Task MarkNotificationsAsSeenAsync(Guid userId, CancellationToken ct)
    {
      await _userContext.UserNotifications
          .Where(n => n.UserId == userId && !n.IsSeen)
          .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsSeen, true), ct);
    }
  }
}
