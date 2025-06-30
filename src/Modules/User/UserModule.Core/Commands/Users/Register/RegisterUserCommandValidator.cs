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
          .MinimumLength(6);
    }
  }
}
