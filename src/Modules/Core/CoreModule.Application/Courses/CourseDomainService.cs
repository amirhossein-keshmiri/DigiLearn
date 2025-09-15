using Common.Domain.Utils;
using CoreModule.Domain.Courses.DomainServices;
using CoreModule.Domain.Courses.Repositories;

namespace CoreModule.Application.Courses
{
  public class CourseDomainService : ICourseDomainService
  {
    private readonly ICourseRepository _repository;

    public CourseDomainService(ICourseRepository repository)
    {
      _repository = repository;
    }

    public bool SlugIsExist(string slug)
    {
      return _repository.Exists(f => f.Slug == slug.ToSlug());
    }
  }
}
