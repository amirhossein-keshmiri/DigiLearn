using FluentValidation;

namespace CoreModule.Application.Categories.Create
{
  public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
  {
    public CreateCategoryCommandValidator()
    {
      RuleFor(f => f.Title)
          .NotEmpty()
          .NotNull();

      RuleFor(f => f.Slug)
          .NotEmpty()
          .NotNull();
    }
  }
}
