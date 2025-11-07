using Common.Query;
using CoreModule.Query._Context;
using CoreModule.Query._DTOs;
using Microsoft.EntityFrameworkCore;

namespace CoreModule.Query.Categories.GetChildren
{
  class GetCourseCategoryChildrenQueryHandler : IQueryHandler<GetCourseCategoryChildrenQuery, List<CourseCategoryDto>>
  {
    private readonly QueryContext _context;

    public GetCourseCategoryChildrenQueryHandler(QueryContext context)
    {
      _context = context;
    }

    public async Task<List<CourseCategoryDto>> Handle(GetCourseCategoryChildrenQuery request, CancellationToken cancellationToken)
    {
      return await _context.CourseCategories
          .Where(r => r.ParentId == request.ParentId)
          .OrderByDescending(d => d.CreationDate)
          .Select(s => new CourseCategoryDto
          {
            Id = s.Id,
            CreationDate = s.CreationDate,
            Title = s.Title,
            Slug = s.Slug,
            ParentId = s.ParentId
          }).ToListAsync(cancellationToken);
    }
  }
}
