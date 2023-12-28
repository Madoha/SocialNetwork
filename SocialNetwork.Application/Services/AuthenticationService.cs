using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.Models.Authentication;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using SocialNetwork.Infrastructure.Interfaces.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IUserCreationRepository _userCreationRepository;
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IEmailService _emailService;
        public AuthenticationService(IMapper mapper,
            IUserCreationRepository userCreationRepository,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthenticationService> logger,
            IUserRolesRepository userRolesRepository,
            IEmailService emailService)
        {
            _mapper = mapper;
            _userCreationRepository = userCreationRepository;
            _signInManager = signInManager;
            _logger = logger;
            _userRolesRepository = userRolesRepository;
            _emailService = emailService;
        }

        public async Task<ApiResponse<string>> RegisterUser(RegisterRequest request)
        {
            var userDto = _mapper.Map<ApplicationUser>(request);
            userDto.TwoFactorEnabled = request.TwoFactor;
            var result = await _userCreationRepository.CreateUserAsync(userDto, request);
            if (result is not null)
            {
                var roles = await _userRolesRepository.AddRolesToUser(userDto.Email, request.Roles);
                
                if (roles == null)
                    return new ApiResponse<string>() { StatusCode = 400, Message = "Failed while adding roles to user", IsSuccess = false, Response = null };

                //var userToResponse = _mapper.Map<RegisterResponse>(userDto);
                //userToResponse.Roles = roles;

                return new ApiResponse<string>()
                {
                    StatusCode = 200,
                    Message = $"User successfully created, Click a link to confirm your email in {request.Email}, WITHOUT CONFIRMATION YOU CAN NOT SIGN UP",
                    IsSuccess = true,
                    Response = result
                };
            }
            return new ApiResponse<string>() { StatusCode = 400, Message = "User creation failed.", IsSuccess = false, Response = null };
        }

        public async Task<ApiResponse<Message>> SendEmail(string link, string email)
        {
            if (link == null || email == null)
            {
                return new ApiResponse<Message>()
                {
                    IsSuccess = false,
                    StatusCode = 0,
                    Message = "one of the parameters null",
                    Response = null
                };
            }

            var message = new Message(new string[] { email! }, "Confirmation email link", link );
            await _emailService.SendMessageAsync(message);
            return new ApiResponse<Message>()
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "sended message",
                Response = message
            };
        }

        public async Task<bool> ConfirmEmailApp(string email, string token)
        {
            var user = await _userCreationRepository.ConfirmEmailInf(email, token);
            return user;
        }

        public async Task<ApiResponse<LoginResponse>> LoginUser(LoginRequest request)
        {
            //var userDto = _mapper.Map<ApplicationUser>(request);
            var result = await _userCreationRepository.LoginUserAsync(request);

            return result != null ?
                new ApiResponse<LoginResponse>()
                {
                    IsSuccess = true,
                    Message = "Sign in successfully",
                    StatusCode = 201,
                    Response = result
                } :
                new ApiResponse<LoginResponse>()
                {
                    IsSuccess = false,
                    Message = "Invalid login cred or user not found or email didnot confirm",
                    StatusCode = 300,
                    Response = result
                };
        }

        public async Task<ApiResponse<LoginResponse>> LoginWithOtpApp(string code, string email)
        {
            var signin = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signin != null)
            {
                var token = await _userCreationRepository.GetJwtTokenAsync(email);
                return token;
            }
            return null;
        }

        public async Task<ApiResponse<LoginResponse>> RefreshToken(LoginResponse tokens)
        {
            var result = await _userCreationRepository.RenewAccessToken(tokens);
            return result;

        }

        //public async Task<ApiResponse<List<RegisterResponse>>> GetAllUsers()
        //{
        //    var users = await _userRepository.GetAllUsersAsync();
        //    var registerResponseTasks = users.Select(async user =>
        //    {
        //        try
        //        {
        //            var roles = await _userRepository.GetUserRoles(user.Email);

        //            var userResponse =  new ApiResponse<RegisterResponse>()
        //            {
        //                IsSuccess = true,
        //                Message = "Return all users.",
        //                StatusCode = 201,
        //                Response = new()
        //                {
        //                    Id = user.Id,
        //                    Username = user.UserName,
        //                    Email = user.Email,
        //                    Roles = roles
        //                }
        //            };
        //            return new ApiResponse<List<userResponse>>();
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //    });
        //    return userResponse;
        //}
    }
}
