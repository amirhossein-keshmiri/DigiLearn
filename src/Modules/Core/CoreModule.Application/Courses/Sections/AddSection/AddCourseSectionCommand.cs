using Common.Application;

namespace CoreModule.Application.Courses.Sections.AddSection
{
  public class AddCourseSectionCommand : IBaseCommand
  {
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public int DisplayOrder { get; set; }
  }
}
