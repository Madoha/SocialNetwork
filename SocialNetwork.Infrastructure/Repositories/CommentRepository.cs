using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class CommentRepository : ICommentRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public CommentRepository(UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext; 
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
    }
}
