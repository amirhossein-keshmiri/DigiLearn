using Common.Application;
using Microsoft.AspNetCore.Http;

namespace CoreModule.Application.Courses.Episodes.EditEpisode
{
  public class EditEpisodeCommand : IBaseCommand
  {
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public IFormFile? VideoFile { get; set; }
    public IFormFile? AttachmentFile { get; set; }
    public bool IsActive { get; set; }

  }
}
