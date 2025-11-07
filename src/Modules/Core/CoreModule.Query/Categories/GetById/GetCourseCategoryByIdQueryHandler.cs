using Common.Query;
using CoreModule.Query._Context;
using CoreModule.Query._DTOs;
using Microsoft.EntityFrameworkCore;

namespace CoreModule.Query.Categories.GetById
{
  class GetCourseCategoryByIdQueryHandler : IQueryHandler<GetCourseCategoryByIdQuery, CourseCategoryDto?>
  {
    private readonly QueryContext _context;

    public GetCourseCategoryByIdQueryHandler(QueryContext context)
    {
      _context = context;
    }

    public async Task<CourseCategoryDto?> Handle(GetCourseCategoryByIdQuery request, CancellationToken cancellationToken)
    {
      var category = await _context.CourseCategories
          .Include(c => c.Children)
          .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken: cancellationToken);

      if (category == null)
      {
        return null;
      }
      return new CourseCategoryDto
      {
        Id = category.Id,
        CreationDate = category.CreationDate,
        Title = category.Title,
        Slug = category.Slug,
        ParentId = category.ParentId,
        Children = category.Children.Select(r => new CourseCategoryChildDto()
        {
          CreationDate = r.CreationDate,
          Id = r.Id,
          ParentId = r.ParentId,
          Slug = r.Slug,
          Title = r.Title
        }).ToList()
      };
    }
  }
}
