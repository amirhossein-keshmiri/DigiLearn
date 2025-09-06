using Common.Application;
using CoreModule.Domain.Teachers.Repositories;
using MediatR;

namespace CoreModule.Application.Teachers.RejectRequest
{
  public class RejectTeacherRequestCommandHandler : IBaseCommandHandler<RejectTeacherRequestCommand>
  {
    private readonly ITeacherRepository _repository;
    private readonly IMediator _mediator;
    public RejectTeacherRequestCommandHandler(ITeacherRepository repository, IMediator mediator)
    {
      _repository = repository;
      _mediator = mediator;
    }

    public async Task<OperationResult> Handle(RejectTeacherRequestCommand request, CancellationToken cancellationToken)
    {
      var teacher = await _repository.GetTracking(request.TeacherId);
      if (teacher == null)
        return OperationResult.NotFound();

      _repository.Delete(teacher);
      await _repository.Save();

      //Send Event

      return OperationResult.Success();
    }
  }
}
