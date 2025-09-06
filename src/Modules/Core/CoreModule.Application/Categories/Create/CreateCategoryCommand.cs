using Common.Application;

namespace CoreModule.Application.Categories.Create
{
  public class CreateCategoryCommand : IBaseCommand
  {
    public string Title { get; set; }
    public string Slug { get; set; }
  }
}
