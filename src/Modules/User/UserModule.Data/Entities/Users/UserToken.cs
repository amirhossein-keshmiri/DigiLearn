using Common.Domain;
using Common.Domain.Exceptions;

namespace UserModule.Data.Entities.Users
{
  public class UserToken : BaseEntity
  {
    public UserToken()
    {
      
    }
    public UserToken(string hashJwtToken, string hashRefreshToken, DateTime tokenExpireDate,
        DateTime refreshTokenExpireDate, string device)
    {
      Guard(hashJwtToken, hashRefreshToken, tokenExpireDate, refreshTokenExpireDate, device);
      HashJwtToken = hashJwtToken;
      HashRefreshToken = hashRefreshToken;
      TokenExpireDate = tokenExpireDate;
      RefreshTokenExpireDate = refreshTokenExpireDate;
      Device = device;
    }
    public Guid UserId { get; set; }
    public string HashJwtToken { get; private set; }
    public string HashRefreshToken { get; private set; }
    public DateTime TokenExpireDate { get; private set; }
    public DateTime RefreshTokenExpireDate { get; private set; }
    public string Device { get; private set; }
    public void Guard(string hashJwtToken, string hashRefreshToken, DateTime tokenExpireDate,
        DateTime refreshTokenExpireDate, string device)
    {
      NullOrEmptyDomainDataException.CheckString(hashJwtToken, nameof(HashJwtToken));
      NullOrEmptyDomainDataException.CheckString(hashRefreshToken, nameof(HashRefreshToken));

      if (tokenExpireDate < DateTime.Now)
        throw new InvalidDomainDataException("Invalid Token ExpireDate");

      //if (refreshTokenExpireDate < tokenExpireDate)
      //  throw new InvalidDomainDataException("Invalid RefreshToken ExpireDate");
    }
  }
}
