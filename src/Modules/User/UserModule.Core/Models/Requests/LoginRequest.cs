using System.ComponentModel.DataAnnotations;

namespace UserModule.Core.Models.Requests
{
  public class LoginRequest
  {
    [Display(Name = "PhoneNumber")]
    [Required(ErrorMessage = "Please Enter {0}")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [MinLength(5, ErrorMessage = "Password Must Be More Than 5 Character")]
    public string Password { get; set; }

    public bool IsRememberMe { get; set; }
  }
}
