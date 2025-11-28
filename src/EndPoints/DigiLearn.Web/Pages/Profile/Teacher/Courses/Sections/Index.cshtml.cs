using CoreModule.Application.Courses.Episodes.AddEpisode;
using CoreModule.Application.Courses.Sections.AddSection;
using CoreModule.Domain.Courses.Models;
using CoreModule.Facade.Courses;
using CoreModule.Facade.Teachers;
using CoreModule.Query._DTOs;
using DigiLearn.Web.DTOs.Teacher.Courses;
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
    public AddSectionRequest AddSectionRequest { get; set; }

    [BindProperty]
    public AddEpisodeRequest AddEpisodeRequest { get; set; }

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
        Title = AddSectionRequest.Title,
        DisplayOrder = AddSectionRequest.DisplayOrder,
        CourseId = courseId,
      });

      return RedirectAndShowAlert(result, RedirectToPage("Index", new { courseId }));
    }

    public async Task<IActionResult> OnPostAddEpisode(Guid courseId, Guid sectionId)
    {
      var result = await _courseFacade.AddEpisode(new AddCourseEpisodeCommand()
      {
        Title = AddEpisodeRequest.Title,
        AttachmentFile = AddEpisodeRequest.AttachmentFile,
        VideoFile = AddEpisodeRequest.VideoFile,
        IsActive = false,
        CourseId = courseId,
        EnglishTitle = AddEpisodeRequest.EnglishTitle,
        TimeSpan = AddEpisodeRequest.TimeSpan,
        SectionId = sectionId,
      });

      return RedirectAndShowAlert(result, RedirectToPage("Index", new { courseId }));
    }
  }
}
