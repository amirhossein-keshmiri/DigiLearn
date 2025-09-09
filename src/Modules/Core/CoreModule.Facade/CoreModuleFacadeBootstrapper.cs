using CoreModule.Facade.Categories;
using CoreModule.Facade.Courses;
using CoreModule.Facade.Teachers;
using Microsoft.Extensions.DependencyInjection;

namespace CoreModule.Facade
{
  public class CoreModuleFacadeBootstrapper
  {
    public static void RegisterDependency(IServiceCollection services)
    {
      services.AddScoped<ITeacherFacade, TeacherFacade>();
      services.AddScoped<ICourseFacade, CourseFacade>();
      services.AddScoped<ICourseCategoryFacade, CourseCategoryFacade>();
    }
  }
}
