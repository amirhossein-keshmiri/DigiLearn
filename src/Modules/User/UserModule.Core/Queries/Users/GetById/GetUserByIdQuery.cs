using Common.Query;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Queries.Users.GetById
{
  public class GetUserByIdQuery : IQuery<UserDto?>
  {
    public GetUserByIdQuery(Guid userId)
    {
      UserId = userId;
    }

    public Guid UserId { get; private set; }
  }
}
