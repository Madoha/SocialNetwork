using SocialNetwork.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface IFriendService
    {
        Task<bool> AddFriendToMy(string myUsername, string friendId);
        Task<List<UserDTO>> GetMyFriendsApp(string username);
    }
}
