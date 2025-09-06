using Common.Infrastructure.Repository;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Infrastructure.Persistent.Courses
{
  public class CourseRepository : BaseRepository<Domain.Courses.Models.Course, CoreModuleEfContext>, ICourseRepository
  {
    public CourseRepository(CoreModuleEfContext context) : base(context)
    {
    }
  }
}
