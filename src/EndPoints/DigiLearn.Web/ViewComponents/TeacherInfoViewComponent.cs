using DigiLearn.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Services;

namespace DigiLearn.Web.ViewComponents
{
  public class TeacherInfoViewComponent : ViewComponent
  {
    private readonly IUserFacade _userFacade;

    public TeacherInfoViewComponent(IUserFacade userFacade)
    {
      _userFacade = userFacade;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
      var claimsPrincipal = HttpContext.User;

      if (claimsPrincipal == null)
      {
        return View(null);
      }

      var phoneNumber = claimsPrincipal.GetPhoneNumber();

      if (string.IsNullOrWhiteSpace(phoneNumber))
      {
        return View(null);
      }

      var userDto = await _userFacade.GetUserByPhoneNumber(phoneNumber);

      if (userDto == null)
      {
        return View(null);
      }

      return View(userDto);
    }
  }
}
