using Common.Application;
using Common.EventBus.Abstractions;
using Common.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Users.EditProfile
{
  public class EditUserProfileCommandHandler : IBaseCommandHandler<EditUserProfileCommand>
  {
    private readonly UserContext _userContext;
    private readonly IEventBus _eventBus;

    public EditUserProfileCommandHandler(UserContext userContext, IEventBus eventBus)
    {
      _userContext = userContext;
      _eventBus = eventBus;
    }

    public async Task<OperationResult> Handle(EditUserProfileCommand request, CancellationToken cancellationToken)
    {
      var user = await _userContext.Users.FirstOrDefaultAsync(f => f.Id == request.UserId, cancellationToken);
      if (user == null)
      {
        return OperationResult.NotFound();
      }

      user.Name = request.Name;
      user.Family = request.Family;
      if (string.IsNullOrWhiteSpace(request.Email) == false)
      {
        user.Email = request.Email;
      }
      _userContext.Users.Update(user);
      await _userContext.SaveChangesAsync(cancellationToken);

      _eventBus.Publish(new UserEdited()
      {
        UserId = user.Id,
        Name = user.Name,
        Family = user.Family,
        PhoneNumber = user.PhoneNumber,
        Email = user.Email,
      }, null, Exchanges.UserTopicExchange, ExchangeType.Topic, "user.edited");

      return OperationResult.Success();
    }
  }
}
