using SocialNetwork.Contracts;
using SocialNetwork.Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<RegisterResponse>> RegisterUser(RegisterRequest request);
        //Task<ApiResponse<List<RegisterResponse>>> GetAllUsers();
        Task<ApiResponse<LoginResponse>> LoginUser(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshToken(LoginResponse tokens);
    }
}
