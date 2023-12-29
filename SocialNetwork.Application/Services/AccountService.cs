using AutoMapper;
using Microsoft.Extensions.Logging;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;
        public AccountService(IAccountRepository accountRepository, 
            IEmailService emailService,
            ILogger<AccountService> logger,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<string>> GetResetToken(string email)
        {
            var token = await _accountRepository.GetResetToken(email);
            if (token == null)
            {
                return new ApiResponse<string>()
                {
                    IsSuccess = false,
                    Message = "User not found or token is null",
                    StatusCode = 500,
                    Response = token
                };
            }
            return new ApiResponse<string>
            {
                IsSuccess = true,
                Message = "returning reset token",
                StatusCode = 200,
                Response = token
            };
        }

        public async Task<ApiResponse<Message>> SendEmailReset(string link, string email)
        {
            if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Link or email is empty", link, email);
                return new ApiResponse<Message>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Link or email is empty",
                    Response = null
                };
            }

            var message = new Message(new string[] { email }, "Reset password", link);
            await _emailService.SendMessageAsync(message);

            return new ApiResponse<Message>
            {
                IsSuccess = true,
                Message = "Sended reset link",
                StatusCode = 200,
                Response = message
            };

        }
        public async Task<ApiResponse<bool>> ResetPasswordApp(ResetPassword resetPassword)
        {
            var tokenResult = await _accountRepository.ResetPasswordInf(resetPassword);
            if (tokenResult)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "changed the password",
                    Response = tokenResult
                };
            }
            return new ApiResponse<bool>()
            { 
                IsSuccess = false,
                StatusCode = 0,
                Message = "Can not change the password",
                Response = false
            };
        }

        public async Task<ApiResponse<UserDTO>> UserSelfProfile(string username)
        {
            var userData = await _accountRepository.UserSelfProfile(username);
            var userDto = _mapper.Map<UserDTO>(userData);

            if (userData != null)
            {
                //var userPosts = await GetUserPosts(username);
                //userDto.Posts = userPosts.Response;
                return new ApiResponse<UserDTO>
                {
                    IsSuccess = true,
                    Message = "User profile",
                    StatusCode = 200,
                    Response = userDto
                };
            }

            return new ApiResponse<UserDTO>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "User does not exist or something",
                Response = null
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _accountRepository.GetAllUsers();

            if (users == null)
                return null;

            var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
            return usersDto;
        }
    }
}
