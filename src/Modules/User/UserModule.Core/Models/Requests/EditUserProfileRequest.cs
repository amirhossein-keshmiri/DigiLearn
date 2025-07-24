using System.ComponentModel.DataAnnotations;

namespace UserModule.Core.Models.Requests
{
  public class EditUserProfileRequest
  {
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [MinLength(3, ErrorMessage = "The Lenght is too short and must be greater than 3")]
    public string Name { get; set; }

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [MinLength(3, ErrorMessage = "The Lenght is too short and must be greater than 3")]
    public string Family { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
  }
}
