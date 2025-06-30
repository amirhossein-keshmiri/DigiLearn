using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Services;
using UserModule.Data.Entities;

namespace UserModule.Core
{
  public static class UserModuleBootstrapper
  {
    public static IServiceCollection InitUserModule(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<UserContext>((context) =>
      {
        context.UseSqlServer(configuration.GetConnectionString("User_Context"));
      });

      services.AddMediatR(typeof(UserModuleBootstrapper).Assembly);
      services.AddScoped<IUserFacade, UserFacade>();
      services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

      return services;
    }
  }
}