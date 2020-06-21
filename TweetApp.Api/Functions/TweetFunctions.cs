using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TweetApp.Api.Models;
using TweetApp.Api.Data;
using MongoDB.Driver;

namespace TweetApp.Api.Functions
{
    public static class TweetFunctionc
    {
        [FunctionName("Create")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Tweet")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Tweet data = JsonConvert.DeserializeObject<Tweet>(requestBody);

            if (data == null)
                return new BadRequestObjectResult("Invalid request");

            data.Created = DateTime.UtcNow;

            var tweetColl = MongoDbFactory.Get<Tweet>("tweets");
            await tweetColl.InsertOneAsync(data);

            return new OkObjectResult(data);
        }

        [FunctionName("List")]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Tweet")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var tweetColl = MongoDbFactory.Get<Tweet>("tweets");
            var documents = await tweetColl.Find(Builders<Tweet>.Filter.Empty).ToListAsync();

            return new OkObjectResult(documents);
        }
    }
}
