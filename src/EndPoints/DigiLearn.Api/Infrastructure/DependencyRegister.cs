using DigiLearn.Api.Infrastructure.JwtUtil;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Shop.Api.Infrastructure;
public static class DependencyRegister
{
    public static void RegisterApiDependency(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddTransient<CustomJwtValidation>();
    }
}

