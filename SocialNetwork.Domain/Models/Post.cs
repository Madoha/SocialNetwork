using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SocialNetwork.Domain.Models
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime WasCreated { get; set; } = DateTime.UtcNow;
        public DateTime WasEdited { get; set; } = DateTime.UtcNow; // idk
        public string UserId { get; set; }
        public string MediaUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        [JsonIgnore]
        public ApplicationUser User { get; set; }
    }
}
