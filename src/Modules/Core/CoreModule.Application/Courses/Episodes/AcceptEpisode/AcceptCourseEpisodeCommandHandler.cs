using Common.Application;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses.Episodes.AcceptEpisode
{
  public class AcceptCourseEpisodeCommandHandler : IBaseCommandHandler<AcceptCourseEpisodeCommand>
  {
    private readonly ICourseRepository _repository;

    public AcceptCourseEpisodeCommandHandler(ICourseRepository repository)
    {
      _repository = repository;
    }

    public async Task<OperationResult> Handle(AcceptCourseEpisodeCommand request, CancellationToken cancellationToken)
    {
      var course = await _repository.GetTracking(request.CourseId);
      if (course == null)
      {
        return OperationResult.NotFound();
      }

      course.AcceptEpisode(request.EpisodeId);
      await _repository.Save();
      return OperationResult.Success();
    }
  }
}
