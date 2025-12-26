using Common.Application;

namespace CoreModule.Application.Courses.Sections.EditSection
{
  public class EditCourseSectionCommand : IBaseCommand
  {
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public int DisplayOrder { get; set; }
  }
}
