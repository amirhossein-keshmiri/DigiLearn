using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.ChangePassword;
using UserModule.Core.Models.Requests;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Profile.Settings
{
  [Authorize]
  [BindProperties]
  public class ChangePassword : BaseRazor
  {
    private readonly IUserFacade _userFacade;

    public ChangePassword(IUserFacade userFacade)
    {
      _userFacade = userFacade;
    }

    public ChangePasswordRequest ChangePasswordRequest { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
      var result = await _userFacade.ChangePassword(new ChangeUserPasswordCommand()
      {
        CurrentPassword = ChangePasswordRequest.CurrentPassword,
        NewPassword = ChangePasswordRequest.NewPassword,
        UserId = User.GetUserId()
      });
      return RedirectAndShowAlert(result, RedirectToPage("Index"));
    }
  }
}
