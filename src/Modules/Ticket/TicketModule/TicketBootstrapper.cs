using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketModule.Core.Services;
using TicketModule.Data.Context;

namespace TicketModule
{
  public static class TicketBootstrapper
  {
    public static IServiceCollection InitTicketModule(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<TicketContext>((context) =>
      {
        context.UseSqlServer(configuration.GetConnectionString("Ticket_Context"));
      });

      services.AddScoped<ITicketService, TicketService>();
      services.AddAutoMapper(typeof(MapperProfile).Assembly);
      return services;
    }
  }
}