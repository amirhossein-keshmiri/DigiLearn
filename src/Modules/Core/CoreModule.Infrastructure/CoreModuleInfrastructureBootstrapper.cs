using CoreModule.Domain.Categories.Repositories;
using CoreModule.Domain.Courses.Repositories;
using CoreModule.Domain.Teachers.Repositories;
using CoreModule.Infrastructure.EventHandlers;
using CoreModule.Infrastructure.Persistent;
using CoreModule.Infrastructure.Persistent.Categories;
using CoreModule.Infrastructure.Persistent.Courses;
using CoreModule.Infrastructure.Persistent.Teachers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreModule.Infrastructure
{
  public class CoreModuleInfrastructureBootstrapper
  {
    public static void RegisterDependency(IServiceCollection services, IConfiguration configuration)
    {
      services.AddScoped<ICourseRepository, CourseRepository>();
      services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
      services.AddScoped<ITeacherRepository, TeacherRepository>();

      services.AddHostedService<UserRegisteredEventHandler>();

      services.AddDbContext<CoreModuleEfContext>(option =>
      {
        option.UseSqlServer(configuration.GetConnectionString("CoreModule_Context"));
      });
    }
  }
}
