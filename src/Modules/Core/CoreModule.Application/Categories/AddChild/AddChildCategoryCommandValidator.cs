using FluentValidation;

namespace CoreModule.Application.Categories.AddChild
{
  public class AddChildCategoryCommandValidator : AbstractValidator<AddChildCategoryCommand>
  {
    public AddChildCategoryCommandValidator()
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
