using Common.Query;
using CoreModule.Query._DTOs;

namespace CoreModule.Query.Categories.GetById
{
  public record GetCourseCategoryByIdQuery(Guid CategoryId) : IQuery<CourseCategoryDto?>;

}
