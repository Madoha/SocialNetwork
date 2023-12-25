using SocialNetwork.Contracts.Models;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
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
    }
}
