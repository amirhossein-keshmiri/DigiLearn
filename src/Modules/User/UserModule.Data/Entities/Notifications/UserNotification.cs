using Common.Domain;
using System.ComponentModel.DataAnnotations;

namespace UserModule.Data.Entities.Notifications
{
  public class UserNotification : BaseEntity
  {
    public Guid UserId { get; set; }

    [MaxLength(255)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string Text { get; set; }
    public bool IsSeen { get; set; }
  }
}
