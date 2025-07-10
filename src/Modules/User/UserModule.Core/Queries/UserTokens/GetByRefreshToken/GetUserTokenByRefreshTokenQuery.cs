using Common.Query;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Queries.UserTokens.GetByRefreshToken
{
  public record GetUserTokenByRefreshTokenQuery(string HashRefreshToken) : IQuery<UserTokenDto?>;
}
