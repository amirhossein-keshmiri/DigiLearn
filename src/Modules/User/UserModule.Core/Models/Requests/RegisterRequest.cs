using System.ComponentModel.DataAnnotations;

namespace UserModule.Core.Models.Requests
{
  public class RegisterRequest
  {
    [Display(Name = "PhoneNumber")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [MaxLength(11, ErrorMessage = "Phone Number Must Be 11 Digits")]
    [MinLength(11, ErrorMessage = "Phone Number Must Be 11 Digits")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [MinLength(5, ErrorMessage = "Password Must Be More Than 5 Character")]
    public string Password { get; set; }

    [Display(Name = "ConfirmPassword")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [Compare("Password", ErrorMessage = "Confirm PassWord Is Not Correct")]
    public string ConfirmPassword { get; set; }
  }
}
