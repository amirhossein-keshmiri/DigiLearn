using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Courses.GetByFilter
{
  public class GetCoursesByFilterQuery : QueryFilter<CourseFilterResult, CourseFilterParams>
  {
    public GetCoursesByFilterQuery(CourseFilterParams filterParams) : base(filterParams)
    {
    }
  }
}
