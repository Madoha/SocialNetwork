﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; } = "0";
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
