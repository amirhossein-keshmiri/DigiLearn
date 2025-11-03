namespace UserModule.Core.Models.Responses
{
  public class LoginResponse
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int TokenExpiryMinutes { get; set; }
  }
}
