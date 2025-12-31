using Common.EventBus.Abstractions;

namespace Common.EventBus.Events;

public class UserRegistered : IntegrationEvent
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Family { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
    public string Avatar { get; set; }
}
