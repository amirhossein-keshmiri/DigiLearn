using UserModule.Data.Entities.Users;

namespace UserModule.Core.Queries._DTOs
{
  public class UserDto
  {
    public Guid Id { get; set; }

    public DateTime CreationDate { get; set; }

    public string? Name { get; set; }

    public string? Family { get; set; }

    public string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; }

    public string Avatar { get; set; }

    public List<RoleDto> Roles { get; set; } = [];

    public List<UserToken> Tokens { get; } = [];
  }
}
