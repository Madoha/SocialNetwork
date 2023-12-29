using SocialNetwork.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Interfaces
{
    public interface ICommentService
    {
        Task<bool> PostComment(string postId, string username, CommentDTO comment);
    }
}
