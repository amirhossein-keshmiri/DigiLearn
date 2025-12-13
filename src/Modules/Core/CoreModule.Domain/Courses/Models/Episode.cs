using Common.Domain;
using Common.Domain.Exceptions;
using Common.Domain.Utils;

namespace CoreModule.Domain.Courses.Models
{
  public class Episode : BaseEntity
  {
    public Episode(Guid sectionId, string title, string englishTitle, Guid token, TimeSpan timeSpan, string videoName, string? attachmentName, bool isActive)
    {
      Guard(videoName, englishTitle, title);

      SectionId = sectionId;
      Title = title;
      EnglishTitle = englishTitle;
      Token = token;
      TimeSpan = timeSpan;
      VideoName = videoName;
      AttachmentName = attachmentName;
      IsActive = isActive;
    }

    public Guid SectionId { get; private set; }
    public string Title { get; private set; }
    public string EnglishTitle { get; private set; }
    public Guid Token { get; private set; }
    public TimeSpan TimeSpan { get; private set; }
    public string VideoName { get; private set; }
    public string? AttachmentName { get; private set; }
    public bool IsActive { get; private set; }

    public void ToggleStatus()
    {
      IsActive = !IsActive;
    }

    public void Edit(string title, bool isActive, TimeSpan timeSpan, string? attachmentName)
    {
      NullOrEmptyDomainDataException.CheckString(title, nameof(title));
      Title = title;
      IsActive = isActive;
      TimeSpan = timeSpan;
      if (string.IsNullOrWhiteSpace(attachmentName) == false)
      {
        AttachmentName = attachmentName;
      }
    }

    void Guard(string videoName, string englishTitle, string title)
    {
      NullOrEmptyDomainDataException.CheckString(videoName, nameof(videoName));
      NullOrEmptyDomainDataException.CheckString(englishTitle, nameof(englishTitle));
      NullOrEmptyDomainDataException.CheckString(title, nameof(title));
      if (englishTitle.IsUniCode())
      {
        throw new InvalidDomainDataException("Invalid EnglishTitle");
      }
    }
  }
}
