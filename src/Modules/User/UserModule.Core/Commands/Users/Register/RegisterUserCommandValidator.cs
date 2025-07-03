using Common.Application.Validation;
using FluentValidation;

namespace UserModule.Core.Commands.Users.Register
{
  public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
  {
    public RegisterUserCommandValidator()
    {
      RuleFor(r => r.Password)
          .NotEmpty()
          .NotNull()
          .MinimumLength(6)
          .MaximumLength(11)
          .WithMessage(ValidationMessages.MaxLength);
    }
  }
}
