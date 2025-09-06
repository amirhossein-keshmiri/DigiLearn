using FluentValidation;

namespace CoreModule.Application.Courses.Create
{
  public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
  {
    public CreateCourseCommandValidator()
    {
      RuleFor(r => r.Title)
          .NotNull()
          .NotEmpty();

      RuleFor(r => r.Slug)
          .NotNull()
          .NotEmpty();


      RuleFor(r => r.Description)
          .NotNull()
          .NotEmpty();


      RuleFor(r => r.ImageFile)
          .NotNull();
    }
  }
}
