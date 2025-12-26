using CoreModule.Application.Courses.Episodes.AcceptEpisode;
using CoreModule.Application.Courses.Episodes.DeleteEpisode;
using CoreModule.Application.Courses.Sections.DeleteSection;
using CoreModule.Application.Courses.Sections.EditSection;
using CoreModule.Facade.Courses;
using CoreModule.Query._DTOs;
using DigiLearn.Web.DTOs.Teacher.Courses;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;

namespace DigiLearn.Web.Areas.Admin.Pages.Courses.Sections
{
    public class IndexModel : BaseRazor
    {
        private readonly ICourseFacade _courseFacade;
        public IndexModel(ICourseFacade courseFacade)
        {
            _courseFacade = courseFacade;
        }

        public CourseDto Course { get; set; }

        [BindProperty]
        public EditSectionRequest EditSectionRequest { get; set; }

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

        public async Task<IActionResult> OnPostAccept(Guid courseId, Guid episodeId)
        {
            return await AjaxTryCatch(
                () => _courseFacade.AcceptEpisode(new AcceptCourseEpisodeCommand(courseId, episodeId)));
        }
        public async Task<IActionResult> OnPostDelete(Guid courseId, Guid episodeId)
        {
            return await AjaxTryCatch(
                () => _courseFacade.DeleteEpisode(new DeleteCourseEpisodeCommand(courseId, episodeId)));
        }

        public async Task<IActionResult> OnPostDeleteSection(Guid courseId, Guid sectionId)
        {
            return await AjaxTryCatch(
                () => _courseFacade.DeleteSection(new DeleteCourseSectionCommand(courseId, sectionId)));
        }

        public async Task<IActionResult> OnPostEditSection(Guid courseId, Guid sectionId)
        {
            return await AjaxTryCatch(
                async () =>
                {
                    return await _courseFacade.EditSection(new EditCourseSectionCommand()
                    {
                        Id = sectionId,
                        CourseId = courseId,
                        Title = EditSectionRequest.Title,
                        DisplayOrder = EditSectionRequest.DisplayOrder,
                    });
                });
        }
    }
}
