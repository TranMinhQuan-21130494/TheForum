namespace BackendAPI.Data {
    public class CommentReactionRepository(AppDbContext appDbContext) {
        private readonly AppDbContext _appDbContext = appDbContext;

        public int countReactionByCommentId(Guid commentId) {
            return _appDbContext.CommentsReaction
                .Where(commentReaction => commentReaction.CommentId == commentId)
                .Count();
        }
    }
}
