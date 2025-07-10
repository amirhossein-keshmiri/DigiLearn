using Common.Application;

namespace UserModule.Core.Commands.Users.RemoveToken
{
  public record RemoveUserTokenCommand(Guid UserId, Guid TokenId) : IBaseCommand<string>;
}
