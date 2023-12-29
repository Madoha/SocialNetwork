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
    public class FriendRepository : IFriendRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public FriendRepository(UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext; 
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
            if (user == null)
                return null;

            var userInfo = await _dbContext.Friendships.Where(u => u.User1Id == user.Id).ToListAsync();

            List<String> users = new();
            foreach (var val in userInfo)
                users.Add(val.User2Id);


            return users;
        }
        public async Task<ApplicationUser> GetUserByName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}
