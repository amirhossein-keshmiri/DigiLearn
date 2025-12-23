using CoreModule.Domain.Teachers.Events;
using MediatR;

namespace CoreModule.Application.Teachers._EventHandlers
{
  class AcceptRequestEventHandler : INotificationHandler<AcceptTeacherRequestEvent>
  {
    public async Task Handle(AcceptTeacherRequestEvent notification, CancellationToken cancellationToken)
    {
      await Task.CompletedTask;
    }
  }
}
