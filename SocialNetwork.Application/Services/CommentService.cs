using AutoMapper;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        public CommentService(ICommentRepository commentRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
        }
        public async Task<bool> PostComment(string postId, string username, CommentDTO comment)
        {
            var commentToPost = _mapper.Map<Comment>(comment);
            commentToPost.PostId = Guid.Parse(postId);
            commentToPost.Username = username;

            var result = await _commentRepository.PostComment(commentToPost);
            return result;
        }
    }
}
