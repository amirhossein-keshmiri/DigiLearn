using Common.Application;
using Microsoft.AspNetCore.Http;

namespace CoreModule.Application.Courses.Episodes.AddEpisode
{
  public class AddCourseEpisodeCommand : IBaseCommand
  {
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string EnglishTitle { get; set; }
    public Guid SectionId { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public IFormFile VideoFile { get; set; }
    public IFormFile? AttachmentFile { get; set; }
    public bool IsActive { get; set; }
  }
}
