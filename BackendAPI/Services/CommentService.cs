using BackendAPI.Data;
using BackendAPI.Entities;

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
                CreatedTime = DateTime.Now,
                PostId = commentAddDTO.PostId,
                UserId = commentAddDTO.UserId
            };
            _commentRepository.Add(comment);
        }

        public CommentResponse ToCommentResponse(CommentDTO commentDTO) {
            return new() { 
                Id = commentDTO.Id,
                Content = commentDTO.Content,
                CreatedTime = commentDTO.CreatedTime,
                ReactionCount = _commentReactionService.countReactionByCommentId(commentDTO.Id),
                User = _userService.ToUserResponse(commentDTO.User),
            };
        }
    }
}
