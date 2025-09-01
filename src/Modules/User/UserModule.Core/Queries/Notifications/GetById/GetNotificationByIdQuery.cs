using Common.Query;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Queries.Notifications.GetById
{
  public class GetNotificationByIdQuery : IQuery<NotificationDto?>
  {
    public GetNotificationByIdQuery(Guid notificationId, Guid userId)
    {
      UserId = userId;
      NotificationId = notificationId;
    }
    public Guid NotificationId { get; private set; }
    public Guid UserId { get; private set; }
  }
}
