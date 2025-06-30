using AutoMapper;
using Common.Query;
using Microsoft.EntityFrameworkCore;
using UserModule.Core.Queries._DTOs;
using UserModule.Data.Entities;

namespace UserModule.Core.Queries.Users.GetByPhoneNumber
{
  public class GetUserByPhoneNumberQueryHandler : IQueryHandler<GetUserByPhoneNumberQuery, UserDto?>
  {
    private readonly UserContext _userContext;
    private readonly IMapper _mapper;
    public GetUserByPhoneNumberQueryHandler(UserContext userContext, IMapper mapper)
    {
      _userContext = userContext;
      _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByPhoneNumberQuery request, CancellationToken cancellationToken)
    {
      var user = await _userContext.Users.FirstOrDefaultAsync(f => f.PhoneNumber == request.PhoneNumber, cancellationToken);
      if (user == null)
      {
        return null;
      }

      return _mapper.Map<UserDto>(user);
    }
  }
}
