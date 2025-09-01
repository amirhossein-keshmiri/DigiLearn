using AutoMapper;
using Common.Application;
using UserModule.Data.Entities;
using UserModule.Data.Entities.Notifications;

namespace UserModule.Core.Commands.Notifications.Create
{
  public class CreateNotificationCommandHandler : IBaseCommandHandler<CreateNotificationCommand>
  {
    private readonly UserContext _userContext;
    private readonly IMapper _mapper;

    public CreateNotificationCommandHandler(UserContext userContext, IMapper mapper)
    {
      _userContext = userContext;
      _mapper = mapper;
    }

    public async Task<OperationResult> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
      var model = _mapper.Map<UserNotification>(request);

      _userContext.UserNotifications.Add(model);
      await _userContext.SaveChangesAsync(cancellationToken);

      return OperationResult.Success();
    }
  }
}
