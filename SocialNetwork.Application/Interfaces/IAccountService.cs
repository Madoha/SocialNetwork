using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<string>> GetResetToken(string email);
        Task<ApiResponse<Message>> SendEmailReset(string link, string email);
        Task<ApiResponse<bool>> ResetPasswordApp(ResetPassword resetPassword);
        Task<bool> AddPost(AddPost post, string username);
        Task<List<Post>> AllPosts();
        Task<List<Post>> UserPostsByData(DateTime? from, DateTime? to, string username);
        Task<ApiResponse<List<PostDTO>>> GetUserPosts(string username);
        Task<ApiResponse<bool>> DeleteUserPost(string postId, string username);
        Task<ApiResponse<Post>> EditPost(string postId, string username, EditPost post);
        Task<ApiResponse<Post>> GetPost(string postId);
        Task<ApiResponse<UserDTO>> UserSelfProfile(string username);
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<bool> PostComment(string postId, string username, CommentDTO comment);
        Task<bool> AddFriendToMy(string myUsername, string friendId);
        Task<List<UserDTO>> GetMyFriendsApp(string username);
    }
}
