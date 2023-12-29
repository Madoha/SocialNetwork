﻿using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.Response;
using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface IPostService
    {
        Task<bool> AddPost(AddPost post, string username);
        Task<List<Post>> AllPosts();
        Task<List<Post>> UserPostsByData(DateTime? from, DateTime? to, string username);
        Task<ApiResponse<List<PostDTO>>> GetUserPosts(string username);
        Task<ApiResponse<bool>> DeleteUserPost(string postId, string username);
        Task<ApiResponse<Post>> EditPost(string postId, string username, EditPost post);
        Task<ApiResponse<Post>> GetPost(string postId);
        
    }
}
