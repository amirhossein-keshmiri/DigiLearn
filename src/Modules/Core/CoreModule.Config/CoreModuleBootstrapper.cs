using CoreModule.Application.Categories;
using CoreModule.Application.Categories.Create;
using CoreModule.Application.Courses;
using CoreModule.Application.Teachers;
using CoreModule.Domain.Categories.DomainServices;
using CoreModule.Domain.Courses.DomainServices;
using CoreModule.Domain.Teachers.DomainServices;
using CoreModule.Facade;
using CoreModule.Infrastructure;
using CoreModule.Query;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreModule.Config
{
  public static class CoreModuleBootstrapper
  {
    public static IServiceCollection InitCoreModule(this IServiceCollection services, IConfiguration configuration)
    {
      CoreModuleFacadeBootstrapper.RegisterDependency(services);
      CoreModuleInfrastructureBootstrapper.RegisterDependency(services, configuration);
      CoreModuleQueryBootstrapper.RegisterDependency(services, configuration);

      services.AddMediatR(typeof(CreateCategoryCommand).Assembly);
      services.AddValidatorsFromAssembly(typeof(CreateCategoryCommand).Assembly);

      services.AddScoped<ICourseDomainService, CourseDomainService>();
      services.AddScoped<ITeacherDomainService, TeacherDomainService>();
      services.AddScoped<ICategoryDomainService, CategoryDomainService>();

      return services;
    }
  }
}
