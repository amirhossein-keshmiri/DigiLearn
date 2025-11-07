using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Categories.GetChildren
{
  public record GetCourseCategoryChildrenQuery(Guid ParentId) : IQuery<List<CourseCategoryDto>>;

}
