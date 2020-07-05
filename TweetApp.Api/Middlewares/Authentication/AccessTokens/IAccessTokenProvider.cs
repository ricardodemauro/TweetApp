using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TweetApp.Api.Middlewares.Authentication.AccessTokens
{
    public interface IAccessTokenProvider
    {
        Task<AccessTokenResult> ValidateToken(HttpRequest request, CancellationToken cancellationToken = default);

        string GenerateToken(TokenUser user);
    }
}
