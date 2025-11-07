using CoreModule.Application.Courses.Sections.AddSection;
using CoreModule.Facade.Courses;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Pages.Profile.Teacher.Courses.Sections
{
  [BindProperties]
  public class AddModel : BaseRazor
  {
    private ICourseFacade _courseFacade;

    public AddModel(ICourseFacade courseFacade)
    {
      _courseFacade = courseFacade;
    }

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public string Title { get; set; }

    [Display(Name = "ترتیب نمایش")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public int DisplayOrder { get; set; }
    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPost(Guid courseId)
    {
      var result = await _courseFacade.AddSection(new AddCourseSectionCommand()
      {
        Title = Title,
        DisplayOrder = DisplayOrder,
        CourseId = courseId,
      });

      return RedirectAndShowAlert(result, RedirectToPage("Index", new { courseId }));
    }

  }
}
