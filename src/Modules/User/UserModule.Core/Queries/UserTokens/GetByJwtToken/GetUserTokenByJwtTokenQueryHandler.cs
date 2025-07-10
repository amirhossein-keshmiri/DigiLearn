using AutoMapper;
using Common.Query;
using Microsoft.EntityFrameworkCore;
using UserModule.Core.Queries._DTOs;
using UserModule.Data.Entities;

namespace UserModule.Core.Queries.UserTokens.GetByJwtToken
{
  public class GetUserTokenByJwtTokenQueryHandler : IQueryHandler<GetUserTokenByJwtTokenQuery, UserTokenDto?>
  {
    private readonly UserContext _userContext;
    private readonly IMapper _mapper;

    public GetUserTokenByJwtTokenQueryHandler(UserContext userContext, IMapper mapper)
    {
      _userContext = userContext;
      _mapper = mapper;
    }

    public async Task<UserTokenDto?> Handle(GetUserTokenByJwtTokenQuery request, CancellationToken cancellationToken)
    {
      var userTokens = await _userContext.UserTokens.FirstOrDefaultAsync(f => f.HashJwtToken == request.HashJwtToken, cancellationToken);
      if (userTokens == null)
      {
        return null;
      }

      return _mapper.Map<UserTokenDto>(userTokens);
    }
  }
}
