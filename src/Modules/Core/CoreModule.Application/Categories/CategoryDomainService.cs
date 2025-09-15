using Common.Domain.Utils;
using CoreModule.Domain.Categories.DomainServices;
using CoreModule.Domain.Categories.Repositories;

namespace CoreModule.Application.Categories
{
  public class CategoryDomainService : ICategoryDomainService
  {
    private readonly ICourseCategoryRepository _repository;

    public CategoryDomainService(ICourseCategoryRepository repository)
    {
      _repository = repository;
    }

    public bool SlugIsExist(string slug)
    {
      return _repository.Exists(f => f.Slug == slug.ToSlug());
    }
  }
}
