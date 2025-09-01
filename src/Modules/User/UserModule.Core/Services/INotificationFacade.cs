using Common.Application;
using UserModule.Core.Commands.Notifications.Create;
using UserModule.Core.Commands.Notifications.Delete;
using UserModule.Core.Commands.Notifications.DeleteAll;
using UserModule.Core.Commands.Notifications.Seen;
using UserModule.Core.Commands.Notifications.SeenAll;
using UserModule.Core.Queries._DTOs;

namespace UserModule.Core.Services
{
  public interface INotificationFacade
  {
    Task<OperationResult> Create(CreateNotificationCommand command);
    Task<OperationResult> Delete(DeleteNotificationCommand command);
    Task<OperationResult> DeleteAll(DeleteAllNotificationCommand command);
    Task<OperationResult> Seen(SeenNotificationCommand command);
    Task<OperationResult> SeenAll(SeenAllNotificationsCommand command);

    Task<NotificationFilterResult> GetByFilter(NotificationFilterParams filterParam);
    Task<NotificationDto?> GetNotification(Guid notificationId, Guid userId);
  }
}
