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
  public class UsersController : ApiController
  {
    private readonly IUserFacade _userFacade;
    private readonly IConfiguration _configuration;
    public UsersController(IUserFacade userFacade, IConfiguration configuration)
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

      var isComparedPassword = Sha256Hasher.IsCompare(user.Password, loginRequest.PhoneNumber);
      if (isComparedPassword == false)
      {
        var result = OperationResult<LoginResponse?>.Error("PassWord is Wrong!");
        return CommandResult(result);
      }

      var token = JwtTokenBuilder.BuildToken(user, _configuration);

      var loginResult = await AddTokenAndGenerateJwt(user);
      return CommandResult(loginResult);

      ////return CommandResult<LoginResponse>(
      ////    OperationResult<LoginResponse>.Success
      ////    (new LoginResponse { Token = token }));
    }

    [HttpPost("RefreshToken")]
    public async Task<ApiResult<LoginResponse?>> RefreshToken(string refreshToken)
    {
      var result = await _userFacade.GetUserTokenByRefreshToken(refreshToken);

      if (result == null)
        return CommandResult(OperationResult<LoginResponse?>.NotFound());

      if (result.TokenExpireDate > DateTime.Now)
      {
        return CommandResult(OperationResult<LoginResponse>.Error("توکن هنوز منقضی نشده است"));
      }

      if (result.RefreshTokenExpireDate < DateTime.Now)
      {
        return CommandResult(OperationResult<LoginResponse>.Error("زمان رفرش توکن به پایان رسیده است"));
      }
      var user = await _userFacade.GetUserById(result.UserId);
      await _userFacade.RemoveToken(new RemoveUserTokenCommand(result.UserId, result.Id));
      var loginResult = await AddTokenAndGenerateJwt(user);
      return CommandResult(loginResult);
    }

    [Authorize]
    [HttpDelete("logout")]
    public async Task<ApiResult> Logout()
    {
      var token = await HttpContext.GetTokenAsync("access_token");
      var result = await _userFacade.GetUserTokenByJwtToken(token);
      if (result == null)
        return CommandResult(OperationResult.NotFound());

      await _userFacade.RemoveToken(new RemoveUserTokenCommand(result.UserId, result.Id));
      return CommandResult(OperationResult.Success());
    }

    private async Task<OperationResult<LoginResponse?>> AddTokenAndGenerateJwt(UserDto user)
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
        return OperationResult<LoginResponse?>.Error();

      return OperationResult<LoginResponse?>.Success(new LoginResponse()
      {
        Token = token,
        RefreshToken = refreshToken
      });
    }
  }
}
