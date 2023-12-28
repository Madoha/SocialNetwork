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

        [HttpPost("/add-post")]
        public async Task<IActionResult> AddPost(AddPost post)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var addPost = await _accountService.AddPost(post, user);
            return addPost ? StatusCode(StatusCodes.Status201Created,
                new Response { IsSuccess = true, Message = "Added the post" })
                : StatusCode(StatusCodes.Status401Unauthorized, new Response { IsSuccess = false, Message = "Can not add the post" }); 

        }

        [HttpGet("/all-posts")]
        [AllowAnonymous]
        public async Task<IActionResult> AllPosts() // no DTOs
        {
            var posts = await _accountService.AllPosts();

            if (posts == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "There is no posts" });
            }

            return StatusCode(StatusCodes.Status200OK, posts);
        } 

        [HttpGet("/user-self-posts")]
        public async Task<IActionResult> UserSelfPosts()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var userData = await _accountService.GetUserPosts(username);
            return userData.IsSuccess
                ? StatusCode(StatusCodes.Status200OK, userData.Response)
                : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = userData.Message });
        }

        [HttpGet("/user-posts/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(string postId) // change the returning result
        {
            var post = await _accountService.GetPost(postId);
            return post.IsSuccess ? StatusCode(StatusCodes.Status200OK, post.Response)
                : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "Can not find && return post", Status = "404" });
        }

        /// <summary>
        /// See user posts in entered time
        /// </summary>
        /// <param name="from">2023-12-26 00:13:18.085876+06</param>
        /// <param name="to">2023-12-26 00:13:38.188112+06</param>
        /// <returns>List of user posts</returns>
        [HttpGet("/posts-by-date")]
        public async Task<IActionResult> UserPostsByDate(DateTime? from, DateTime? to)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var posts = await _accountService.UserPostsByData(from, to, username);

            return posts != null ? Ok(posts) : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "User not found or someting" });
        }

        [HttpPost("/delete-post")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var result = await _accountService.DeleteUserPost(postId, username);
            return result.IsSuccess ? StatusCode(StatusCodes.Status202Accepted, new Response { IsSuccess = true, Message = result.Message})
                : StatusCode(StatusCodes.Status400BadRequest, new Response { IsSuccess = false, Message =  result.Message });
        }

        [HttpPut("/edit-post")]
        public async Task<IActionResult> EditPost([FromQuery]string postId, [FromBody] EditPost post)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var editPost = await _accountService.EditPost(postId, username, post);

            return editPost.IsSuccess ? StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Message = editPost.Message })
                : StatusCode(StatusCodes.Status400BadRequest, new Response { IsSuccess =true, Message = editPost.Message });
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
            return Ok(users);
        }

        [HttpPost("/enter-comment-to-post")]
        public async Task<IActionResult> PostComment([FromQuery] string postId,[FromBody] CommentDTO comment)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var addComment = await _accountService.PostComment(postId, username, comment);

            return addComment ? StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Message = "Comment added" })
                : StatusCode(StatusCodes.Status400BadRequest, new Response { IsSuccess = false, Message = "Can not add comment" });
        }

        [HttpGet("/see-post-comments/{postId}")]
        public async Task<IActionResult> SeePostComments(string postId)
        {
            var post = await _accountService.GetPost(postId);

            return post.IsSuccess ? StatusCode(StatusCodes.Status200OK, post.Response)
                : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "Can not find && return post", Status = "404" });
        }

        [HttpPost("/add-friends")]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var addFriendResult = await _accountService.AddFriendToMy(username, friendId);

            return addFriendResult ? Ok(addFriendResult) : BadRequest();
        }

        [HttpGet("/my-friends")]
        public async Task<IActionResult> MyFriends()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var friends = await _accountService.GetMyFriendsApp(username);
            return Ok(friends);
        }
        // after frontend
        //[HttpPost("/upload-image")]
        //public async Task<IActionResult> UploadPhoto(IFormFile file)
        //{
        //    var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        //    var result = await _photoService.AddPhotoAsync(file);
        //}
    }
}
