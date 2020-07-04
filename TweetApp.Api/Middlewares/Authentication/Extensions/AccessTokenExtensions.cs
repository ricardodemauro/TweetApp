namespace TweetApp.Api.Middlewares.Authentication.Extensions
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using TweetApp.Api.Middlewares.Authentication.AccessTokens;

    /// <summary>
    /// Called from Startup to load the custom binding when the Azure Functions host starts up.
    /// </summary>
    public static class AccessTokenExtensions
    {
        public static IWebJobsBuilder AddAccessTokenBinding(this IWebJobsBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.AddExtension<AccessTokenExtensionProvider>();

            // Get the configuration files for the OAuth token issuer
            //var issuerToken = Environment.GetEnvironmentVariable("IssuerToken");
            //var audience = Environment.GetEnvironmentVariable("Audience");
            var issuer = Environment.GetEnvironmentVariable("Issuer");

            // Register the access token provider as a singleton
            //builder.Services.AddTransient<IAccessTokenProvider, AccessTokenProvider>(
            //    s => new AccessTokenProvider(issuerToken, audience, issuer));
            builder.Services.AddTransient<IAccessTokenProvider, OidcAccessTokenProvider>(x =>
                new OidcAccessTokenProvider(issuer, x.GetRequiredService<IHttpClientFactory>()));

            return builder;
        }
    }
}
