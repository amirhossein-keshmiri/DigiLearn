using AutoMapper;
using Common.Query;
using Microsoft.EntityFrameworkCore;
using UserModule.Core.Queries._DTOs;
using UserModule.Data.Entities;

namespace UserModule.Core.Queries.Users.GetById
{
  public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto?>
  {
    private readonly UserContext _userContext;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(UserContext userContext, IMapper mapper)
    {
      _userContext = userContext;
      _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
      var user = await _userContext.Users.FirstOrDefaultAsync(f => f.Id == request.UserId, cancellationToken);
      if (user == null)
        return null;

      return _mapper.Map<UserDto>(user);
    }
  }
}
