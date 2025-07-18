using Common.Application;

namespace UserModule.Core.Commands.Users.EditProfile
{
  public class EditUserProfileCommand : IBaseCommand
  {
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public string? Email { get; set; }
  }
}
