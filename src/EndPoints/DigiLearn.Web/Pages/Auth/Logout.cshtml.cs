using Common.Application;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.RemoveToken;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Auth
{
  public class LogoutModel : BaseRazor
  {
    private readonly IUserFacade _userFacade;

    public LogoutModel(IUserFacade userFacade)
    {
      _userFacade = userFacade;
    }

    public async Task<IActionResult> OnGet()
    {
      var token = await HttpContext.GetTokenAsync("access_token");
      if (token == null)
      {
        //return NotFound();
        ErrorAlert("Please Login First!");
        return RedirectToPage("../Index");
      }
      var result = await _userFacade.GetUserTokenByJwtToken(token);
      if (result == null)
        return Page();

      var removeUserToken = await _userFacade.RemoveToken(new RemoveUserTokenCommand(result.UserId, result.Id));
      if (removeUserToken.Status == OperationResultStatus.Success)
      {
        HttpContext.Response.Cookies.Delete("digi-token");
        HttpContext.Response.Cookies.Delete("refresh-token");
      }

      return RedirectAndShowAlert(removeUserToken, RedirectToPage("../Index"));
    }
  }
}
