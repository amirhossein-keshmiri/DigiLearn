using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Teachers.GetById
{
  public record GetTeacherByIdQuery(Guid Id) : IQuery<TeacherDto?>;

}
