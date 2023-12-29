using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api.Models;
using SocialNetwork.Application.Interfaces;
using System.Security.Claims;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;
        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpPost("/add-friends")]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var addFriendResult = await _friendService.AddFriendToMy(username, friendId);

            return addFriendResult ? StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Message = "Friend added" })
                : StatusCode(StatusCodes.Status404NotFound, new Response { IsSuccess = false, Message = "Something went wrong while adding, or user with friend id does not exist" });
        }

        [HttpGet("/my-friends")]
        public async Task<IActionResult> MyFriends()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var friends = await _friendService.GetMyFriendsApp(username);
            return friends != null ? Ok(friends) : BadRequest(ModelState);
        }
    }
}
