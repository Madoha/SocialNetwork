using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Domain.Models;
using Serilog;
using SocialNetwork.Contracts.Models.Authentication;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Api.Models;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.RegisterUser(request);

            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token = result.Response, email = request.Email }, Request.Scheme);
            var sendMessage = await _authenticationService.SendEmail(confirmationLink, request.Email);

            return result.IsSuccess ? Ok(result) : BadRequest();
        }

        [HttpGet("/confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var confirmation = await _authenticationService.ConfirmEmailApp(token, email);
            if (confirmation)
            {
                return StatusCode(StatusCodes.Status200OK,
                    new Response { IsSuccess = true, Message = "Email verified successfully" });
            }
            return StatusCode(StatusCodes.Status403Forbidden,
                new Response { IsSuccess = false, Message = "Unaccepted, try again" });
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _authenticationService.LoginUser(request);

            return result.IsSuccess ? Ok(result) : BadRequest();
        }

        [HttpPost("/login-otp")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
            var loginWithOTP = await _authenticationService.LoginWithOtpApp(code, email);
            return loginWithOTP.IsSuccess ? Ok(loginWithOTP) : BadRequest();
        }

        [HttpPost("/renew-refresh-token")]
        public async Task<ApiResponse<LoginResponse>> RenewRefreshToken(LoginResponse tokens)
        {
            var result = await _authenticationService.RefreshToken(tokens);
            return result;
        }

        [HttpPost("/reset-password")]
        public async Task<IActionResult> ResetPassword()
        {
            return null;
        }
        //[HttpGet("/get-users")]
        //public async Task<ApiResponse<List<RegisterResponse>>> getUsers()
        //{
        //    var result = await _authenticationService.GetAllUsers();
        //    return result;
        //}
    }
}
