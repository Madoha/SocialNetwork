using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api.Models;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using System.Security.Claims;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService,
            IPhotoService photoService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("/forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        { 
            var token = await _accountService.GetResetToken(email);
            if (!token.IsSuccess)
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { IsSuccess=token.IsSuccess, Message=token.Message });

            var resetLink = Url.Action(nameof(ResetPassword), "Account", new { token = token.Response, email = email }, Request.Scheme);
            var result = await _accountService.SendEmailReset(resetLink, email);

            if (result.IsSuccess)
                return StatusCode(StatusCodes.Status200OK,
                    new Response { IsSuccess = true, Message = "Check your email" });

            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpGet("/reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var obj = new ResetPassword { Token = token, Email = email };
            return Ok(obj);
        }

        [HttpPost("/reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var result = await _accountService.ResetPasswordApp(resetPassword);
            if (result == null || !result.Response)
            {
                return BadRequest(ModelState);
            }
            return StatusCode(StatusCodes.Status200OK,
                new Response { IsSuccess = true, Message = "password has been changed" });
        }

        [HttpGet("/user-self-profile")]
        public async Task<IActionResult> UserSelfProfile()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userData = await _accountService.UserSelfProfile(username);
            return userData.IsSuccess ? StatusCode(StatusCodes.Status200OK, userData.Response)
                : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "User does not exist or something" });
        }

        [HttpGet("/get-all-users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _accountService.GetAllUsers();
            return users != null ? Ok(users) : BadRequest(ModelState);
        }
    }
}
