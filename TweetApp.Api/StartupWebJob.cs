using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using TweetApp.Api.Middlewares.Authentication.Extensions;

[assembly: WebJobsStartup(typeof(TweetApp.Api.StartupWebJob))]
namespace TweetApp.Api
{
    public class StartupWebJob : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddAccessTokenBinding();
        }
    }
}
