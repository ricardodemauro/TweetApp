using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetApp.Api.Middlewares.Authentication.AccessTokens
{
    public class TokenUser
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
