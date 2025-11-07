using FluentValidation;

namespace CoreModule.Application.Courses.Sections.AddSection
{
  public class AddCourseSectionCommandValidator : AbstractValidator<AddCourseSectionCommand>
  {
    public AddCourseSectionCommandValidator()
    {
      RuleFor(r => r.Title)
          .NotEmpty()
          .NotNull();
    }
  }
}
