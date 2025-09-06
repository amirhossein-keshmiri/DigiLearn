using Common.Application;
using CoreModule.Domain.Categories.DomainServices;
using CoreModule.Domain.Categories.Repositories;

namespace CoreModule.Application.Categories.Edit
{
  public class EditCategoryCommandHandler : IBaseCommandHandler<EditCategoryCommand>
  {
    private readonly ICourseCategoryRepository _courseCategoryRepository;
    private readonly ICategoryDomainService _categoryDomainService;

    public EditCategoryCommandHandler(ICourseCategoryRepository courseCategoryRepository, ICategoryDomainService categoryDomainService)
    {
      _courseCategoryRepository = courseCategoryRepository;
      _categoryDomainService = categoryDomainService;
    }

    public async Task<OperationResult> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
      var category = await _courseCategoryRepository.GetTracking(request.Id);
      if (category == null)
      {
        return OperationResult.NotFound();
      }
      category.Edit(request.Title, request.Slug, _categoryDomainService);
      await _courseCategoryRepository.Save();
      return OperationResult.Success();
    }
  }
}
