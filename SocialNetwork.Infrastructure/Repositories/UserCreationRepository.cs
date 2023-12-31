﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Contracts.Models;
using SocialNetwork.Contracts.Models.Authentication;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using SocialNetwork.Infrastructure.Interfaces.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SocialNetwork.Infrastructure.Repositories
{
    public class UserCreationRepository : IUserCreationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserCreationRepository> _logger;
        private readonly IEmailService _emailService;
        public UserCreationRepository(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<UserCreationRepository> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<string> CreateUserAsync(ApplicationUser user, RegisterRequest request)
        {
            var findUser = await _userManager.FindByEmailAsync(user.Email);
            if (findUser != null)
                return null;
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) 
            {
                _logger.LogWarning("User creation failed => {@result}", result);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            _logger.LogInformation("User successfully created, confirm your email => {@result}", result);
            return token;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            return users;
        }

        public async Task<bool> ConfirmEmailInf(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest user)
        {
            var userExist = await _userManager.FindByEmailAsync(user.Email);
            //var result = await _userManager.CheckPasswordAsync(userExist, user.Password);

            if (userExist != null && userExist.EmailConfirmed)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(userExist, user.Password, false, true);

                if (userExist.TwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(userExist, "Email");
                    var message = new Message(new string[] { userExist.Email }, "OTP Confirmation", token);
                    await _emailService.SendMessageAsync(message);

                    return new LoginResponse { Message = $"We have send OTP to {userExist.Email}" };
                } 
                else
                {
                    var tokens = await GetJwtTokenAsync(userExist.Email);

                    if (result.Succeeded && tokens != null)
                    {
                    
                        var loginResponse = new LoginResponse() { accessToken = tokens.Response.accessToken, refreshToken = tokens.Response.refreshToken};
                        _logger.LogInformation("Returning loginResponse => {@loginResponse}", loginResponse);

                        return loginResponse;
                    }
                    else
                    {
                        _logger.LogWarning("Can not sign in => {@result} || jwt is null => {@jwtToken}", result, tokens);
                        return null;
                    }
                }

            }

            _logger.LogWarning("User does not exist or email did not confirmed => {@userExist}", userExist);
            return null;
        }

        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                _logger.LogWarning("Can not find user");
                return new ApiResponse<LoginResponse>()
                {
                    IsSuccess = false,
                    Message = "Can not find user",
                    StatusCode = 0,
                    Response = null
                };
            }
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
                            ExpiryTokenDate = user.RefreshTokenExpiry
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

                if (user != null && refreshToken.Token == user.RefreshToken && user.RefreshTokenExpiry > DateTime.UtcNow)
                {
                    var token = await GetJwtTokenAsync(user.Email);

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
