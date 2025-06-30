using Common.Domain;
using System.ComponentModel.DataAnnotations;

namespace UserModule.Data.Entities.Users
{
  public class User : BaseEntity
  {
    [MaxLength(50)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Family { get; set; }

    [MaxLength(11)]
    [Required]
    public string PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? Email { get; set; }

    [MinLength(6)]
    [Required]
    public string Password { get; set; }

    [MaxLength(100)]
    [Required]
    public string Avatar { get; set; }

    public List<UserRole> UserRoles { get; set; }
  }
}
