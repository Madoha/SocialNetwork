using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api.Models;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.DTOs;
using System.Security.Claims;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("/enter-comment-to-post")]
        public async Task<IActionResult> PostComment([FromQuery] string postId, [FromBody] CommentDTO comment)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var addComment = await _commentService.PostComment(postId, username, comment);

            return addComment ? StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Message = "Comment added" })
                : StatusCode(StatusCodes.Status400BadRequest, new Response { IsSuccess = false, Message = "Can not add comment" });
        }
    }
}
