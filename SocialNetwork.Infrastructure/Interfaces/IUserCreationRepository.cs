using SocialNetwork.Contracts;
using SocialNetwork.Contracts.Authentication;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces
{
    public interface IUserCreationRepository
    {
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, RegisterRequest request);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<LoginResponse> LoginUserAsync(LoginRequest user);
        Task<TokenType> GetJwtTokenAsync(ApplicationUser user);
    }
}
