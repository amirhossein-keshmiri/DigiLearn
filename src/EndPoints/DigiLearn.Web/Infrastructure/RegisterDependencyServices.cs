using DigiLearn.Web.Infrastructure.RazorUtils;

namespace DigiLearn.Web.Infrastructure;

public static class RegisterDependencyServices
{
    public static IServiceCollection RegisterWebDependencies(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<IRenderViewToString, RenderViewToString>();
        services.AddAutoMapper(typeof(RegisterDependencyServices).Assembly);

        return services;
    }
}