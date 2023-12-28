using AutoMapper;
using Microsoft.Extensions.Logging;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.EmailServiceModels;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
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
        private readonly IMapper _mapper;
        public AccountService(IAccountRepository accountRepository, 
            IEmailService emailService,
            ILogger<AccountService> logger,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _logger = logger;
            _mapper = mapper;
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
            if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(email))
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

        public async Task<bool> AddPost(AddPost post, string username)
        {
            var rePost = _mapper.Map<Post>(post);
            var addPost = await _accountRepository.AddPost(rePost, username);

            return addPost;
        }

        public async Task<List<Post>> AllPosts()
        {
            var posts = await _accountRepository.AllPosts();

            return posts.Count == 0 ? null : posts;
        }

        public async Task<ApiResponse<List<PostDTO>>> GetUserPosts(string username)
        {
            var userPosts = await _accountRepository.GetUserPosts(username);
            var userPostsDto = _mapper.Map<List<PostDTO>>(userPosts);
            return userPosts != null ? new ApiResponse<List<PostDTO>>
            {
                IsSuccess = true,
                Message = "User posts",
                StatusCode = 200,
                Response = userPostsDto,
            } : new ApiResponse<List<PostDTO>>
            {
                IsSuccess = false,
                Message = "Can not return user posts(returning null)",
                StatusCode = 400,
                Response = userPostsDto,
            };
        }

        public async Task<ApiResponse<Post>> GetPost(string postId)
        {
            var post = await _accountRepository.GetPost(postId);
            return post != null ? new ApiResponse<Post>
            {
                IsSuccess = true,
                Message = "returning post",
                Response = post
            } : new ApiResponse<Post>
            {
                IsSuccess = false,
                Message = "can not return this post",
                Response = post
            };
        }

        public async Task<ApiResponse<bool>> DeleteUserPost(string postId, string username)
        {
            var result = await _accountRepository.DeleteUserPost(postId, username);
            return result ? new ApiResponse<bool> { IsSuccess = true, Message = "Post removed", StatusCode = 200 }
            : new ApiResponse<bool>() { IsSuccess = false, StatusCode = 400, Message = "Can not remove this post, note: check if post id is correct" };
        }

        public async Task<List<Post>> UserPostsByData(DateTime? from, DateTime? to, string username)
        {
            var posts = await _accountRepository.UserPostsByData(from, to, username);
            return posts;
        }

        public async Task<ApiResponse<Post>> EditPost(string postId, string username, EditPost post)
        {
            var postMapper = _mapper.Map<Post>(post);
            postMapper.Id = Guid.Parse(postId);

            var editPost = await _accountRepository.EditPost(postId, username, postMapper);

            return editPost != null ? new ApiResponse<Post>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Post edited",
                Response = editPost
            } : new ApiResponse<Post>()
            {
                IsSuccess = false,
                Message = "Can not edit post",
                StatusCode = 400,
                Response = editPost
            };
        }

        public async Task<ApiResponse<UserDTO>> UserSelfProfile(string username)
        {
            var userData = await _accountRepository.UserSelfProfile(username);
            var userDto = _mapper.Map<UserDTO>(userData);

            if (userData != null)
            {
                var userPosts = await GetUserPosts(username);
                userDto.Posts = userPosts.Response;
                return new ApiResponse<UserDTO>
                {
                    IsSuccess = true,
                    Message = "User profile",
                    StatusCode = 200,
                    Response = userDto
                };
            }

            return new ApiResponse<UserDTO>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "User does not exist or something",
                Response = null
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _accountRepository.GetAllUsers();

            if (users == null)
                return null;

            var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
            return usersDto;
        }

        public async Task<bool> PostComment(string postId, string username, CommentDTO comment)
        {
            var commentToPost = _mapper.Map<Comment>(comment);
            commentToPost.PostId = Guid.Parse(postId);
            commentToPost.Username = username;

            var result = await _accountRepository.PostComment(commentToPost);
            return result;
        }

        public async Task<bool> AddFriendToMy(string myUsername, string friendId)
        {
            var user = await _accountRepository.GetUserByName(myUsername);
            var friendExist = await _accountRepository.GetUserById(friendId);

            if (user == null || friendExist == null)
                return false;

            var result = await _accountRepository.AddFriendToMy(user, friendExist);
            return result;
        }

        public async Task<List<UserDTO>> GetMyFriendsApp(string username)
        {
            // list of friends id
            var friendsId = await _accountRepository.GetMyFriendsInf(username);

            if (friendsId == null)
                return null;

            List<ApplicationUser> friendsUser = new();
            
            foreach (var friend in friendsId)
                friendsUser.Add(await _accountRepository.GetUserById(friend));

            List<UserDTO> userDTOs = _mapper.Map<List<UserDTO>>(friendsUser);

            return userDTOs;
        }
    }
}
