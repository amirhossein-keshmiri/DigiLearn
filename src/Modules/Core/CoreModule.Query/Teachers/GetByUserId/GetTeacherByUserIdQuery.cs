using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Teachers.GetByUserId
{
  public record GetTeacherByUserIdQuery(Guid UserId) : IQuery<TeacherDto?>;
}
