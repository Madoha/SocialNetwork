using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Contracts.DTOs
{
    public class PostDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string MediaUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime WasCreated { get; set; }
        public DateTime WasEdited { get; set; }
    }
}
