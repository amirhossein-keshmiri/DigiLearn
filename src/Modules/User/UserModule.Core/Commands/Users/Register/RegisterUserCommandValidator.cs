using Common.Application.Validation;
using FluentValidation;

namespace UserModule.Core.Commands.Users.Register
{
  public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
  {
    public RegisterUserCommandValidator()
    {
      RuleFor(r => r.PhoneNumber)
          .NotEmpty()
          .NotNull()
          .MinimumLength(11)
          .MaximumLength(11)
          .WithMessage(ValidationMessages.InvalidPhoneNumber);

      RuleFor(r => r.Password)
          .NotEmpty()
          .NotNull()
          .MinimumLength(6);
    }
  }
}
