using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SocialNetwork.Contracts.DTOs
{
    public class CommentDTO
    {
        [JsonIgnore]
        public Guid PostId { get; set; }
        [JsonIgnore]
        public string Username { get; set; } = "Unknown";
        public string Content { get; set; }
    }
}
