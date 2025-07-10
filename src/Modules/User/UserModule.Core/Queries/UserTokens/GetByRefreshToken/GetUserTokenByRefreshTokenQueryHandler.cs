using AutoMapper;
using Common.Query;
using Microsoft.EntityFrameworkCore;
using UserModule.Core.Queries._DTOs;
using UserModule.Data.Entities;

namespace UserModule.Core.Queries.UserTokens.GetByRefreshToken
{
  public class GetUserTokenByRefreshTokenQueryHandler : IQueryHandler<GetUserTokenByRefreshTokenQuery, UserTokenDto?>
  {
    private readonly UserContext _userContext;
    private readonly IMapper _mapper;

    public GetUserTokenByRefreshTokenQueryHandler(UserContext userContext, IMapper mapper)
    {
      _userContext = userContext;
      _mapper = mapper;
    }

    public async Task<UserTokenDto?> Handle(GetUserTokenByRefreshTokenQuery request, CancellationToken cancellationToken)
    {
      var userTokens = await _userContext.UserTokens.FirstOrDefaultAsync(f => f.HashRefreshToken == request.HashRefreshToken, cancellationToken);
      if (userTokens == null)
      {
        return null;
      }

      return _mapper.Map<UserTokenDto>(userTokens);
    }
  }
}
