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
using System.Security.Cryptography;
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
                var tokens = await GetJwtTokenAsync(userExist);

                if (result.Succeeded && tokens != null)
                {
                    
                    var loginResponse = new LoginResponse() { accessToken = tokens.Response.accessToken, refreshToken = tokens.Response.refreshToken };
                    _logger.LogInformation("Returning loginResponse => {@loginResponse}", loginResponse);

                    return loginResponse;
                }
                else
                {
                    _logger.LogWarning("Can not sign in => {@result} || jwt is null => {@jwtToken}", result, tokens);
                    return null;
                }
            }

            _logger.LogWarning("User does not exist => {@userExist}", userExist);
            return null;
        }

        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var jwtToken = await GenerateJwtToken(claims);
                var refreshToken = await GenerateRefreshToken();

                _ = int.TryParse(_configuration["JwtSettings:RefreshTokenValidity"], out int refreshTokenValidity);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenValidity);

                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Returning tokens => {}");
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Returning tokens",
                    Response = new LoginResponse
                    {
                        accessToken = new TokenType
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                            ExpiryTokenDate = jwtToken.ValidTo
                        },

                        refreshToken = new TokenType
                        {
                            Token = user.RefreshToken,
                            ExpiryTokenDate = (DateTime)user.RefreshTokenExpiry
                        }
                    }
                };
            }

            return null;
        }
        public async Task<ApiResponse<LoginResponse>> RenewAccessToken(LoginResponse loginResponse)
        {
            var accessToken = loginResponse.accessToken;
            var refreshToken = loginResponse.refreshToken;

            var principal = await GetClaimsPrincipal(accessToken.Token);

            if(principal != null && principal.Identity != null && !string.IsNullOrEmpty(principal.Identity.Name)) 
            {
                var user = await _userManager.FindByNameAsync(principal.Identity.Name);

                if (user != null && refreshToken.Token == user.RefreshToken && refreshToken.ExpiryTokenDate > DateTime.UtcNow)
                {
                    var token = await GetJwtTokenAsync(user);

                    _logger.LogInformation("Returning token => {@token}", token);
                    return token;
                }
            }

            _logger.LogWarning("Invalid principal or user not found");
            return new ApiResponse<LoginResponse>
            {
                IsSuccess = false,
                Message = "Invalid principal or user not found",
                StatusCode = 400,
            };
        }


        #region PrivateMethods
        private async Task<JwtSecurityToken> GenerateJwtToken(List<Claim> claims)
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
        
        private async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        
        private async Task<ClaimsPrincipal> GetClaimsPrincipal(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"])),
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            _logger.LogInformation("Validate token => @{principal}");

            return principal;
        }
        #endregion
    }
}
