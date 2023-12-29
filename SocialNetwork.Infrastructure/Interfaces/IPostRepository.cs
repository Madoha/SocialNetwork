﻿using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces
{
    public interface IPostRepository
    {
        Task<bool> AddPost(Post post, string username);
        Task<List<Post>> AllPosts();
        Task<List<Post>> GetUserPosts(string username);
        Task<Post> GetPost(string postId);
        Task<bool> DeleteUserPost(string postId, string username);
        Task<List<Post>> UserPostsByData(DateTime? from, DateTime? to, string username);
        Task<Post> EditPost(string postId, string username, Post post);
    }
}
