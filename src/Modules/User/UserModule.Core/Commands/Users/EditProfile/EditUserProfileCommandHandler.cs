using Common.Application;
using Microsoft.EntityFrameworkCore;
using UserModule.Data.Entities;

namespace UserModule.Core.Commands.Users.EditProfile
{
  public class EditUserProfileCommandHandler : IBaseCommandHandler<EditUserProfileCommand>
  {
    private readonly UserContext _userContext;

    public EditUserProfileCommandHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<OperationResult> Handle(EditUserProfileCommand request, CancellationToken cancellationToken)
    {
      var user = await _userContext.Users.FirstOrDefaultAsync(f => f.Id == request.UserId , cancellationToken);
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
      return OperationResult.Success();
    }
  }
}
