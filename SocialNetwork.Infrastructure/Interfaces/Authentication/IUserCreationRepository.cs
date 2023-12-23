using SocialNetwork.Contracts.Models.Authentication;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces.Authentication
{
    public interface IUserCreationRepository
    {
        Task<string> CreateUserAsync(ApplicationUser user, RegisterRequest request);
        Task<List<ApplicationUser>> GetAllUsersAsync();

        Task<bool> ConfirmEmailInf(string token, string email);
        Task<LoginResponse> LoginUserAsync(LoginRequest user);
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
        Task<ApiResponse<LoginResponse>> RenewAccessToken(LoginResponse loginResponse);
    }
}
