using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SocialNetwork.Infrastructure.Interfaces;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Infrastructure.Repositories;

namespace SocialNetwork.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        public PostService(IMapper mapper,
            IPostRepository postRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
        }

        public async Task<bool> AddPost(AddPost post, string username)
        {
            var rePost = _mapper.Map<Post>(post);
            var addPost = await _postRepository.AddPost(rePost, username);

            return addPost;
        }

        public async Task<List<Post>> AllPosts()
        {
            var posts = await _postRepository.AllPosts();

            return posts.Count == 0 ? null : posts;
        }

        public async Task<ApiResponse<List<PostDTO>>> GetUserPosts(string username)
        {
            var userPosts = await _postRepository.GetUserPosts(username);
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
            var post = await _postRepository.GetPost(postId);
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
            var result = await _postRepository.DeleteUserPost(postId, username);
            return result ? new ApiResponse<bool> { IsSuccess = true, Message = "Post removed", StatusCode = 200 }
            : new ApiResponse<bool>() { IsSuccess = false, StatusCode = 400, Message = "Can not remove this post, note: check if post id is correct" };
        }

        public async Task<List<Post>> UserPostsByData(DateTime? from, DateTime? to, string username)
        {
            var posts = await _postRepository.UserPostsByData(from, to, username);
            return posts;
        }

        public async Task<ApiResponse<Post>> EditPost(string postId, string username, EditPost post)
        {
            var postMapper = _mapper.Map<Post>(post);
            postMapper.Id = Guid.Parse(postId);

            var editPost = await _postRepository.EditPost(postId, username, postMapper);

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
    }
}
