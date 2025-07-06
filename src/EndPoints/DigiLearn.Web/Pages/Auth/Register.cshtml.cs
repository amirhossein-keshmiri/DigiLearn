using Common.Application;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Services;
namespace DigiLearn.Web.Pages.Auth
{
  [BindProperties]
  public class RegisterModel : BaseRazor
  {
    private readonly IUserFacade _userFacade;
    public RegisterModel(IUserFacade userFacade)
    {
      _userFacade = userFacade;
    }

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
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
      var result = await _userFacade.RegisterUser(new RegisterUserCommand()
      {
        Password = Password,
        PhoneNumber = PhoneNumber,
      });

      if (result.Status == OperationResultStatus.Success)
      {
        result.Message = "Registration was successful.";
      }
      else
      {
        result.Message = "An error has occurred.";
      }
      return RedirectAndShowAlert(result, RedirectToPage("Login"));
    }
  }
}
