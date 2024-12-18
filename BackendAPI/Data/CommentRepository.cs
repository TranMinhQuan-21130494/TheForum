using BackendAPI.Entities;

namespace BackendAPI.Data {
    public class CommentRepository(AppDbContext appDbContext) {
        private readonly AppDbContext _appDbContext = appDbContext;

        public int CountCommentByPostId(Guid postId) {
            return _appDbContext.Comments.Where(comment => comment.PostId == postId).Count();
        }

        public void Add(Comment comment) {
            _appDbContext.Comments.Add(comment);
            _appDbContext.SaveChanges();
        }
    }
}
