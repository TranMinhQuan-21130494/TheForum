using BackendAPI.Data;
using BackendAPI.Entities;
using BackendAPI.Exceptions;

namespace BackendAPI.Services {
    public class CommentService(
        CommentRepository commentRepository,
        UserService userService,
        CommentReactionService commentReactionService
        ) {
        private readonly CommentRepository _commentRepository = commentRepository;
        private readonly UserService _userService = userService;
        private readonly CommentReactionService _commentReactionService = commentReactionService;

        public int CountCommentByPostId(Guid postId) {
            return _commentRepository.CountCommentByPostId(postId);
        }

        public void Add(CommentAddDTO commentAddDTO) {
            Comment comment = new() {
                Id = Guid.NewGuid(),
                Content = commentAddDTO.Content,
                Status = "PUBLISHED",
                ImageName = commentAddDTO.ImageName,
                CreatedTime = DateTime.Now,
                PostId = commentAddDTO.PostId,
                UserId = commentAddDTO.UserId
            };
            _commentRepository.Add(comment);
        }

        public ICollection<CommentDTO> GetListByPostId(Guid postId, int pageSize, int pageNumber) {
            if (pageNumber <= 0 || pageSize <= 0) {
                throw new IllegalParameterException();
            }

            ICollection<Comment> comments = _commentRepository.GetListByPostId(postId, pageSize, pageNumber);
            ICollection<CommentDTO> result = [];
            foreach (Comment comment in comments) {
                result.Add(CommentDTO.FromEntity(comment));
            }
            return result;
        }

        public CommentResponse ToCommentResponse(CommentDTO commentDTO) {
            return new() { 
                Id = commentDTO.Id,
                Content = commentDTO.Content,
                Status = commentDTO.Status,
                ImageName = commentDTO.ImageName,
                CreatedTime = commentDTO.CreatedTime,
                ReactionCount = _commentReactionService.countReactionByCommentId(commentDTO.Id),
                User = _userService.ToUserResponse(commentDTO.User),
            };
        }
    }
}
