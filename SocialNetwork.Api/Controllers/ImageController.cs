using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        // after frontend
        //[HttpPost("/upload-image")]
        //public async Task<IActionResult> UploadPhoto(IFormFile file)
        //{
        //    var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        //    var result = await _photoService.AddPhotoAsync(file);
        //}
    }
}
