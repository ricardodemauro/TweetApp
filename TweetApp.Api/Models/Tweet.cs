using Microsoft.Azure.Cosmos.Table;
using System;

namespace TweetApp.Api.Models
{
    public class Tweet : TableEntity
    {
        public string Id => $"{RowKey}{PartitionKey}";

        public string Message { get; set; }

        public string User { get; set; }

        public string UserId { get; set; }

        public Tweet()
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = "tweets";
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
