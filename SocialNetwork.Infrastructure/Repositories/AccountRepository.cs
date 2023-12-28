using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit.Cryptography;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Models.AccountMani;
using SocialNetwork.Contracts.Models.AccountMani.AddPost;
using SocialNetwork.Database.Models;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountRepository> _logger;
        private readonly ApplicationDbContext _dbContext;
        public AccountRepository(UserManager<ApplicationUser> userManager,
            ILogger<AccountRepository> logger,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> GetResetToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found", user);
                return null;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ResetPasswordInf(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var response = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (response.Succeeded)
                    return true;
            }
            _logger.LogWarning($"Something went wrong while Reset password: {resetPassword.Token}");
            return false;

        }

        public async Task<bool> AddPost(Post post, string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return false;

            post.UserId = user.Id;
            var result = await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task<List<Post>> AllPosts()
        {
            return await _dbContext.Posts.ToListAsync();
        }
        public async Task<List<Post>> GetUserPosts(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return null;

            var userPosts = await _dbContext.Posts.Where(u => u.UserId == user.Id).ToListAsync();

            return userPosts;
        }
        public async Task<bool> DeleteUserPost(string postId, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            //var postExists = await _dbContext.Posts.Where(p => p.Id.ToString() == postId).FirstOrDefaultAsync();
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id.ToString() == postId && p.UserId == user.Id);
            if (post != null)
            {
                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Post>> UserPostsByData(DateTime? from, DateTime? to, string username)
        {
            //var projects = _dbContext.Posts.Include(p => p.User).AsQueryable();
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return null;

            var projects = _dbContext.Posts.Where(p => p.UserId == user.Id).AsQueryable();
            if (from != null)
            {
                projects = projects.Where(p => p.WasCreated > from);
            } if (to != null)
            {
                projects = projects.Where(p => p.WasCreated < to);
            }

            return await projects.ToListAsync();
        }

        public async Task<Post> EditPost(string postId, string username, Post post)
        {
            var user = await _userManager.FindByNameAsync(username);
            post.UserId = user.Id;
            post.User = user;

            //var postToEdit = await _dbContext.Posts.AsNoTracking().SingleOrDefaultAsync(p => p.Id.ToString() == postId);
            var postToEdit = await _dbContext.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id.ToString() == postId && p.UserId == user.Id);

            if (postToEdit != null)
            {
                post.WasCreated = postToEdit.WasCreated;

                _dbContext.Posts.Update(post);
                await _dbContext.SaveChangesAsync();

                //return await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id.ToString() == postId);
                return post;
            }

            return null;
        }

        public async Task<Post> GetPost(string postId)
        {
            //var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id.ToString() == postId);
            var post = await _dbContext.Posts.Where(p => p.Id.ToString() == postId).Include(p => p.Comments).FirstOrDefaultAsync();

            return post;
        }

        public async Task<ApplicationUser> UserSelfProfile(string username)
        {
            var userData = await _userManager.FindByNameAsync(username);
            return userData;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<bool> PostComment(Comment commentToPost)
        {
            var commentDescription = await _userManager.FindByNameAsync(commentToPost.Username);
            commentToPost.User = commentDescription;
            commentToPost.Post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == commentToPost.PostId);
            commentToPost.UserId = commentDescription.Id;

            var result = await _dbContext.Comments.AddAsync(commentToPost);
            var post = await _dbContext.Posts.Where(p => p.Id == commentToPost.PostId).FirstOrDefaultAsync();
            post.CommentCount++;

            await _dbContext.SaveChangesAsync();
            
            return result != null ? true : false;
        }

        public async Task<ApplicationUser> GetUserByName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> AddFriendToMy(ApplicationUser user, ApplicationUser friendExist)
        {
            var friendShip = new Friendship
            {
                User1Id = user.Id,
                User2Id = friendExist.Id,
                User1 = user,
                User2 = friendExist,
            };

            await _dbContext.Friendships.AddAsync(friendShip);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0 ? true : false;
        }

        public async Task<List<String>> GetMyFriendsInf(string username)
        {
            var user = await GetUserByName(username);
            var userInfo = await _dbContext.Friendships.Where(u => u.User1Id == user.Id).ToListAsync();

            List<String> users = new();
            foreach (var val in userInfo)
                users.Add(val.User2Id);


            return users;
        }
    }
}
