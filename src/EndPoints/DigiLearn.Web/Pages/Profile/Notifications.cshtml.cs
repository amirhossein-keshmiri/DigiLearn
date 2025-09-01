using Common.Application;
using Common.Application.DateUtil;
using DigiLearn.Web.Infrastructure;
using DigiLearn.Web.Infrastructure.RazorUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketModule.Core.DTOs.Tickets;
using UserModule.Core.Commands.Notifications.Delete;
using UserModule.Core.Commands.Notifications.DeleteAll;
using UserModule.Core.Commands.Notifications.Seen;
using UserModule.Core.Commands.Notifications.SeenAll;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;

namespace DigiLearn.Web.Pages.Profile
{
  [Authorize]
  public class NotificationsModel : BaseRazorFilter<NotificationFilterParams>
  {
    private readonly INotificationFacade _notificationFacade;

    public NotificationsModel(INotificationFacade notificationFacade)
    {
      _notificationFacade = notificationFacade;
    }

    public NotificationFilterResult FilterResult { get; set; }

    [BindProperty(SupportsGet = true)]
    public NotificationFilterParams? NotificationFilterParams { get; set; }

    public async Task OnGet()
    {
      FilterResult = await _notificationFacade.GetByFilter(new NotificationFilterParams()
      {
        IsSeen = NotificationFilterParams?.IsSeen,
        Title = NotificationFilterParams?.Title,
        PageId = FilterParams.PageId,
        Take = FilterParams.Take,
        UserId = User.GetUserId()
      });
    }

    public async Task<IActionResult> OnGetFilter()
    {
      // Parse query parameters
      var isSeenStr = Request.Query["NotificationFilterParams.IsSeen"];

      bool? isSeen = null;
      if (!string.IsNullOrEmpty(isSeenStr))
      {
        if (bool.TryParse(isSeenStr, out var parsed))
          isSeen = parsed;
        else if (string.Equals(isSeenStr, "true", StringComparison.OrdinalIgnoreCase))
          isSeen = true;
        else if (string.Equals(isSeenStr, "false", StringComparison.OrdinalIgnoreCase))
          isSeen = false;
      }

      var filterParams = new NotificationFilterParams
      {
        UserId = User.GetUserId(),
        Title = Request.Query["NotificationFilterParams.Title"],
        Text = Request.Query["NotificationFilterParams.Text"],
        IsSeen = isSeen,
        PageId = int.TryParse(Request.Query["filterParams.pageId"], out var page) ? page : 1,
        Take = int.TryParse(Request.Query["FilterParams.Take"], out var take) ? take : 10
      };

      var result = await _notificationFacade.GetByFilter(filterParams);

      var dto = new
      {
        success = true,
        filterResult = new PagedResult<NotificationFilterData>
        {
          Data = result.Data.Select(t => new NotificationFilterData
          {
            Id = t.Id,
            Title = t.Title,
            Text = t.Text,
            IsSeen = t.IsSeen,
            CreationDate = t.CreationDate,
            PersianCreationDate = t.PersianCreationDate,
          }).ToList(),
          CurrentPage = result.CurrentPage,
          TotalPages = result.PageCount,
          TotalCount = result.EntityCount,
          Take = result.Take
        }
      };

      return new JsonResult(dto);
    }

    public async Task<IActionResult> OnPostDeleteAll()
    {
      try
      {
        var result = await _notificationFacade.DeleteAll(new DeleteAllNotificationCommand(User.GetUserId()));

        if (result.Status == OperationResultStatus.NotFound)
        {
          return new JsonResult(new { success = false, message = "Notifications not found." });
        }
        if (result.Status == OperationResultStatus.Success)
        {
          return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = result.Message });
      }
      catch (Exception ex)
      {
        return new JsonResult(new { success = false, message = ex.Message });
      }
    }

    public async Task<IActionResult> OnGetNotificationById(Guid notificationId)
    {
      var notification = await _notificationFacade.GetNotification(notificationId, User.GetUserId());
      if (notification == null)
      {
        return new JsonResult(new { success = false });
      }

      // Map NotificationDto to a simplified DTO for JSON serialization
      var notificationDetails = new NotificationDto
      {
        Id = notification.Id,
        Title = notification.Title,
        Text = notification.Text,
        CreationDate = notification.CreationDate,
        PersianCreationDate = notification.CreationDate.ToPersianDateTime(),
        IsSeen = notification.IsSeen
      };

      return new JsonResult(new { success = true, notification = notificationDetails });
    }

    public async Task<IActionResult> OnPostDeleteNotification(Guid id)
    {
      try
      {
        var result = await _notificationFacade.Delete(new DeleteNotificationCommand(id, User.GetUserId()));

        if (result.Status == OperationResultStatus.NotFound)
        {
          return new JsonResult(new { success = false, message = "Notification not found." });
        }

        if (result.Status == OperationResultStatus.Success)
        {
          return new JsonResult(new { success = true });
        }
        return new JsonResult(new { success = false, message = result.Message });
      }
      catch (Exception ex)
      {
        return new JsonResult(new { success = false, message = ex.Message });
      }
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostSeenNotification(Guid notificationId)
    {
      var result = await _notificationFacade.Seen(new SeenNotificationCommand(notificationId, User.GetUserId()));

      if (result.Status == OperationResultStatus.NotFound)
      {
        return new JsonResult(new { success = false, message = "Notification not found." });
      }

      if (result.Status == OperationResultStatus.Success)
      {
        return new JsonResult(new { success = true });
      }

      return new JsonResult(new { success = false, message = result.Message });
    }

    public async Task<IActionResult> OnPostSeenAll()
    {
      try
      {
        var result = await _notificationFacade.SeenAll(new SeenAllNotificationsCommand(User.GetUserId()));

        if (result.Status == OperationResultStatus.Success)
        {
          return new JsonResult(new { success = true });
        }

        return new JsonResult(new { success = false, message = result.Message });
      }
      catch (Exception ex)
      {
        return new JsonResult(new { success = false, message = ex.Message });
      }
    }

    // Helper method to truncate text
    public string Truncate(string value, int maxLength = 100)
    {
      if (string.IsNullOrEmpty(value)) return value;
      return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "…";
    }
  }
}
