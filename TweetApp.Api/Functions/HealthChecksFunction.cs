using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetApp.Api.Functions
{
    public static class HealthChecksFunction
    {
        [FunctionName("HealthChecks")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Healthz")] HttpRequest req)
        {
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            var conn = Environment.GetEnvironmentVariable("TableStorageConnectionString");

            return new OkObjectResult(new { issuer, conn });
        }
    }
}
