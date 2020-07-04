using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TweetApp.Api.Data;
using TweetApp.Api.Extensions;
using TweetApp.Api.Middlewares.Authentication;
using TweetApp.Api.Middlewares.Authentication.AccessTokens;
using TweetApp.Api.Models;

namespace TweetApp.Api.Functions
{
    public class TweetFunctions
    {
        private readonly ILogger<TweetFunctions> _logger;

        private readonly ITweetRepository _repository;

        public TweetFunctions(ILogger<TweetFunctions> logger, ITweetRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [FunctionName("Create")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Tweet")] HttpRequest req,
            [AccessToken] AccessTokenResult accessToken = default)
        {
            _logger.LogInformation("Starting {operation}", nameof(Create));

            if (accessToken?.Status != AccessTokenStatus.Valid)
                return new UnauthorizedResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Tweet data = JsonConvert.DeserializeObject<Tweet>(requestBody);

            if (data == null)
                return new BadRequestObjectResult("Invalid request");

            data.User = accessToken.Principal.UserName();
            data.UserId = accessToken.Principal.UserId();

            data = await _repository.InsertOneAsync(data);

            return new OkObjectResult(data);
        }

        [FunctionName("List")]
        public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Tweet")] HttpRequest req,
            [AccessToken] AccessTokenResult accessToken = default)
        {
            _logger.LogInformation("Starting {operation}", nameof(List));

            if (accessToken?.Status != AccessTokenStatus.Valid)
                return new UnauthorizedResult();

            var documents = await _repository.List();

            return new OkObjectResult(documents);
        }
    }
}
