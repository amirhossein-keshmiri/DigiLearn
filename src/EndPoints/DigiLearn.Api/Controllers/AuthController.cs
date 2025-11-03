using Common.Application;
using Common.Application.SecurityUtil;
using DigiLearn.Api.Infrastructure;
using DigiLearn.Api.Infrastructure.JwtUtil;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.AddToken;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Models.Requests;
using UserModule.Core.Models.Responses;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;
using UAParser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using UserModule.Core.Commands.Users.RemoveToken;

namespace DigiLearn.Api.Controllers
{
  public class AuthController : ApiController
  {
    private readonly IUserFacade _userFacade;
    private readonly IConfiguration _configuration;
    public AuthController(IUserFacade userFacade, IConfiguration configuration)
    {
      _userFacade = userFacade;
      _configuration = configuration;
    }

    [HttpPost("RegisterUser")]
    public async Task<ApiResult<Guid>> Register([FromBody] RegisterRequest registerRequest)
    {
      var command = new RegisterUserCommand()
      {
        PhoneNumber = registerRequest.PhoneNumber,
        Password = registerRequest.Password,
      };

      var result = await _userFacade.RegisterUser(command);
      return CommandResult<Guid>(result);
    }

    [HttpPost("LoginUser")]
    public async Task<ApiResult<LoginResponse?>> LoginUser([FromBody] LoginRequest loginRequest)
    {
      var user = await _userFacade.GetUserByPhoneNumber(loginRequest.PhoneNumber);
      if (user == null)
      {
        var result = OperationResult<LoginResponse?>.Error("User Not Found!");
        return CommandResult(result);
      }

      var isComparedPassword = Sha256Hasher.IsCompare(user.Password, loginRequest.Password);
      if (isComparedPassword == false)
      {
        var result = OperationResult<LoginResponse?>.Error("PassWord is Wrong!");
        return CommandResult(result);
      }

      var loginResult = await AddTokenAndGenerateJwt(user, loginRequest.IsRememberMe);
      if (loginResult.Status != OperationResultStatus.Success)
        return CommandResult(loginResult);

      var loginResponse = loginResult.Data!;

      SetAuthCookies(loginResponse, loginRequest.IsRememberMe);

      return CommandResult(OperationResult<LoginResponse?>.Success(new LoginResponse
      {
        Token = null,
        RefreshToken = null
      }));
    }

    //[Authorize]
    //[HttpGet("Profile")]
    //public IActionResult GetProfile()
    //{
    //  var userId = User.FindFirst("userId")?.Value;
    //  return Ok(new { userId, message = "Authorized access" });
    //}

    [HttpGet("check")]
    public IActionResult CheckAuth()
    {
      if (User.Identity?.IsAuthenticated == true)
      {
        Response.Headers.Add("Cache-Control", "private, max-age=5");
        return Ok(new { isAuthenticated = true });
      }

      return Unauthorized(new { isAuthenticated = false });
    }

    [HttpPost("RefreshToken")]
    public async Task<ApiResult<LoginResponse?>> RefreshToken()
    {
      var refreshToken = Request.Cookies["refresh-token"];

      if (string.IsNullOrEmpty(refreshToken))
        return CommandResult(OperationResult<LoginResponse?>.Error("Refresh token not found."));

      var result = await _userFacade.GetUserTokenByRefreshToken(refreshToken);
      if (result == null)
        return CommandResult(OperationResult<LoginResponse?>.NotFound());

      if (result.TokenExpireDate > DateTime.Now)
        return CommandResult(OperationResult<LoginResponse>.Error("Access token not yet expired."));

      if (result.RefreshTokenExpireDate < DateTime.Now)
        return CommandResult(OperationResult<LoginResponse>.Error("Refresh token expired."));

      var user = await _userFacade.GetUserById(result.UserId);
      await _userFacade.RemoveToken(new RemoveUserTokenCommand(result.UserId, result.Id));

      var newTokens = await AddTokenAndGenerateJwt(user, true);
      if (newTokens.Status != OperationResultStatus.Success)
        return CommandResult(OperationResult<LoginResponse?>.Error("Unable to generate new tokens."));

      // ✅ Re-set new cookies (same as login)
      SetAuthCookies(newTokens.Data!, true);

      return CommandResult(OperationResult<LoginResponse?>.Success(new LoginResponse
      {
        Token = null,
        RefreshToken = null,
        TokenExpiryMinutes = newTokens.Data!.TokenExpiryMinutes,
      }));
    }

    [Authorize]
    [HttpDelete("logout")]
    public async Task<ApiResult> Logout()
    {
      var token = await HttpContext.GetTokenAsync("access_token");
      var result = await _userFacade.GetUserTokenByJwtToken(token);
      if (result == null)
        return CommandResult(OperationResult.NotFound());

      var removeUserToken = await _userFacade.RemoveToken(new RemoveUserTokenCommand(result.UserId, result.Id));
      if (removeUserToken.Status == OperationResultStatus.Success)
      {
        HttpContext.Response.Cookies.Delete("digi-token");
        HttpContext.Response.Cookies.Delete("refresh-token");
      }
      return CommandResult(OperationResult.Success());
    }

    private async Task<OperationResult<LoginResponse?>> AddTokenAndGenerateJwt(UserDto user, bool isRememberMe = false)
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
      //var refreshToken = Guid.NewGuid().ToString();
      var refreshToken = Guid.NewGuid().ToString();

      var refreshTokenExpiry = isRememberMe
                  ? DateTime.Now.AddDays(int.Parse(_configuration["JwtConfig:RefreshTokenExpiryDays"]))
                  : DateTime.Now.AddMinutes(int.Parse(_configuration["JwtConfig:AccessTokenExpiryMinutes"])); // Short if not rememberMe

      var hashJwt = Sha256Hasher.Hash(token);
      var hashRefreshToken = Sha256Hasher.Hash(refreshToken);

      var tokenExpiry = DateTime.Now.AddMinutes(int.Parse(_configuration["JwtConfig:AccessTokenExpiryMinutes"]));

      var tokenResult = await _userFacade.AddToken(new AddUserTokenCommand(
          user.Id,
          hashJwt,
          hashRefreshToken,
          tokenExpiry,
          refreshTokenExpiry,
          device));

      if (tokenResult.Status != OperationResultStatus.Success)
        return OperationResult<LoginResponse?>.Error();

      return OperationResult<LoginResponse?>.Success(new LoginResponse()
      {
        Token = token,
        RefreshToken = refreshToken,
        TokenExpiryMinutes = (int)(tokenExpiry - DateTime.Now).TotalMinutes

      });
    }

    private void SetAuthCookies(LoginResponse loginResponse, bool isRememberMe = false)
    {
      var accessOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true, // Enforce HTTPS
        SameSite = SameSiteMode.None,
        IsEssential = true
      };

      // Always set expiry for access cookie to match token lifetime (short)
      accessOptions.Expires = DateTime.Now.AddMinutes(int.Parse(_configuration["JwtConfig:AccessTokenExpiryMinutes"])); // Or parse from config
      HttpContext.Response.Cookies.Append("digi-token", loginResponse.Token, accessOptions);

      var refreshOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        IsEssential = true
      };

      // Conditional expiry for refresh
      if (isRememberMe)
      {
        refreshOptions.Expires = DateTime.Now.AddDays(int.Parse(_configuration["JwtConfig:RefreshTokenExpiryDays"])); // Persistent
      }
      // Else: Session cookie (no Expires, deletes on browser close)

      HttpContext.Response.Cookies.Append("refresh-token", loginResponse.RefreshToken, refreshOptions);
    }

  }
}
