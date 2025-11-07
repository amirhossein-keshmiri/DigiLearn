namespace DigiLearn.Api.Infrastructure.Middlewares
{
  public class JwtCookieMiddleware
  {
    private readonly RequestDelegate _next;
    public JwtCookieMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
      // Copy JWT from cookie to Authorization header
      if (!context.Request.Headers.ContainsKey("Authorization"))
      {
        var token = context.Request.Cookies["digi-token"];
        if (!string.IsNullOrEmpty(token))
          context.Request.Headers.Add("Authorization", $"Bearer {token}");
      }

      await _next(context);
    }
  }
}
