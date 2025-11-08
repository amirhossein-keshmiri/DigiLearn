using CoreModule.Application.Courses.Sections.AddSection;
using CoreModule.Domain.Courses.Models;
using CoreModule.Facade.Courses;
using CoreModule.Facade.Teachers;
using CoreModule.Query._DTOs;
using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Pages.Profile.Teacher.Courses.Sections
{
  [ServiceFilter(typeof(TeacherActionFilter))]
  public class IndexModel : BaseRazor
  {
    private readonly ICourseFacade _courseFacade;
    private readonly ITeacherFacade _teacherFacade;
    public IndexModel(ICourseFacade courseFacade, ITeacherFacade teacherFacade)
    {
      _courseFacade = courseFacade;
      _teacherFacade = teacherFacade;
    }

    public CourseDto Course { get; set; }

    [BindProperty]
    [Display(Name = "Title")]
    [Required(ErrorMessage = "Please Insert {0}")]
    public string Title { get; set; }

    [BindProperty]
    [Display(Name = "Display Order")]
    [Required(ErrorMessage = "Please Insert {0}")]
    public int DisplayOrder { get; set; }

    public async Task<IActionResult> OnGet(Guid courseId)
    {
      var course = await _courseFacade.GetCourseById(courseId);
      var teacher = await _teacherFacade.GetByUserId(User.GetUserId());

      if (course == null || course.TeacherId != teacher!.Id)
      {
        return RedirectToPage("../Index");
      }
      Course = course;
      return Page();
    }

    public async Task<IActionResult> OnPostAddSection(Guid courseId)
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
