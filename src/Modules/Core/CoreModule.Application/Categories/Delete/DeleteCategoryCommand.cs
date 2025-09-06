using Common.Application;

namespace CoreModule.Application.Categories.Delete
{
  public record DeleteCategoryCommand(Guid CategoryId) : IBaseCommand;
}
