using Common.Query;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Queries.Notifications.GetByFilter
{
  public class GetNotificationsByFilterQuery : QueryFilter<NotificationFilterResult, NotificationFilterParams>
  {
    public GetNotificationsByFilterQuery(NotificationFilterParams filterParams) : base(filterParams)
    {
    }
  }
}
