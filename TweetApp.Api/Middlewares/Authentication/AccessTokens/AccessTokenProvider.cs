namespace TweetApp.Api.Middlewares.Authentication.AccessTokens
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class AccessTokenProvider : IAccessTokenProvider
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        private readonly string _issuerToken;
        private readonly string _audience;
        private readonly string _issuer;

        public AccessTokenProvider(string issuerToken, string audience, string issuer)
        {
            _issuerToken = issuerToken;
            _audience = audience;
            _issuer = issuer;
        }

        byte[] Token => Encoding.ASCII.GetBytes(_issuerToken);

        public Task<AccessTokenResult> ValidateToken(HttpRequest request)
        {
            try
            {
                if (request != null &&
                    request.Headers.ContainsKey(AUTH_HEADER_NAME) &&
                    request.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX))
                {
                    var token = request.Headers[AUTH_HEADER_NAME].ToString().Substring(BEARER_PREFIX.Length);

                    var tokenParams = new TokenValidationParameters()
                    {
                        RequireSignedTokens = true,
                        ValidAudience = _audience,
                        ValidateAudience = true,
                        ValidIssuer = _issuer,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Token)
                    };

                    var handler = new JwtSecurityTokenHandler();
                    var result = handler.ValidateToken(token, tokenParams, out var securityToken);
                    return Task.FromResult<AccessTokenResult>(AccessTokenResult.Success(result));

                }
                else
                {
                    return Task.FromResult<AccessTokenResult>(AccessTokenResult.NoToken());
                }
            }
            catch (SecurityTokenExpiredException)
            {
                return Task.FromResult<AccessTokenResult>(AccessTokenResult.Expired());
            }
            catch (Exception ex)
            {
                return Task.FromResult<AccessTokenResult>(AccessTokenResult.Error(ex));
            }
        }

        public string GenerateToken(TokenUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                }, "JWT"),
                Audience = _audience,
                Issuer = _issuer,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Token),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
