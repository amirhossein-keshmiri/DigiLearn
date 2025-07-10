using Common.Query;

namespace UserModule.Core.Queries._DTOs
{
  public class UserTokenDto : BaseDto
  {
    public Guid UserId { get; set; }
    public string HashJwtToken { get; set; }
    public string HashRefreshToken { get; set; }
    public DateTime TokenExpireDate { get; set; }
    public DateTime RefreshTokenExpireDate { get; set; }
    public string Device { get; set; }
  }
}
