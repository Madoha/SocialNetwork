using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Contracts.Authentication;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Domain.Models;
using SocialNetwork.Contracts;
using Serilog;

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
            
            return result.IsSuccess ? Ok(result) : BadRequest();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _authenticationService.LoginUser(request);

            return result.IsSuccess ? Ok(result) : BadRequest();
        }

        [HttpPost("/renew-refresh-token")]
        public async Task<ApiResponse<LoginResponse>> RenewRefreshToken(LoginResponse tokens)
        {
            var result = await _authenticationService.RefreshToken(tokens);
            return result;
        }

        [HttpGet]
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
