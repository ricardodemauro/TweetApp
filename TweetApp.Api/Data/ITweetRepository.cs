using System.Collections.Generic;
using System.Threading.Tasks;
using TweetApp.Api.Models;

namespace TweetApp.Api.Data
{
    public interface ITweetRepository
    {
        Task<Tweet> InsertOneAsync(Tweet data);

        Task<IReadOnlyList<Tweet>> List();
    }
}
