using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using TweetApp.Api.Data;
using TweetApp.Api.Middlewares.Authentication.AccessTokens;

[assembly: FunctionsStartup(typeof(TweetApp.Api.StartupFunction))]

namespace TweetApp.Api
{
    public class StartupFunction : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var tableStorageConnectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");

            builder.Services.AddTransient<ITweetRepository, AzTableStorageRepository>(x =>
                new AzTableStorageRepository(
                    x.GetRequiredService<ILogger<AzTableStorageRepository>>(),
                    tableStorageConnectionString));

            builder.Services.AddHttpContextAccessor();

            // Get the configuration files for the OAuth token issuer
            //var issuerToken = Environment.GetEnvironmentVariable("IssuerToken");
            //var audience = Environment.GetEnvironmentVariable("Audience");
            var issuer = Environment.GetEnvironmentVariable("issuer");

            // Register the access token provider as a singleton
            //builder.Services.AddTransient<IAccessTokenProvider, AccessTokenProvider>(
            //    s => new AccessTokenProvider(issuerToken, audience, issuer));

            builder.Services.AddTransient<IAccessTokenProvider, OidcAccessTokenProvider>(x =>
             new OidcAccessTokenProvider(issuer, x.GetRequiredService<IHttpClientFactory>()));

            builder.Services.AddHttpClient();
        }
    }
}
