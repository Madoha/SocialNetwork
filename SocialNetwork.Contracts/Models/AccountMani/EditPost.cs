using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SocialNetwork.Contracts.Models.AccountMani
{
    public class EditPost
    {
        public string MediaUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [JsonIgnore]
        public DateTime WasEdited { get; set; } = DateTime.UtcNow;
    }
}
