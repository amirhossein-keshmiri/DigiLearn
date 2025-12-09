using Common.Application;

namespace CoreModule.Application.Courses.Episodes.AcceptEpisode
{
  public record AcceptCourseEpisodeCommand(Guid CourseId, Guid EpisodeId) : IBaseCommand;

}
