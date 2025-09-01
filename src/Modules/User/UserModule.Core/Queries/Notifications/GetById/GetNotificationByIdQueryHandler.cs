using AutoMapper;
using Common.Query;
using Microsoft.EntityFrameworkCore;
using UserModule.Core.Queries._DTOs;
using UserModule.Data.Entities;

namespace UserModule.Core.Queries.Notifications.GetById
{
  public class GetNotificationByIdQueryHandler : IQueryHandler<GetNotificationByIdQuery, NotificationDto?>
  {
    private readonly UserContext _userContext;
    private readonly IMapper _mapper;

    public GetNotificationByIdQueryHandler(UserContext userContext, IMapper mapper)
    {
      _userContext = userContext;
      _mapper = mapper;
    }

    public async Task<NotificationDto?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
      var notification = await _userContext.UserNotifications.FirstOrDefaultAsync(f => f.Id == request.NotificationId && f.UserId == request.UserId, cancellationToken);
      if (notification == null)
        return null;

      return _mapper.Map<NotificationDto>(notification);
    }
  }
}
