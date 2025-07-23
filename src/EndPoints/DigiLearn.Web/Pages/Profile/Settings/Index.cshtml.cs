using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserModule.Core.Commands.Users.EditProfile;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Profile.Settings
{
  [Authorize]
  [BindProperties]
  public class IndexModel : BaseRazor
  {
    private readonly IUserFacade _userFacade;

    public IndexModel(IUserFacade userFacade)
    {
      _userFacade = userFacade;
    }

    [Display(Name = "First Name")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    [MinLength(3, ErrorMessage = "The Lenght is too short and must be greater than 3")]
    public string Name { get; set; }

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    [MinLength(3, ErrorMessage = "The Lenght is too short and must be greater than 3")]
    public string Family { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    public async Task OnGet()
    {
      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
      if (user != null)
      {
        Name = user.Name;
        Family = user.Family;
        Email = user.Email;
      }
    }

    public async Task<IActionResult> OnPost()
    {
      if (!ModelState.IsValid)
      {
        return Page(); // Re-render with errors
      }

      var result = await _userFacade.EditUserProfile(new EditUserProfileCommand()
      {
        UserId = User.GetUserId(),
        Name = Name,
        Family = Family,
        Email = Email
      });

      return RedirectAndShowAlert(result, RedirectToPage("Index"));
    }
  }
}
