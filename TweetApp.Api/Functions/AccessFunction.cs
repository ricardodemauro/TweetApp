using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TweetApp.Api.Middlewares.Authentication;
using TweetApp.Api.Middlewares.Authentication.AccessTokens;

namespace TweetApp.Api.Functions
{
    public class AccessFunction
    {
        private readonly IAccessTokenProvider _tokenProvider;

        private readonly ILogger<AccessFunction> _logger;

        public AccessFunction(IAccessTokenProvider tokenProvider, ILogger<AccessFunction> logger)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("ValidateToken")]
        public async Task<IActionResult> ValidateToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Token")] HttpRequest req)
        {
            var result = await _tokenProvider.ValidateToken(req);

            if (result.Status == AccessTokenStatus.Valid)
            {
                _logger.LogInformation($"Request received for {result.Principal.Identity.Name}.");
                return new OkResult();
            }
            else
            {
                return new UnauthorizedResult();

            }
        }

        [FunctionName("CreateToken")]
        public async Task<IActionResult> CreateToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Token")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TokenUser data = JsonConvert.DeserializeObject<TokenUser>(requestBody);

            var result = _tokenProvider.GenerateToken(data);

            _logger.LogInformation($"Generated token");
            return new OkObjectResult(result);
        }
    }
}