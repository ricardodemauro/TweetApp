using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TweetApp.Api.Middlewares.Authentication.AccessTokens
{
    public class OidcAccessTokenProvider : IAccessTokenProvider
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";

        private readonly string _issuerUrl;

        private DiscoveryDocumentResponse _disco;

        private readonly IHttpClientFactory _httpClientFactory;

        public OidcAccessTokenProvider(string issuerUrl, IHttpClientFactory httpClientFactory)
        {
            _issuerUrl = issuerUrl ?? throw new ArgumentNullException(nameof(issuerUrl));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public string GenerateToken(TokenUser user)
        {
            throw new NotImplementedException();
        }

        static string GetAccessTokenFromRequest(HttpRequest request)
        {
            if (request != null &&
                request.Headers.ContainsKey(AUTH_HEADER_NAME) &&
                request.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX))
            {
                var token = request.Headers[AUTH_HEADER_NAME].ToString().Substring(BEARER_PREFIX.Length);

                return token;
            }
            return string.Empty;
        }

        private async ValueTask<DiscoveryDocumentResponse> GetDiscoAsync()
        {
            if (_disco != null)
                return _disco;

            var discoveryRequest = new DiscoveryDocumentRequest()
            {
                Address = _issuerUrl,
                Policy = new DiscoveryPolicy
                {
                    ValidateEndpoints = false, // okta has this issue
                }
            };

            using var client = _httpClientFactory.CreateClient();

            var disco = await client.GetDiscoveryDocumentAsync(discoveryRequest);
            if (disco.IsError) throw new Exception(disco.Error);

            _disco = disco;
            return disco;
        }

        public async Task<AccessTokenResult> ValidateToken(HttpRequest request)
        {
            var token = GetAccessTokenFromRequest(request);

            var disco = await GetDiscoAsync();

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = token
            });

            var claimsColl = response.Claims;
            var claimsIdentity = new ClaimsIdentity(claimsColl, "oidc");

            var principal = new ClaimsPrincipal(claimsIdentity);

            return AccessTokenResult.Success(principal);
        }
    }
}
