using Common.Domain.Repository;
using CoreModule.Domain.Categories.Models;

namespace CoreModule.Domain.Categories.Repositories
{
  public interface ICourseCategoryRepository : IBaseRepository<CourseCategory>
  {
    Task Delete(CourseCategory category);
  }
}
