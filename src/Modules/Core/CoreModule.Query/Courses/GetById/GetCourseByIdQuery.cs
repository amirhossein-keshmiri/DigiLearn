using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Courses.GetById
{
  public record GetCourseByIdQuery(Guid Id) : IQuery<CourseDto?>;
}
