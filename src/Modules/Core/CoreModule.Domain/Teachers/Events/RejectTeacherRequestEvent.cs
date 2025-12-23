using Common.Domain;

namespace CoreModule.Domain.Teachers.Events
{
  public class RejectTeacherRequestEvent : BaseDomainEvent
  {
    public string Description { get; set; }
    public Guid UserId { get; set; }
  }
}
