using Common.Application;
using Common.Application.SecurityUtil;
using DigiLearn.Api.Infrastructure;
using DigiLearn.Api.Infrastructure.JwtUtil;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Models.Requests;
using UserModule.Core.Models.Responses;
using UserModule.Core.Services;

namespace DigiLearn.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
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

      return CommandResult<LoginResponse>(
          OperationResult<LoginResponse>.Success
          (new LoginResponse { Token = token }));
    }
  }
}
