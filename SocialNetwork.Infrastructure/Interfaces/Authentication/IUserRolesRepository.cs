using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces.Authentication
{
    public interface IUserRolesRepository
    {
        Task<List<string>> AddRolesToUser(string Email, List<string> Roles);
        Task<List<string>> GetUserRoles(string email);
    }
}
