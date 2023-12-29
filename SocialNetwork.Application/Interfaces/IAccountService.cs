using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<string>> GetResetToken(string email);
        Task<ApiResponse<Message>> SendEmailReset(string link, string email);
        Task<ApiResponse<bool>> ResetPasswordApp(ResetPassword resetPassword);
        Task<ApiResponse<UserDTO>> UserSelfProfile(string username);
        Task<IEnumerable<UserDTO>> GetAllUsers();
    }
}
