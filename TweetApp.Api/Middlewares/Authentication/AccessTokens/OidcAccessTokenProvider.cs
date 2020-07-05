using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
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

        public async Task<AccessTokenResult> ValidateToken(HttpRequest request, CancellationToken cancellationToken = default)
        {
            var token = GetAccessTokenFromRequest(request);

            var disco = await GetDiscoAsync();

            var client = _httpClientFactory.CreateClient();

            return await ValidateUsingUserInfo(client, disco, token, cancellationToken);
            //return await ValidateUsingInstrospect(client, disco, token, cancellationToken);
            //return await ValidateUsingJWKs(client, disco, token, cancellationToken);
        }

        async Task<AccessTokenResult> ValidateUsingUserInfo(HttpClient client, DiscoveryDocumentResponse disco, string token, CancellationToken cancellationToken = default)
        {
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

        async Task<AccessTokenResult> ValidateUsingInstrospect(HttpClient client, DiscoveryDocumentResponse disco, string token, CancellationToken cancellationToken = default)
        {
            var instrospectResponse = await client.IntrospectTokenAsync(new TokenIntrospectionRequest()
            {
                Address = disco.IntrospectionEndpoint,
                ClientId = Environment.GetEnvironmentVariable("ClientId"),
                Token = token,
                TokenTypeHint = "access_token"
            }, CancellationToken.None);

            var claimsColl = instrospectResponse.Claims;

            var claimsIdentity = new ClaimsIdentity(claimsColl, "oidc");

            var principal = new ClaimsPrincipal(claimsIdentity);

            return AccessTokenResult.Success(principal);
        }

        async Task<AccessTokenResult> ValidateUsingJWKs(HttpClient client, DiscoveryDocumentResponse disco, string token, CancellationToken cancellationToken = default)
        {
            // Replace with your authorization server URL:

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                _issuerUrl + "/.well-known/oauth-authorization-server",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            var discoveryDocument = await configurationManager.GetConfigurationAsync(cancellationToken);
            var signingKeys = discoveryDocument.SigningKeys;

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,

                ValidateIssuer = true,
                ValidIssuer = _issuerUrl,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,

                ValidateAudience = false,

                ValidateLifetime = true,
                // Allow for some drift in server time
                // (a lower value is better; we recommend two minutes or less)
                ClockSkew = TimeSpan.FromMinutes(2),
                // See additional validation for aud below
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, validationParameters, out var rawValidatedToken);

                var securityToken = (JwtSecurityToken)rawValidatedToken;

                return AccessTokenResult.Success(principal);
            }
            catch (SecurityTokenValidationException ex)
            {
                // Logging, etc.

                return AccessTokenResult.Expired();
            }


            //var claimsColl = instrospectResponse.Claims;

            //var claimsIdentity = new ClaimsIdentity(claimsColl, "oidc");

            //var principal = new ClaimsPrincipal(claimsIdentity);

            //return AccessTokenResult.Success(principal);
        }
    }
}
