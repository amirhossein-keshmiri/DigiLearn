using Common.Application;
using UserModule.Core.Commands.Users.AddToken;
using UserModule.Core.Commands.Users.ChangePassword;
using UserModule.Core.Commands.Users.EditProfile;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Commands.Users.RemoveToken;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Services
{
  public interface IUserFacade
  {
    Task<OperationResult<Guid>> RegisterUser(RegisterUserCommand command);
    Task<OperationResult> AddToken(AddUserTokenCommand command);
    Task<OperationResult> RemoveToken(RemoveUserTokenCommand command);
    Task<OperationResult> EditUserProfile(EditUserProfileCommand command);
    Task<OperationResult> ChangePassword(ChangeUserPasswordCommand command);

    Task<UserDto?> GetUserByPhoneNumber(string phoneNumber);
    Task<UserDto?> GetUserById(Guid userId);
    Task<UserTokenDto?> GetUserTokenByRefreshToken(string refreshToken);
    Task<UserTokenDto?> GetUserTokenByJwtToken(string jwtToken);
  }
}
