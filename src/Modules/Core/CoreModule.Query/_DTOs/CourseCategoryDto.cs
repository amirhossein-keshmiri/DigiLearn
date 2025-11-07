using Common.Query;

namespace CoreModule.Query._DTOs
{
  public class CourseCategoryDto : BaseDto
  {
    public string Title { get; set; }
    public string Slug { get; set; }
    public Guid? ParentId { get; set; }
    public List<CourseCategoryChildDto> Children { get; set; }
  }

  public class CourseCategoryChildDto : BaseDto
  {
    public string Title { get; set; }
    public string Slug { get; set; }
    public Guid? ParentId { get; set; }
  }
}
