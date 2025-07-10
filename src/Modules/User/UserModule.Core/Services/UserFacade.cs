using Common.Application;
using Common.Application.SecurityUtil;
using MediatR;
using UserModule.Core.Commands.Users.AddToken;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Commands.Users.RemoveToken;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Queries.Users.GetById;
using UserModule.Core.Queries.Users.GetByPhoneNumber;
using UserModule.Core.Queries.UserTokens.GetByJwtToken;
using UserModule.Core.Queries.UserTokens.GetByRefreshToken;

namespace UserModule.Core.Services
{
  public class UserFacade : IUserFacade
  {
    private readonly IMediator _mediator;
    public UserFacade(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task<OperationResult<Guid>> RegisterUser(RegisterUserCommand command)
    {
      return await _mediator.Send(command);
    }
    public async Task<OperationResult> AddToken(AddUserTokenCommand command)
    {
      return await _mediator.Send(command);
    }
    public async Task<OperationResult> RemoveToken(RemoveUserTokenCommand command)
    {
      var result = await _mediator.Send(command);

      if (result.Status != OperationResultStatus.Success)
        return OperationResult.Error();

      return OperationResult.Success();
    }

    public async Task<UserDto?> GetUserByPhoneNumber(string phoneNumber)
    {
      return await _mediator.Send(new GetUserByPhoneNumberQuery(phoneNumber));
    }
    public async Task<UserDto?> GetUserById(Guid userId)
    {
      return await _mediator.Send(new GetUserByIdQuery(userId));
    }
    public async Task<UserTokenDto?> GetUserTokenByRefreshToken(string refreshToken)
    {
      var hashRefreshToken = Sha256Hasher.Hash(refreshToken);
      return await _mediator.Send(new GetUserTokenByRefreshTokenQuery(hashRefreshToken));
    }
    public async Task<UserTokenDto?> GetUserTokenByJwtToken(string jwtToken)
    {
      var hashJwtToken = Sha256Hasher.Hash(jwtToken);
      return await _mediator.Send(new GetUserTokenByJwtTokenQuery(hashJwtToken));
    }
  }
}
