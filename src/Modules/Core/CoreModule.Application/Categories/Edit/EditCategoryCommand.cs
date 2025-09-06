using Common.Application;

namespace CoreModule.Application.Categories.Edit
{
  public class EditCategoryCommand : IBaseCommand
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
  }
}
