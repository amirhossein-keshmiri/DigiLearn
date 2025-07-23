using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Pages.Profile.Settings
{
  [Authorize]
  [BindProperties]
  public class ChangePassword : BaseRazor
  {
    [Display(Name = "نام")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public string Name { get; set; }

    [Display(Name = "نام خانوادگی")]
    [Required(ErrorMessage = "{0} را وارد کنید")]

    public string Family { get; set; }

    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    public void OnGet()
    {
    }
  }
}
