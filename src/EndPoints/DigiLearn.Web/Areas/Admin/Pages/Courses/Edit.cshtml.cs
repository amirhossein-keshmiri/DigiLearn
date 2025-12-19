using Common.Domain.ValueObjects;
using CoreModule.Application._Utilities;
using CoreModule.Application.Courses.Edit;
using CoreModule.Domain.Courses.Enums;
using CoreModule.Facade.Courses;
using DigiLearn.Web.Infrastructure.RazorUtils;
using DigiLearn.Web.Infrastructure.Utils.CustomValidation.IFormFile;
using DigiLearn.Web.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Areas.Admin.Pages.Courses
{
  public class EditModel : BaseRazor
  {
    private readonly ICourseFacade _courseFacade;

    public EditModel(ICourseFacade courseFacade)
    {
      _courseFacade = courseFacade;
    }

    public Guid CourseId { get; set; }

    [BindProperty]
    [Display(Name = "دسته بندی اصلی")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public Guid CategoryId { get; set; }

    [BindProperty]
    [Display(Name = "زیر دسته بندی")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public Guid SubCategoryId { get; set; }

    [BindProperty]
    [Display(Name = "Title")]
    [Required(ErrorMessage = "Please enter {0}")]
    public string Title { get; set; }

    [BindProperty]
    [Display(Name = "Slug")]
    [Required(ErrorMessage = "Please enter {0}")]
    public string Slug { get; set; }

    [BindProperty]
    [Display(Name = "Description")]
    [Required(ErrorMessage = "Please enter {0}")]
    [UIHint("Ckeditor4")]
    public string Description { get; set; }

    [BindProperty]
    [Display(Name = "Image")]
    [FileImage(ErrorMessage = "Image is invalid.")]
    public IFormFile? ImageFile { get; set; }

    [BindProperty]
    [Display(Name = "Intro Video")]
    [FileType("mp4", ErrorMessage = "Intro video is invalid.")]
    public IFormFile? VideoFile { get; set; }

    [BindProperty]
    [Display(Name = "Price")]
    public int Price { get; set; }

    [BindProperty]
    public SeoDataViewModel SeoData { get; set; }

    [BindProperty]
    [Display(Name = "Course Level")]
    public CourseLevel CourseLevel { get; set; }

    [BindProperty]
    [Display(Name = "Course Status")]
    public CourseStatus CourseStatus { get; set; }

    [BindProperty]
    [Display(Name = "Action Status")]
    public CourseActionStatus ActionStatus { get; set; }

    [BindProperty]
    public string ExistingImageName { get; set; }

    [BindProperty]
    public string? ExistingVideoName { get; set; }
    public string ExistingImageUrl { get; set; }
    public string ExistingVideoUrl { get; set; }

    public async Task<IActionResult> OnGet(Guid courseId)
    {
      var course = await _courseFacade.GetCourseById(courseId);

      if (course == null)
        return RedirectToPage("Index");

      CourseId = courseId;
      CategoryId = course.CategoryId;
      SubCategoryId = course.SubCategoryId;
      Title = course.Title;
      Slug = course.Slug;
      Description = course.Description;
      SeoData = SeoDataViewModel.ConvertToViewModel(course.SeoData);
      CourseLevel = course.CourseLevel;
      CourseStatus = course.CourseStatus;
      ActionStatus = course.Status;
      Price = course.Price;

      ExistingImageName = course.ImageName;
      ExistingVideoName = course.VideoName;
      ExistingImageUrl = CoreModuleDirectories.GetCourseImage(course.ImageName);

      if (!string.IsNullOrEmpty(course.VideoName))
      {
        ExistingVideoUrl = $"/core/course/{courseId}/{course.VideoName}";
      }

      return Page();
    }

    public async Task<IActionResult> OnPost(Guid courseId)
    {
      if (!ModelState.IsValid)
      {
        var courseData = await _courseFacade.GetCourseById(courseId);
        if (courseData != null)
        {
          CourseId = courseId;
          ExistingImageName = courseData.ImageName;
          ExistingVideoName = courseData.VideoName;
          ExistingImageUrl = CoreModuleDirectories.GetCourseImage(courseData.ImageName);
          if (!string.IsNullOrEmpty(courseData.VideoName))
          {
            ExistingVideoUrl = $"/core/course/{courseId}/{courseData.VideoName}";
          }
        }
        return Page();
      }

      var result = await _courseFacade.Edit(new EditCourseCommand
      {
        CourseId = courseId,
        CategoryId = CategoryId,
        SubCategoryId = SubCategoryId,
        Title = Title,
        Slug = Slug,
        Description = Description,
        ImageFile = ImageFile,
        VideoFile = VideoFile,
        Price = Price,
        SeoData = SeoData.Map(),
        CourseLevel = CourseLevel,
        CourseStatus = CourseStatus,
        CourseActionStatus = ActionStatus
      });
      return RedirectAndShowAlert(result, RedirectToPage("Index"));
    }
  }
}
