using FluentValidation;

namespace UserModule.Core.Commands.Notifications.Create
{
  public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
  {
    public CreateNotificationCommandValidator()
    {
      RuleFor(r => r.Title).NotEmpty().NotNull();
      RuleFor(r => r.Text).NotEmpty().NotNull();
    }
  }
}
