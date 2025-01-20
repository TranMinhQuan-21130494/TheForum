using BackendAPI.Entities;
using Microsoft.EntityFrameworkCore;

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

        public void UpdateStatus(Guid id, string status) {
            Comment comment = _appDbContext.Comments.Single(comment => comment.Id == id);
            comment.Status = status;
            _appDbContext.Comments.Update(comment);
            _appDbContext.SaveChanges();
        }

        public ICollection<Comment> GetListByPostId(Guid postId, int pageSize, int pageNumber) {
            return _appDbContext.Comments
                .Include(comment => comment.User)
                .Where(comment => comment.PostId == postId)
                .OrderBy(comment => comment.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
