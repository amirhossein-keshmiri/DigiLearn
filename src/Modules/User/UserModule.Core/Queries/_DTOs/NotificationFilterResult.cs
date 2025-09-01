using Common.Query;
using Common.Query.Filter;

namespace UserModule.Core.Queries._DTOs
{
  public class NotificationFilterResult : BaseFilter<NotificationFilterData>
  {
  }

  public class NotificationFilterParams : BaseFilterParam
  {
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public bool? IsSeen { get; set; }
  }

  public class NotificationFilterData : BaseDto
  {
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public bool IsSeen { get; set; }
    public string PersianCreationDate { get; set; }
  }
}
