using FluentValidation;

namespace UserModule.Core.Commands.Users.ChangePassword
{
  public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
  {
    public ChangeUserPasswordCommandValidator()
    {
      RuleFor(f => f.CurrentPassword)
            .NotEmpty()
            .NotNull();

      RuleFor(f => f.NewPassword)
            .NotEmpty()
            .NotNull()
            .MinimumLength(6);
    }
  }
}
