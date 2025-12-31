using Common.EventBus.Abstractions;

namespace Common.EventBus.Events;

public class UserEdited : IntegrationEvent
{
  public Guid UserId { get; set; }
  public string? Name { get; set; }
  public string? Family { get; set; }
  public string? Email { get; set; }
  public string PhoneNumber { get; set; }
}