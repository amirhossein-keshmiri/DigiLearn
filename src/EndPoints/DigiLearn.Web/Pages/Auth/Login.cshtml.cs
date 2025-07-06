using Common.Application.SecurityUtil;
using DigiLearn.Web.Infrastructure.JwtUtil;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Models.Requests;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Auth
{
  [BindProperties]
  public class LoginModel : BaseRazor
  {
    private readonly IUserFacade _userFacade;
    private IConfiguration _configuration { get; set; }
    public LoginModel(IUserFacade userFacade, IConfiguration configuration)
    {
      _userFacade = userFacade;
      _configuration = configuration;
    }

    public LoginRequest LoginRequest { get; set; }
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
      var user = await _userFacade.GetUserByPhoneNumber(LoginRequest.PhoneNumber);
      if (user == null)
      {
        ErrorAlert("User Not Found!");
        return Page();
      }

      var isComparedPassword = Sha256Hasher.IsCompare(user.Password, LoginRequest.PhoneNumber);
      if (isComparedPassword == false)
      {
        ErrorAlert("PassWord is Wrong!");
        return Page();
      }

      var token = JwtTokenBuilder.BuildToken(user, _configuration);
      if (LoginRequest.IsRememberMe)
      {
        HttpContext.Response.Cookies.Append("digi-token", token, new CookieOptions()
        {
          HttpOnly = true,
          Expires = DateTime.UtcNow.AddMinutes(60),
          Secure = true,
        });
      }
      else
      {
        HttpContext.Response.Cookies.Append("digi-token", token, new CookieOptions()
        {
          HttpOnly = true,
          Secure = true,
        });
      }

      return RedirectToPage("../Index");
    }
  }
}
