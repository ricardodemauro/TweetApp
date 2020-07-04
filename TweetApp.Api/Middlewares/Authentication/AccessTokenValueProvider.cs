namespace TweetApp.Api.Middlewares.Authentication
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TweetApp.Api.Middlewares.Authentication.AccessTokens;

    public class AccessTokenValueProvider : IValueProvider
    {
        private HttpRequest _request;

        public AccessTokenValueProvider(HttpRequest request)
        {
            _request = request;
        }

        public async Task<object> GetValueAsync()
        {
            try
            {
                var accessTokenProvider = _request.HttpContext.RequestServices.GetRequiredService<IAccessTokenProvider>();

                var result = await accessTokenProvider.ValidateToken(_request);

                return AccessTokenResult.Success(result.Principal);

            }
            catch (SecurityTokenExpiredException)
            {
                return AccessTokenResult.Expired();
            }
            catch (Exception ex)
            {
                return AccessTokenResult.Error(ex);
            }
        }

        public Type Type => typeof(ClaimsPrincipal);

        public string ToInvokeString() => string.Empty;
    }
}
