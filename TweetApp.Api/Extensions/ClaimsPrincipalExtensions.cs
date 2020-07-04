using System.Linq;
using System.Security.Claims;

namespace TweetApp.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string UserId(this ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
                return principal.Claims.First(x => x.Type == "sub").Value;
            return string.Empty;
        }

        public static string UserName(this ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
                return principal.Claims.First(x => x.Type == "name").Value;
            return string.Empty;
        }
    }
}
