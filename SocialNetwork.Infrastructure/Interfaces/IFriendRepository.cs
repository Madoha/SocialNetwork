using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces
{
    public interface IFriendRepository
    {
        Task<bool> AddFriendToMy(ApplicationUser user, ApplicationUser friendExist);
        Task<List<String>> GetMyFriendsInf(string username);
        Task<ApplicationUser> GetUserByName(string username);
        Task<ApplicationUser> GetUserById(string id);
    }
}
