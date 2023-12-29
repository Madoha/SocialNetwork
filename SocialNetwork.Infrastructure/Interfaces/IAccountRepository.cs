using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces
{
    public interface IAccountRepository
    {
        Task<string> GetResetToken(string email);
        Task<bool> ResetPasswordInf(ResetPassword resetPassword);
        Task<ApplicationUser> UserSelfProfile(string username);
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
    }
}
