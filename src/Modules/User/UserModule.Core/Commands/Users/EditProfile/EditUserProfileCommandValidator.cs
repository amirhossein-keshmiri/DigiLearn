using FluentValidation;

namespace UserModule.Core.Commands.Users.EditProfile
{
  public class EditUserProfileCommandValidator : AbstractValidator<EditUserProfileCommand>
  {
    public EditUserProfileCommandValidator()
    {
      RuleFor(f => f.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Name cannot be null!");

      RuleFor(f => f.Family)
            .NotEmpty()
            .NotNull()
            .WithMessage("Family cannot be null!");
    }
  }
}
