using Common.Query;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Queries.Users.GetByPhoneNumber
{
  public record GetUserByPhoneNumberQuery(string PhoneNumber) : IQuery<UserDto?>;
}
