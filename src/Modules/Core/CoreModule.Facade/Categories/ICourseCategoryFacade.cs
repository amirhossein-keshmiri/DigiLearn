using Common.Application;
using CoreModule.Application.Categories.AddChild;
using CoreModule.Application.Categories.Create;
using CoreModule.Application.Categories.Delete;
using CoreModule.Application.Categories.Edit;
using CoreModule.Query._DTOs;
using CoreModule.Query.Categories.GetAll;
using CoreModule.Query.Categories.GetById;
using CoreModule.Query.Categories.GetChildren;
using MediatR;

namespace CoreModule.Facade.Categories
{
  public interface ICourseCategoryFacade
  {
    Task<OperationResult> Create(CreateCategoryCommand command);
    Task<OperationResult> Edit(EditCategoryCommand command);
    Task<OperationResult> Delete(Guid id);
    Task<OperationResult> AddChild(AddChildCategoryCommand command);

    Task<List<CourseCategoryDto>> GetMainCategories();
    Task<CourseCategoryDto?> GetById(Guid categoryId);
    Task<List<CourseCategoryDto>> GetChildren(Guid parentId);
  }

  class CourseCategoryFacade : ICourseCategoryFacade
  {
    private readonly IMediator _mediator;

    public CourseCategoryFacade(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task<OperationResult> Create(CreateCategoryCommand command)
    {
      return await _mediator.Send(command);

    }

    public async Task<OperationResult> Edit(EditCategoryCommand command)
    {
      return await _mediator.Send(command);
    }

    public async Task<OperationResult> Delete(Guid id)
    {
      return await _mediator.Send(new DeleteCategoryCommand(id));
    }

    public async Task<OperationResult> AddChild(AddChildCategoryCommand command)
    {
      return await _mediator.Send(command);
    }

    public async Task<List<CourseCategoryDto>> GetMainCategories()
    {
      return await _mediator.Send(new GetAllCourseCategoriesQuery());

    }

    public async Task<CourseCategoryDto?> GetById(Guid categoryId)
    {
      return await _mediator.Send(new GetCourseCategoryByIdQuery(categoryId));
    }

    public async Task<List<CourseCategoryDto>> GetChildren(Guid parentId)
    {
      return await _mediator.Send(new GetCourseCategoryChildrenQuery(parentId));
    }
  }
}
