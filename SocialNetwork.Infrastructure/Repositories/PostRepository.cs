using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Database.Models;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public PostRepository(UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext) 
        {
            _userManager = userManager;
            _dbContext = dbContext; 
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
            }
            if (to != null)
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
    }
}
