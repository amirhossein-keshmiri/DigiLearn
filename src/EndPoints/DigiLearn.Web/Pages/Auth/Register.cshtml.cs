using Common.Application;
using Common.Application.Validation;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Models.Requests;
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

    public RegisterRequest RegisterRequest { get; set; }
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
      try
      {
        var result = await _userFacade.RegisterUser(new RegisterUserCommand()
        {
          Password = RegisterRequest.Password,
          PhoneNumber = RegisterRequest.PhoneNumber,
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
      catch (InvalidCommandException ex)
      {
        ErrorAlert(ex.Message);
        return Page();
      }
    }
  }
}
