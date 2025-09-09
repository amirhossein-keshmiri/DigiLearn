using Common.Application;
using CoreModule.Application.Categories.AddChild;
using CoreModule.Application.Categories.Create;
using CoreModule.Application.Categories.Delete;
using CoreModule.Application.Categories.Edit;
using MediatR;

namespace CoreModule.Facade.Categories
{
  public interface ICourseCategoryFacade
  {
    Task<OperationResult> Create(CreateCategoryCommand command);
    Task<OperationResult> Edit(EditCategoryCommand command);
    Task<OperationResult> Delete(Guid id);
    Task<OperationResult> AddChild(AddChildCategoryCommand command);
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
  }
}
