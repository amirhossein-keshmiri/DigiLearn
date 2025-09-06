namespace CoreModule.Domain.Courses.DomainServices
{
  public interface ICourseDomainService
  {
    bool SlugIsExist(string slug);
  }
}
