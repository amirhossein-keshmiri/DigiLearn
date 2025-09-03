using Common.Application;
using Common.Application.DateUtil;
using DigiLearn.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModule.Core.Commands.Notifications.Delete;
using UserModule.Core.Commands.Notifications.DeleteAll;
using UserModule.Core.Commands.Notifications.Seen;
using UserModule.Core.Commands.Notifications.SeenAll;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;

namespace DigiLearn.Api.Controllers
{
  [Authorize]
  public class NotificationsController : ApiController
  {
    private readonly INotificationFacade _notificationFacade;

    public NotificationsController(INotificationFacade notificationFacade)
    {
      _notificationFacade = notificationFacade;
    }

    [HttpGet("GetNotificationsByFilter")]
    public async Task<ApiResult<PagedResult<NotificationFilterData>>> GetNotificationsByFilter(
        [FromQuery] bool? isSeen,
        [FromQuery] string? title,
        [FromQuery] int page = 1,
        [FromQuery] int take = 10)
    {
      var filterParams = new NotificationFilterParams
      {
        UserId = User.GetUserId(),
        IsSeen = isSeen,
        Title = title,
        PageId = page < 1 ? 1 : page,
        Take = take < 1 ? 10 : take
      };

      var result = await _notificationFacade.GetByFilter(filterParams);

      var dto = new PagedResult<NotificationFilterData>
      {
        Data = result.Data.Select(t => new NotificationFilterData
        {
          Id = t.Id,
          UserId = t.UserId,
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
      };

      return QueryResult(dto);
    }

    [HttpGet("GetNotification/{id:guid}")]
    public async Task<ApiResult<NotificationDto>> GetNotification(Guid id)
    {
      var notification = await _notificationFacade.GetNotification(id, User.GetUserId());
      if (notification == null)
      {
        return QueryResult(new NotificationDto());
      }

      var dto = new NotificationDto
      {
        Id = notification.Id,
        UserId = notification.UserId,
        Title = notification.Title,
        Text = notification.Text,
        CreationDate = notification.CreationDate,
        PersianCreationDate = notification.CreationDate.ToPersianDateTime(),
        IsSeen = notification.IsSeen
      };

      return QueryResult(dto);
    }

    [HttpDelete("{notificationId:guid}/delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiResult> DeleteNotification(Guid notificationId)
    {
      try
      {
        var notification = await _notificationFacade.Delete(new DeleteNotificationCommand(notificationId, User.GetUserId()));

        if (notification.Status == OperationResultStatus.NotFound)
        {
          var result = OperationResult.Error("notification not found or access denied!");
          return CommandResult(result);
        }

        if (notification.Status == OperationResultStatus.Success)
        {
          return CommandResult(notification);
        }

        var failedResult = OperationResult.Error("Could not delete the notification.");
        return CommandResult(failedResult);
      }
      catch (Exception ex)
      {
        var result = OperationResult.Error(string.Format("Failed to delete notification. Please try again! error: {0}", ex.Message));
        return CommandResult(result);
      }
    }

    [HttpDelete("/deleteAll")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiResult> DeleteAllNotifications()
    {
      try
      {
        var notification = await _notificationFacade.DeleteAll(new DeleteAllNotificationCommand(User.GetUserId()));

        if (notification.Status == OperationResultStatus.NotFound)
        {
          var result = OperationResult.Error("notifications not found or access denied!");
          return CommandResult(result);
        }

        if (notification.Status != OperationResultStatus.Success)
        {

          return CommandResult(notification);
        }

        var failedResult = OperationResult.Error("Could not delete the notifications.");
        return CommandResult(notification);
      }
      catch (Exception ex)
      {
        var result = OperationResult.Error(string.Format("Failed to delete notifications. Please try again! error: {0}", ex.Message));
        return CommandResult(result);
      }
    }

    [HttpPost("{notificationId:guid}/seen")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiResult> SeenNotification(Guid notificationId)
    {
      try
      {
        var notification = await _notificationFacade.Seen(new SeenNotificationCommand(notificationId, User.GetUserId()));

        if (notification.Status == OperationResultStatus.NotFound)
        {
          var result = OperationResult.Error("notification not found or access denied!");
          return CommandResult(result);
        }

        if (notification.Status == OperationResultStatus.Success)
        {
          return CommandResult(notification);
        }

        var failedResult = OperationResult.Error("Could not read the notification.");
        return CommandResult(failedResult);
      }
      catch (Exception ex)
      {
        var result = OperationResult.Error(string.Format("Failed to read notification. Please try again! error: {0}", ex.Message));
        return CommandResult(result);
      }
    }

    [HttpPost("/seenAll")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<ApiResult> SeenAllNotifications()
    {
      try
      {
        var notification = await _notificationFacade.SeenAll(new SeenAllNotificationsCommand(User.GetUserId()));

        if (notification.Status == OperationResultStatus.NotFound)
        {
          var result = OperationResult.Error("notifications not found or access denied!");
          return CommandResult(result);
        }

        if (notification.Status != OperationResultStatus.Success)
        {

          return CommandResult(notification);
        }

        var failedResult = OperationResult.Error("Could not mark all notifications as read.");
        return CommandResult(notification);
      }
      catch (Exception ex)
      {
        var result = OperationResult.Error(string.Format("Failed to mark all notifications as read. Please try again! error: {0}", ex.Message));
        return CommandResult(result);
      }
    }

  }
}
