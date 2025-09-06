using Common.Domain;
using Common.Domain.Exceptions;

namespace CoreModule.Domain.Courses.Models
{
  public class Section : BaseEntity
  {
    public Section(Guid courseId, string title, int displayOrder)
    {
      NullOrEmptyDomainDataException.CheckString(title, nameof(title));

      CourseId = courseId;
      Title = title;
      DisplayOrder = displayOrder;

      Episodes = new List<Episode>();
    }

    public Guid CourseId { get; private set; }
    public string Title { get; private set; }
    public int DisplayOrder { get; private set; }

    public List<Episode> Episodes { get; private set; }

    public void Edit(int displayOrder, string title)
    {
      NullOrEmptyDomainDataException.CheckString(title, nameof(title));
      DisplayOrder = displayOrder;
      Title = title;
    }

    public Episode AddEpisode(string title, string englishTitle, Guid token, TimeSpan timeSpan, string videoName, string? attachmentName, bool isActive)
    {
      var episode = new Episode(Id, title, englishTitle, token, timeSpan, videoName, attachmentName, isActive);
      Episodes.Add(episode);
      return episode;
    }
  }
}
