using BackendAPI.Data;

namespace BackendAPI.Services {
    public class CommentReactionService(CommentReactionRepository repository) {
        private readonly CommentReactionRepository _repository = repository;

        public int countReactionByCommentId(Guid commentId) {
            return _repository.countReactionByCommentId(commentId);
        }
    }
}
