using CoreModule.Application.Courses.Sections.AddSection;
using CoreModule.Facade.Courses;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

//===============================================================
//  THIS DOESNOT USE, I USE MODAL INSTEAD INSIDE SECTION INDEX
//===============================================================

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

    [Display(Name = "Title")]
    [Required(ErrorMessage = "Please Insert {0}")]
    public string Title { get; set; }

    [Display(Name = "Display Order")]
    [Required(ErrorMessage = "Please Insert {0}")]
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
