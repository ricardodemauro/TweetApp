using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetApp.Api.Data
{
    public static class MongoDbFactory
    {
        public static IMongoCollection<T> Get<T>(string collectionName)
        {
            var connString = Environment.GetEnvironmentVariable("MongoConnectionString");
            var db = new MongoClient(connString);
            return db.GetDatabase("tweets").GetCollection<T>(collectionName);
        }
    }
}
