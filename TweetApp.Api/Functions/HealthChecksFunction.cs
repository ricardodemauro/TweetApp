using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;

namespace TweetApp.Api.Functions
{
    public static class HealthChecksFunction
    {
        [FunctionName("HealthChecks")]
        public static IActionResult HealthChecks(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Healthz")] HttpRequest req)
        {
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            var conn = Environment.GetEnvironmentVariable("TableStorageConnectionString");

            var result = new { issuerEmpty = string.IsNullOrEmpty(issuer), tableConnectionStringEmpty = string.IsNullOrEmpty(conn) };

            return new OkObjectResult(result);
        }

        [FunctionName("HealthChecksRaw")]
        public static IActionResult HealthChecksRaw(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Healthz/raw")] HttpRequest req)
        {
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            var conn = Environment.GetEnvironmentVariable("TableStorageConnectionString");

            var result = new { issuer, tableConnectionString = conn };

            return new OkObjectResult(result);
        }
    }
}
