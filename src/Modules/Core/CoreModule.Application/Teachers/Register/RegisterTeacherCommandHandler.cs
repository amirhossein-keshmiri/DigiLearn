using Common.Application;
using Common.Application.FileUtil.Interfaces;
using CoreModule.Application._Utilities;
using CoreModule.Domain.Teachers.DomainServices;
using CoreModule.Domain.Teachers.Repositories;

namespace CoreModule.Application.Teachers.Register
{
  public class RegisterTeacherCommandHandler : IBaseCommandHandler<RegisterTeacherCommand>
  {
    private readonly ITeacherRepository _repository;
    private readonly ITeacherDomainService _domainService;
    private readonly ILocalFileService _localFileService;


    public RegisterTeacherCommandHandler(ITeacherRepository repository, ITeacherDomainService domainService, ILocalFileService localFileService)
    {
      _repository = repository;
      _domainService = domainService;
      _localFileService = localFileService;
    }

    public async Task<OperationResult> Handle(RegisterTeacherCommand request, CancellationToken cancellationToken)
    {
      var cvFileName = await _localFileService.SaveFileAndGenerateName(request.CvFile, CoreModuleDirectories.CvFileNames);

      var teacher = new Domain.Teachers.Models.Teacher(request.UserId, request.UserName, cvFileName, _domainService);
      _repository.Add(teacher);
      await _repository.Save();
      return OperationResult.Success();

    }
  }
}
