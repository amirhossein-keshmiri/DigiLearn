using DigiLearn.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.ChangePassword;
using UserModule.Core.Commands.Users.EditProfile;
using UserModule.Core.Models.Requests;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;

namespace DigiLearn.Api.Controllers
{
  [Authorize]
  public class UsersController : ApiController
  {
    private readonly IUserFacade _userFacade;

    public UsersController(IUserFacade userFacade)
    {
      _userFacade = userFacade;
    }

    [HttpGet("CurrentUserProfile")]
    public async Task<ApiResult<UserDto>> GetUserByPhoneNumber()
    {
      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
      return QueryResult(user);
    }

    [HttpPut("UserProfile")]
    public async Task<ApiResult> EditUserProfile([FromBody] EditUserProfileRequest editUserProfileRequest)
    {
      var command = new EditUserProfileCommand()
      {
        UserId = User.GetUserId(),
        Name = editUserProfileRequest.Name,
        Family = editUserProfileRequest.Family,
        Email = editUserProfileRequest.Email,
      };

      var result = await _userFacade.EditUserProfile(command);
      return CommandResult(result);
    }

    [HttpPut("ChangePassword")]
    public async Task<ApiResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePasswordRequest)
    {
      var command = new ChangeUserPasswordCommand()
      {
        UserId = User.GetUserId(),
        CurrentPassword = changePasswordRequest.CurrentPassword,
        NewPassword = changePasswordRequest.NewPassword,
      };

      //You can also use mapper for ChangeUserPassword

      var result = await _userFacade.ChangePassword(command);
      return CommandResult(result);
    }
  }
}
