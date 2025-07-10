using Common.Application;
using Common.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;
using UserModule.Data.Entities.Users;

namespace UserModule.Core.Commands.Users.RemoveToken
{
  public class RemoveUserTokenCommandHandler : IBaseCommandHandler<RemoveUserTokenCommand, string>
  {
    private readonly UserContext _userContext;

    public RemoveUserTokenCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult<string>> Handle(RemoveUserTokenCommand request, CancellationToken cancellationToken)
    {
      var user = await _userContext.Set<User>().AsTracking().FirstOrDefaultAsync(t => t.Id.Equals(request.UserId));
      if (user == null)
        return OperationResult<string>.NotFound();

      var token = RemoveToken(request.TokenId);

      await _userContext.SaveChangesAsync();
      return OperationResult<string>.Success(token);
    }
    private string RemoveToken(Guid tokenId)
    {
      var token = _userContext.UserTokens.FirstOrDefault(f => f.Id == tokenId);
      if (token == null)
        throw new InvalidDomainDataException("invalid TokenId");

      _userContext.Remove(token);
      return token.HashJwtToken;
    }
  }
}
