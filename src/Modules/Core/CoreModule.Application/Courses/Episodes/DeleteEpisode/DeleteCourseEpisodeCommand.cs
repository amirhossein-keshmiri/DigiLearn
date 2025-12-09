using Common.Application;

namespace CoreModule.Application.Courses.Episodes.DeleteEpisode
{
  public record DeleteCourseEpisodeCommand(Guid CourseId, Guid EpisodeId) : IBaseCommand;

}
