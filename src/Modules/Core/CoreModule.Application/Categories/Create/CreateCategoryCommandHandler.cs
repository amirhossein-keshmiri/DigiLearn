using Common.Application;
using CoreModule.Domain.Categories.DomainServices;
using CoreModule.Domain.Categories.Models;
using CoreModule.Domain.Categories.Repositories;

namespace CoreModule.Application.Categories.Create
{
  public class CreateCategoryCommandHandler : IBaseCommandHandler<CreateCategoryCommand>
  {
    private readonly ICourseCategoryRepository _courseCategoryRepository;
    private readonly ICategoryDomainService _categoryDomainService;

    public CreateCategoryCommandHandler(ICourseCategoryRepository courseCategoryRepository, ICategoryDomainService categoryDomainService)
    {
      _courseCategoryRepository = courseCategoryRepository;
      _categoryDomainService = categoryDomainService;
    }

    public async Task<OperationResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
      var category = new CourseCategory(request.Title, request.Slug, null, _categoryDomainService);

      _courseCategoryRepository.Add(category);
      await _courseCategoryRepository.Save();
      return OperationResult.Success();
    }
  }
}
