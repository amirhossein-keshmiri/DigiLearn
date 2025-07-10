using Common.Application;
using Common.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;
using UserModule.Data.Entities.Users;

namespace UserModule.Core.Commands.Users.AddToken
{
  public class AddUserTokenCommandHandler : IBaseCommandHandler<AddUserTokenCommand>
  {
    private readonly UserContext _userContext;

    public AddUserTokenCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(AddUserTokenCommand request, CancellationToken cancellationToken)
    {
      var user = _userContext.Set<User>().AsTracking().FirstOrDefault(t => t.Id.Equals(request.UserId));
      if (user == null)
        return OperationResult.NotFound();

      var activeTokenCount = _userContext.UserTokens.Where(x => x.UserId == request.UserId).Count(c => c.RefreshTokenExpireDate > DateTime.Now);
      if (activeTokenCount == 3)
        return OperationResult.Error("امکان استفاده از 4 دستگاه همزمان وجود ندارد");

      AddToken(user.Id, request.HashJwtToken, request.HashRefreshToken, request.TokenExpireDate, request.RefreshTokenExpireDate, request.Device);

      await _userContext.SaveChangesAsync();

      return OperationResult.Success();
    }

    private void AddToken(Guid userId, string hashJwtToken, string hashRefreshToken, DateTime tokenExpireDate,
        DateTime refreshTokenExpireDate, string device)
    {
      var token = new UserToken(hashJwtToken, hashRefreshToken, tokenExpireDate, refreshTokenExpireDate, device);
      token.UserId = userId;
      _userContext.Add(token);
    }
  }
}
