using Common.Infrastructure.Repository;
using CoreModule.Domain.Teachers.Repositories;

namespace CoreModule.Infrastructure.Persistent.Teachers
{
  public class TeacherRepository : BaseRepository<Domain.Teachers.Models.Teacher, CoreModuleEfContext>, ITeacherRepository
  {
    public TeacherRepository(CoreModuleEfContext context) : base(context)
    {
    }

    public void Delete(Domain.Teachers.Models.Teacher teacher)
    {
      Context.Remove(teacher);
    }
  }
}
