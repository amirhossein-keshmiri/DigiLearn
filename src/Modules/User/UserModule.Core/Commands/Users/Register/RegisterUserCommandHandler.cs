using Common.Application;
using Common.Application.SecurityUtil;
using Common.Domain.Utils;
using Common.EventBus.Abstractions;
using Common.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using UserModule.Data.Entities;
using UserModule.Data.Entities.Users;

namespace UserModule.Core.Commands.Users.Register
{
  public class RegisterUserCommandHandler : IBaseCommandHandler<RegisterUserCommand, Guid>
  {
    private readonly UserContext _userContext;
    private readonly IEventBus _eventBus;
    public RegisterUserCommandHandler(UserContext userContext, IEventBus eventBus)
    {
      _userContext = userContext;
      _eventBus = eventBus;
    }
    public async Task<OperationResult<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
      if (await _userContext.Users.AnyAsync(f => f.PhoneNumber == request.PhoneNumber))
      {
        return OperationResult<Guid>.Error("شماره تلفن تکراری است.");
      }

      var user = new User()
      {
        Id = Guid.NewGuid(),
        PhoneNumber = request.PhoneNumber,
        Password = Sha256Hasher.Hash(request.Password),
        Avatar = "default.png"
      };

      _userContext.Add(user);
      await _userContext.SaveChangesAsync(cancellationToken);

      _eventBus.Publish(new UserRegistered()
      {
        Id = user.Id,
        Name = user.Name,
        Family = user.Family,
        PhoneNumber = user.PhoneNumber,
        Password = user.Password,
        Email = user.Email,
        Avatar = user.Avatar,
      }, null, Exchanges.UserTopicExchange, ExchangeType.Topic, "user.registered"); // if using ExchangeType.Direct we need to fill the queue name

      return OperationResult<Guid>.Success(user.Id);
    }
  }
}
