using Common.Application;
using Common.Application.SecurityUtil;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;
using UserModule.Data.Entities.Users;

namespace UserModule.Core.Commands.Users.Register
{
  public class RegisterUserCommandHandler : IBaseCommandHandler<RegisterUserCommand, Guid>
  {
    private readonly UserContext _userContext;
    public RegisterUserCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
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

      return OperationResult<Guid>.Success(user.Id);
    }
  }
}
