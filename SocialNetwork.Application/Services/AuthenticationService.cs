using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts;
using SocialNetwork.Contracts.Authentication;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
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
        public AuthenticationService(IMapper mapper,
            IUserCreationRepository userCreationRepository,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthenticationService> logger,
            IUserRolesRepository userRolesRepository)
        {
            _mapper = mapper;
            _userCreationRepository = userCreationRepository;
            _signInManager = signInManager;
            _logger = logger;
            _userRolesRepository = userRolesRepository;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterUser(RegisterRequest request)
        {
            var userDto = _mapper.Map<ApplicationUser>(request);
            var result = await _userCreationRepository.CreateUserAsync(userDto, request);
            if (result is not null)
            {
                var roles = await _userRolesRepository.AddRolesToUser(result.Email, request.Roles);
                
                if (roles == null)
                    return new ApiResponse<RegisterResponse>() { StatusCode = 400, Message = "Failed while adding roles to user", IsSuccess = false, Response = null };

                var userToResponse = _mapper.Map<RegisterResponse>(userDto);
                userToResponse.Roles = roles;

                return new ApiResponse<RegisterResponse>()
                {
                    StatusCode = 200,
                    Message = "User successfully created",
                    IsSuccess = true,
                    Response = userToResponse
                };
            }
            return new ApiResponse<RegisterResponse>() { StatusCode = 400, Message = "User creation failed.", IsSuccess = false, Response = null };
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
                    Message = "Invalid login cred or user not found",
                    StatusCode = 300,
                    Response = result
                };
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
