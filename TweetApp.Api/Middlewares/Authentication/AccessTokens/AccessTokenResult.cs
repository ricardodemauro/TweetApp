namespace TweetApp.Api.Middlewares.Authentication.AccessTokens
{
    using System;
    using System.Security.Claims;

    public sealed class AccessTokenResult
    {
        private AccessTokenResult() { }

        public ClaimsPrincipal Principal { get; private set; }

        public AccessTokenStatus Status { get; private set; }

        public Exception Exception { get; private set; }

        public static AccessTokenResult Success(ClaimsPrincipal principal) =>
            new AccessTokenResult
            {
                Principal = principal,
                Status = AccessTokenStatus.Valid
            };

        public static AccessTokenResult Expired() =>
            new AccessTokenResult
            {
                Status = AccessTokenStatus.Expired
            };

        public static AccessTokenResult Error(Exception ex) =>
            new AccessTokenResult
            {
                Status = AccessTokenStatus.Error,
                Exception = ex
            };

        public static AccessTokenResult NoToken() =>
            new AccessTokenResult
            {
                Status = AccessTokenStatus.NoToken
            };
    }
}
