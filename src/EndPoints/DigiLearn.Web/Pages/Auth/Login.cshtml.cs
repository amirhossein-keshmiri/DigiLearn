using AngleSharp.Browser;
using Common.Application;
using Common.Application.SecurityUtil;
using DigiLearn.Web.Infrastructure.JwtUtil;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.AddToken;
using UserModule.Core.Models.Requests;
using UserModule.Core.Models.Responses;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;
using UAParser;

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

      var isComparedPassword = Sha256Hasher.IsCompare(user.Password, LoginRequest.Password);
      if (isComparedPassword == false)
      {
        ErrorAlert("PassWord is Wrong!");
        return Page();
      }

      var loginResult = await AddTokenAndGenerateJwt(user);
      if (loginResult.Status != OperationResultStatus.Success)
      {
        ErrorAlert(loginResult.Message);
        return Page();
      }

      if (LoginRequest.IsRememberMe)
      {
        HttpContext.Response.Cookies.Append("digi-token", loginResult.Data.Token, new CookieOptions()
        {
          HttpOnly = true,
          Expires = DateTime.UtcNow.AddMinutes(60),
          Secure = true,
        });
        HttpContext.Response.Cookies.Append("refresh-token", loginResult.Data.RefreshToken, new CookieOptions()
        {
          HttpOnly = true,
          Expires = DateTimeOffset.Now.AddMinutes(65)
        });
      }
      else
      {
        HttpContext.Response.Cookies.Append("digi-token", loginResult.Data.Token, new CookieOptions()
        {
          HttpOnly = true,
          Secure = true,
        });
        HttpContext.Response.Cookies.Append("refresh-token", loginResult.Data.RefreshToken, new CookieOptions()
        {
          HttpOnly = true,
        });
      }

      return RedirectToPage("../Index");
    }

    private async Task<OperationResult<LoginResponse>> AddTokenAndGenerateJwt(UserDto user)
    {
      var uaParser = Parser.GetDefault();
      var header = HttpContext.Request.Headers["user-agent"].ToString();
      var device = "windows";
      if (header != null)
      {
        var info = uaParser.Parse(header);
        device = $"{info.Device.Family}/{info.OS.Family} {info.OS.Major}.{info.OS.Minor} - {info.UA.Family}";
      }

      var token = JwtTokenBuilder.BuildToken(user, _configuration);
      var refreshToken = Guid.NewGuid().ToString();

      var hashJwt = Sha256Hasher.Hash(token);
      var hashRefreshToken = Sha256Hasher.Hash(refreshToken);

      var tokenResult = await _userFacade.AddToken(new AddUserTokenCommand(user.Id, hashJwt, hashRefreshToken, DateTime.Now.AddMinutes(60), DateTime.Now.AddMinutes(65), device));
      if (tokenResult.Status != OperationResultStatus.Success)
        return OperationResult<LoginResponse>.Error(tokenResult.Message);

      return OperationResult<LoginResponse>.Success(new LoginResponse()
      {
        Token = token,
        RefreshToken = refreshToken
      });
    }
  }
}
