using Common.Application;
using Common.Application.SecurityUtil;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Users.ChangePassword
{
  public class ChangeUserPasswordCommandHandler : IBaseCommandHandler<ChangeUserPasswordCommand>
  {
    private readonly UserContext _userContext;

    public ChangeUserPasswordCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
      var user = await _userContext.Users.FirstOrDefaultAsync(f => f.Id == request.UserId);
      if (user == null)
      {
        return OperationResult.NotFound();
      }

      if (Sha256Hasher.IsCompare(user.Password, request.CurrentPassword))
      {
        var hashedPassword = Sha256Hasher.Hash(request.NewPassword);
        user.Password = hashedPassword;
        _userContext.Update(user);
        await _userContext.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
      }
      return OperationResult.Error("Current Password Is Not Valid");
    }
  }
}
