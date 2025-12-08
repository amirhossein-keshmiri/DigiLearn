using CoreModule.Facade.Courses;
using CoreModule.Query._DTOs;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;

namespace DigiLearn.Web.Areas.Admin.Pages.Courses.Sections
{
  public class IndexModel : BaseRazor
  {
    private ICourseFacade _courseFacade;
    public IndexModel(ICourseFacade courseFacade)
    {
      _courseFacade = courseFacade;
    }

    public CourseDto Course { get; set; }

    public async Task<IActionResult> OnGet(Guid courseId)
    {
      var course = await _courseFacade.GetCourseById(courseId);
      if (course == null)
      {
        return RedirectToPage("../Index");
      }

      Course = course;
      return Page();
    }
  }
}
