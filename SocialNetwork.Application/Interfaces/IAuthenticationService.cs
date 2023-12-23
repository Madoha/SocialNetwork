using SocialNetwork.Contracts.Models.Authentication;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<string>> RegisterUser(RegisterRequest request);
        //Task<ApiResponse<List<RegisterResponse>>> GetAllUsers();
        Task<bool> ConfirmEmailApp(string token, string email);
        Task<ApiResponse<Message>> SendEmail(string link, string email);
        Task<ApiResponse<LoginResponse>> LoginUser(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshToken(LoginResponse tokens);
    }
}
