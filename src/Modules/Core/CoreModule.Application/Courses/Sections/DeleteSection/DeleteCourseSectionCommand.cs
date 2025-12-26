using Common.Application;

namespace CoreModule.Application.Courses.Sections.DeleteSection
{
  public record DeleteCourseSectionCommand(Guid CourseId, Guid SectionId) : IBaseCommand;
}
