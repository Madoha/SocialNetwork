using Microsoft.Extensions.Logging;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.Models;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
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
        public AccountService(IAccountRepository accountRepository, 
            IEmailService emailService,
            ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _logger = logger;
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
            if (link == null || email == null)
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
    }
}
