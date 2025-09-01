using DigiLearn.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Profile
{
  [Authorize]
  public class IndexModel : PageModel
  {
    private readonly IUserFacade _userFacade;
    private readonly INotificationFacade _notificationFacade;
    public IndexModel(IUserFacade userFacade, INotificationFacade notificationFacade)
    {
      _userFacade = userFacade;
      _notificationFacade = notificationFacade;
    }

    public UserDto UserDto { get; set; }
    public List<NotificationFilterData> NewNotifications { get; set; }
    public async Task OnGet()
    {
      UserDto = await _userFacade.GetUserByPhoneNumber(User.GetPhoneNumber());
      var result = await _notificationFacade.GetByFilter(new NotificationFilterParams()
      {
        IsSeen = false,
        PageId = 1,
        Take = 5,
        UserId = UserDto!.Id
      });
      NewNotifications = result.Data;
    }

    // Helper method to truncate text
    public string Truncate(string value, int maxLength = 100)
    {
      if (string.IsNullOrEmpty(value)) return value;
      return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "…";
    }
  }
}
