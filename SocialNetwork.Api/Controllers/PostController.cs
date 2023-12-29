using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api.Models;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.AccountMani;
using System.Security.Claims;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("/add-post")]
        public async Task<IActionResult> AddPost(AddPost post)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var addPost = await _postService.AddPost(post, user);
            return addPost ? StatusCode(StatusCodes.Status201Created,
                new Response { IsSuccess = true, Message = "Added the post" })
                : StatusCode(StatusCodes.Status401Unauthorized, new Response { IsSuccess = false, Message = "Can not add the post" });

        }

        [HttpGet("/all-posts")]
        [AllowAnonymous]
        public async Task<IActionResult> AllPosts() // no DTOs
        {
            var posts = await _postService.AllPosts();

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
            var userData = await _postService.GetUserPosts(username);
            return userData.IsSuccess
                ? StatusCode(StatusCodes.Status200OK, userData.Response)
                : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = userData.Message });
        }

        [HttpGet("/user-posts/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(string postId) // change the returning result
        {
            var post = await _postService.GetPost(postId);
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
            var posts = await _postService.UserPostsByData(from, to, username);

            return posts != null ? Ok(posts) : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "User not found or someting" });
        }

        [HttpPost("/delete-post")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var result = await _postService.DeleteUserPost(postId, username);
            return result.IsSuccess ? StatusCode(StatusCodes.Status202Accepted, new Response { IsSuccess = true, Message = result.Message })
                : StatusCode(StatusCodes.Status400BadRequest, new Response { IsSuccess = false, Message = result.Message });
        }

        [HttpPut("/edit-post")]
        public async Task<IActionResult> EditPost([FromQuery] string postId, [FromBody] EditPost post)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var editPost = await _postService.EditPost(postId, username, post);

            return editPost.IsSuccess ? StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Message = editPost.Message })
                : StatusCode(StatusCodes.Status400BadRequest, new Response { IsSuccess = true, Message = editPost.Message });
        }
    }
}
