using AutoMapper;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Services
{
    public class FriendService : IFriendService
    {
        private readonly IMapper _mapper;
        private readonly IFriendRepository _friendRepository;
        public FriendService(IMapper mapper,
            IFriendRepository friendRepository)
        {
            _friendRepository = friendRepository;
            _mapper = mapper;

        }

        public async Task<bool> AddFriendToMy(string myUsername, string friendId)
        {
            var user = await _friendRepository.GetUserByName(myUsername);
            var friendExist = await _friendRepository.GetUserById(friendId);

            if (user == null || friendExist == null)
                return false;

            var result = await _friendRepository.AddFriendToMy(user, friendExist);
            return result;
        }

        public async Task<List<UserDTO>> GetMyFriendsApp(string username)
        {
            // list of friends id
            var friendsId = await _friendRepository.GetMyFriendsInf(username);

            if (friendsId == null)
                return null;

            List<ApplicationUser> friendsUser = new();

            foreach (var friend in friendsId)
                friendsUser.Add(await _friendRepository.GetUserById(friend));

            List<UserDTO> userDTOs = _mapper.Map<List<UserDTO>>(friendsUser);

            return userDTOs;
        }
    }
}
