using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserModule.Core.Commands.Users.EditProfile;
using UserModule.Core.Models.Requests;
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

    public EditUserProfileRequest EditUserProfile { get; set; }

    public async Task OnGet()
    {
      var user = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());

      EditUserProfile = new EditUserProfileRequest();
      if (user != null)
      {
        EditUserProfile.Name = user.Name;
        EditUserProfile.Family = user.Family;
        EditUserProfile.Email = user.Email;
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
        Name = EditUserProfile.Name,
        Family = EditUserProfile.Family,
        Email = EditUserProfile.Email
      });

      return RedirectAndShowAlert(result, RedirectToPage("Index"));
    }
  }
}
