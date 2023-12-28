using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Domain.Models
{
    public class Friendship
    {
        // fk
        public string User1Id { get; set; }
        public string User2Id { get; set; }

        public DateTime FriendshipDate { get; set; } = DateTime.UtcNow;

        // navigation
        public ApplicationUser User1 { get; set; }
        public ApplicationUser User2 { get; set; }
    }
}
