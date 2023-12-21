using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Repositories
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly ILogger<UserRolesRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesRepository(RoleManager<IdentityRole> roleManager,
            ILogger<UserRolesRepository> logger,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public async Task<List<string>> AddRolesToUser(string Email, List<string> Roles)
        {
            List<string> assignedRoles = new();
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                _logger.LogWarning("Can not find user while Adding roles to user => {@user}", user);
                return null;
            }

            foreach (var role in Roles)
            {
                if (await _roleManager.RoleExistsAsync(role) && !await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                    assignedRoles.Add(role);
                }
            }
            _logger.LogInformation("Successfully added roles to user");
            return assignedRoles;
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            List<string> userRoles = new();

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                foreach (var role in roles)
                    userRoles.Add(role);

                _logger.LogInformation("Iterating all user roles => {@roles}", roles);
                return userRoles;
            }

            _logger.LogWarning("User not found => {@user}", user);
            return null;
        }
    }
}
