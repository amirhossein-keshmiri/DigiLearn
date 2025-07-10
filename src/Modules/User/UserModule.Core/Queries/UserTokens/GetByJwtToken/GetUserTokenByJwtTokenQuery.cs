using Common.Query;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Queries.UserTokens.GetByJwtToken
{
  public record GetUserTokenByJwtTokenQuery(string HashJwtToken) : IQuery<UserTokenDto?>;
}
