using CoreModule.Domain.Teachers.DomainServices;
using CoreModule.Domain.Teachers.Repositories;

namespace CoreModule.Application.Teachers
{
  public class TeacherDomainService : ITeacherDomainService
  {
    private readonly ITeacherRepository _repository;

    public TeacherDomainService(ITeacherRepository repository)
    {
      _repository = repository;
    }

    public bool UserNameIsExist(string userName)
    {
      return _repository.Exists(f => f.UserName == userName.ToLower());
    }
  }
}
