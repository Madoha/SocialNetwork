using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Contracts;
using SocialNetwork.Contracts.Authentication;
using SocialNetwork.Database.Models;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Repositories
{
    public class UserCreationRepository : IUserCreationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserCreationRepository> _logger;
        public UserCreationRepository(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<UserCreationRepository> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, RegisterRequest request)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) 
            {
                _logger.LogWarning("User creation failed => {@result}", result);
            }
            _logger.LogInformation("User successfully created => {@result}", result);
            return result.Succeeded ? user : null;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            return users;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest user)
        {
            var userExist = await _userManager.FindByEmailAsync(user.Email);
            //var result = await _userManager.CheckPasswordAsync(userExist, user.Password);

            if (userExist != null)
            {
                var result = await _signInManager.PasswordSignInAsync(userExist, user.Password, false, false);
                var jwtToken = await GetJwtTokenAsync(userExist);

                if (result.Succeeded && jwtToken != null)
                {
                    
                    var loginResponse = new LoginResponse() { accessToken = jwtToken, refreshToken = null };
                    _logger.LogInformation("Returning loginResponse => {@loginResponse}", loginResponse);

                    return loginResponse;
                }
                else
                {
                    _logger.LogWarning("Can not sign in => {@result} || jwt is null => {@jwtToken}", result, jwtToken);
                    return null;
                }
            }

            _logger.LogWarning("User does not exist => {@userExist}", userExist);
            return null;
        }

        public async Task<TokenType> GetJwtTokenAsync(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var jwtToken = GenerateToken(claims);

                return new TokenType
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    ExpiryTokenDate = jwtToken.ValidTo
                };
            }

            return null;
        }

        #region PrivateMethods
        private JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var _ = int.TryParse(_configuration["JwtSettings:TokenValidityInMinutes"], out int tokenValidityInMinutes);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            _logger.LogInformation("Returning generated jwtoken => {@token}", token);
            return token;
        }
        #endregion
    }
}
