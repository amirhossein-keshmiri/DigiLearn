using Common.Application.DateUtil;
using Common.Query;
using Microsoft.EntityFrameworkCore;
using UserModule.Core.Queries._DTOs;
using UserModule.Data.Entities;

namespace UserModule.Core.Queries.Notifications.GetByFilter
{
  public class GetNotificationsByFilterQueryHandler : IQueryHandler<GetNotificationsByFilterQuery, NotificationFilterResult>
  {
    private readonly UserContext _userContext;

    public GetNotificationsByFilterQueryHandler(UserContext userContext)
    {
      _userContext = userContext;
    }

    public async Task<NotificationFilterResult> Handle(GetNotificationsByFilterQuery request, CancellationToken cancellationToken)
    {
      var result = _userContext.UserNotifications.Where(u => u.UserId == request.FilterParams.UserId).OrderByDescending(x => x.CreationDate).AsQueryable();

      if (request.FilterParams.IsSeen != null)
      {
        result = result.Where(r => r.IsSeen == request.FilterParams.IsSeen);
      }

      if (string.IsNullOrWhiteSpace(request.FilterParams.Title) == false)
        result = result.Where(r => r.Title.Contains(request.FilterParams.Title) || r.Text.Contains(request.FilterParams.Title));

      var skip = (request.FilterParams.PageId - 1) * request.FilterParams.Take;
      var model = new NotificationFilterResult()
      {
        Data = await result.Skip(skip).Take(request.FilterParams.Take)
              .Select(s => new NotificationFilterData
              {
                Id = s.Id,
                UserId = s.UserId,
                Title = s.Title,
                Text = s.Text,
                IsSeen = s.IsSeen,
                CreationDate = s.CreationDate,
                PersianCreationDate = s.CreationDate.ToPersianDate()
              }).OrderByDescending(o => o.CreationDate).ToListAsync(cancellationToken)
      };

      model.GeneratePaging(result, request.FilterParams.Take, request.FilterParams.PageId);
      return model;
    }
  }
}
