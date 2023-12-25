using SocialNetwork.Contracts.Models.EmailServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendMessageAsync(Message message);
    }
}
