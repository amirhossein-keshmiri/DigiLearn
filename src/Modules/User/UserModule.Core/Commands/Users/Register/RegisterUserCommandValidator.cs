using Common.Application.Validation;
using Common.Domain.Utils;
using FluentValidation;

namespace UserModule.Core.Commands.Users.Register
{
  public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
  {
    public RegisterUserCommandValidator()
    {
      RuleFor(r => r.PhoneNumber)
          .Must(x => x.IsValidPhoneNumber())
          .WithMessage(ValidationMessages.InvalidPhoneNumber);

      RuleFor(r => r.Password)
          .NotEmpty()
          .NotNull()
          .MinimumLength(5)
          .WithMessage(ValidationMessages.MinLength);
    }
  }
}
