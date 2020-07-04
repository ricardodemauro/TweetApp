using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

namespace TweetApp.Api.Logging
{
    public static class LoggerExtesions
    {
        public static void LogError<T>(this ILogger<T> logger, Exception ex, [CallerMemberName] string caller = null)
        {
            logger.LogError(ex, "Error when trying to execute {method}", caller);
        }
    }
}
