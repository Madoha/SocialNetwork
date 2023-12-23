using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Contracts.Models;

namespace SocialNetwork.Contracts.Models.Authentication
{
    public class LoginResponse
    {
        public TokenType accessToken { get; set; }
        public TokenType refreshToken { get; set; }
    }
}
