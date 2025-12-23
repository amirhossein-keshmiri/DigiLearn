using Common.Domain;

namespace CoreModule.Domain.Teachers.Events
{
    public class AcceptTeacherRequestEvent : BaseDomainEvent
  {
    public Guid UserId { get; set; }
  }
}
