using Common.Application;
using CoreModule.Domain.Teachers.Repositories;

namespace CoreModule.Application.Teachers.ToggleStatus
{
    class ToggleTeacherStatusCommandHandler : IBaseCommandHandler<ToggleTeacherStatusCommand>
  {
    private readonly ITeacherRepository _repository;

    public ToggleTeacherStatusCommandHandler(ITeacherRepository repository)
    {
      _repository = repository;
    }

    public async Task<OperationResult> Handle(ToggleTeacherStatusCommand request, CancellationToken cancellationToken)
    {
      var teacher = await _repository.GetTracking(request.TeacherId);
      if (teacher == null)
      {
        return OperationResult.NotFound();
      }

      teacher.ToggleStatus();
      await _repository.Save();
      return OperationResult.Success();
    }
  }
}
