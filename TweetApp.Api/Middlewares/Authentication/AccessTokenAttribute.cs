using Microsoft.Azure.WebJobs.Description;
using System;

namespace TweetApp.Api.Middlewares.Authentication
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class AccessTokenAttribute : Attribute
    {

    }
}
