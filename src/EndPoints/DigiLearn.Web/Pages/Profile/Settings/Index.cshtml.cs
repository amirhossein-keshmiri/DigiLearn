using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigiLearn.Web.Pages.Profile.Settings
{
  [Authorize]
  [BindProperties]
  public class IndexModel : BaseRazor
  {
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public string Name { get; set; }

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "{0} را وارد کنید")]

    public string Family { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    public void OnGet()
    {
    }
  }
}
