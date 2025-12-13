using FluentValidation;

namespace CoreModule.Application.Courses.Episodes.EditEpisode
{
  public class EditEpisodeCommandValidator : AbstractValidator<EditEpisodeCommand>
  {
    public EditEpisodeCommandValidator()
    {
      RuleFor(r => r.Title)
          .NotNull()
          .NotEmpty();
    }
  }
}
