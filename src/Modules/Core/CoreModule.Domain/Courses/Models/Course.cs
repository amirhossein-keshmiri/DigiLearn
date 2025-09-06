using Common.Domain;
using Common.Domain.Exceptions;
using Common.Domain.ValueObjects;
using CoreModule.Domain.Courses.DomainServices;
using CoreModule.Domain.Courses.Enums;

namespace CoreModule.Domain.Courses.Models
{
  public class Course : AggregateRoot
  {
    public Course(Guid teacherId, Guid categoryId, Guid subCategoryId, string title, string slug, string description, string imageName, string? videoName, int price,
                  SeoData seoData, CourseLevel courseLevel, ICourseDomainService domainService)
    {
      Guard(title, description, imageName, slug);

      if (domainService.SlugIsExist(slug))
        throw new InvalidDomainDataException("Slug is Exist");

      TeacherId = teacherId;
      CategoryId = categoryId;
      SubCategoryId = subCategoryId;
      Title = title;
      Slug = slug;
      Description = description;
      ImageName = imageName;
      VideoName = videoName;
      Price = price;
      LastUpdate = DateTime.Now;
      SeoData = seoData;
      CourseLevel = courseLevel;
      CourseStatus = CourseStatus.StartSoon;

      Sections = new();
    }

    public Guid TeacherId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid SubCategoryId { get; private set; }
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string Description { get; private set; }
    public string ImageName { get; private set; }
    public string? VideoName { get; private set; }
    public int Price { get; private set; }
    public DateTime LastUpdate { get; private set; }
    public SeoData SeoData { get; private set; }

    public CourseLevel CourseLevel { get; private set; }
    public CourseStatus CourseStatus { get; private set; }

    public List<Section> Sections { get; private set; }

    public void AddSection(int displayOrder, string title)
    {
      if (Sections.Any(f => f.Title == title))
        throw new InvalidDomainDataException("title Is Exist");

      Sections.Add(new Section(Id, title, displayOrder));
    }
    public void EditSection(Guid sectionId, int displayOrder, string title)
    {
      var section = Sections.FirstOrDefault(f => f.Id == sectionId);
      if (section == null) throw new InvalidDomainDataException("Section NotFound");

      section.Edit(displayOrder, title);
    }
    public void RemoveSection(Guid sectionId)
    {
      var section = Sections.FirstOrDefault(f => f.Id == sectionId);
      if (section == null) throw new InvalidDomainDataException("Section NotFound");

      Sections.Remove(section);
    }

    public Episode AddEpisode(Guid sectionId, string title, string englishTitle, Guid token, TimeSpan timeSpan, string videoExtension, string? attachmentExtension, bool isActive)
    {
      var section = Sections.FirstOrDefault(f => f.Id == sectionId);
      if (section == null) throw new InvalidDomainDataException("Section NotFound");

      var episodeCount = Sections.Sum(s => s.Episodes.Count());
      var episodeTitle = $"{episodeCount + 1}_{englishTitle}";

      string attName = null;

      if (string.IsNullOrWhiteSpace(attachmentExtension) == false)
        attName = $"{episodeTitle}.{attachmentExtension}";
      var vidName = $"{episodeTitle}.{videoExtension}";

      if (isActive)
      {
        LastUpdate = DateTime.Now;
        if (CourseStatus == CourseStatus.StartSoon)
        {
          CourseStatus = CourseStatus.InProgress;
        }
      }

      return section.AddEpisode(title, englishTitle, token, timeSpan, vidName, attName, isActive);
    }
    public void AcceptEpisode(Guid episodeId)
    {
      var section = Sections.FirstOrDefault(f => f.Episodes.Any(e => e.Id == episodeId && e.IsActive == false));
      if (section == null)
        throw new InvalidDomainDataException();

      var episode = section.Episodes.First(f => f.Id == episodeId);

      episode.ToggleStatus();
      LastUpdate = DateTime.Now;
    }

    void Guard(string title, string description, string imageName, string slug)
    {
      NullOrEmptyDomainDataException.CheckString(title, nameof(title));
      NullOrEmptyDomainDataException.CheckString(slug, nameof(slug));
      NullOrEmptyDomainDataException.CheckString(description, nameof(description));
      NullOrEmptyDomainDataException.CheckString(imageName, nameof(imageName));
    }
  }
}
