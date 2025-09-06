using Common.Application;
using Common.Domain.ValueObjects;
using CoreModule.Domain.Courses.Enums;
using Microsoft.AspNetCore.Http;

namespace CoreModule.Application.Courses.Create
{
  public class CreateCourseCommand : IBaseCommand
  {
    public Guid TeacherId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid SubCategoryId { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public IFormFile ImageFile { get; set; }
    public IFormFile? VideoFile { get; set; }
    public int Price { get; set; }
    public SeoData SeoData { get; set; }
    public CourseLevel CourseLevel { get; set; }
  }
}
