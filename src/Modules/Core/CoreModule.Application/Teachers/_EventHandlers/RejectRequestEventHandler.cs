using CoreModule.Domain.Teachers.Events;
using MediatR;

namespace CoreModule.Application.Teachers._EventHandlers
{
  class RejectRequestEventHandler : INotificationHandler<RejectTeacherRequestEvent>
  {
    public async Task Handle(RejectTeacherRequestEvent notification, CancellationToken cancellationToken)
    {
      await Task.CompletedTask;
    }
  }
}
