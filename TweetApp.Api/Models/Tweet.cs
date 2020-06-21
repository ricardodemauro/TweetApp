using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TweetApp.Api.Models
{
    public class Tweet
    {
        public ObjectId Id { get; set; }

        public DateTime Created { get; set; }

        public string Message { get; set; }

        public string User { get; set; }
    }
}
