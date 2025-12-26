using FluentValidation;

namespace CoreModule.Application.Courses.Sections.EditSection
{
    public class EditCourseSectionCommandValidator : AbstractValidator<EditCourseSectionCommand>
    {
        public EditCourseSectionCommandValidator()
        {
            RuleFor(r => r.Title)
                .NotEmpty()
                .NotNull();
        }
    }
}
