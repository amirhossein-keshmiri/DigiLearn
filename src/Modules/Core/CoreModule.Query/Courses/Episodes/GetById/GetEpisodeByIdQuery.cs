using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Courses.Episodes.GetById
{
  public record GetEpisodeByIdQuery(Guid EpisodeId) : IQuery<EpisodeDto?>;
}
