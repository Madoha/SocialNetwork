using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MimeKit.Cryptography;
using SocialNetwork.Contracts.Models;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountRepository> _logger;
        public AccountRepository(UserManager<ApplicationUser> userManager,
            ILogger<AccountRepository> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GetResetToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found", user);
                return null;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ResetPasswordInf(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var response = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (response.Succeeded)
                    return true;
            }
            return false;

        }
    }
}
