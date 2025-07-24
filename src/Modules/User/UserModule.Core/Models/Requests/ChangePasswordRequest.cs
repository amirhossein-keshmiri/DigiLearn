using System.ComponentModel.DataAnnotations;

namespace UserModule.Core.Models.Requests
{
  public class ChangePasswordRequest
  {
    [Display(Name = "Current Password")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Display(Name = "New Password")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [DataType(DataType.Password)]

    public string NewPassword { get; set; }

    [Display(Name = "Confirm New Password")]
    [Required(ErrorMessage = "Please Enter {0}")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The Passwords Are not Matched")]
    public string ConfirmNewPassword { get; set; }
  }
}
