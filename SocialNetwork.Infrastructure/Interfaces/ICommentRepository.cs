using SocialNetwork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces
{
    public interface ICommentRepository
    {
        Task<bool> PostComment(Comment commentToPost);
    }
}
