using CoreModule.Facade.Teachers;
using CoreModule.Query._DTOs;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;

namespace DigiLearn.Web.Areas.Admin.Pages.Teachers
{
  public class IndexModel : BaseRazor
  {
    private readonly ITeacherFacade _teacherFacade;

    public IndexModel(ITeacherFacade teacherFacade)
    {
      _teacherFacade = teacherFacade;
    }

    public List<TeacherDto> Teachers { get; set; }
    public async Task OnGet()
    {
      Teachers = await _teacherFacade.GetList();
    }

    public async Task<IActionResult> OnPostToggleStatus(Guid id)
    {
      return await AjaxTryCatch(() => _teacherFacade.ToggleStatus(id));
    }
  }
}
