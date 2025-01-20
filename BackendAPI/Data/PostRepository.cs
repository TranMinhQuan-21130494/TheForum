using BackendAPI.Entities;
using BackendAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data {
    public class PostRepository(AppDbContext appDbContext) {
        private readonly AppDbContext _appDbContext = appDbContext;

        public ICollection<Post> GetList(string category, int pageSize, int pageNumber) {
            return _appDbContext.Posts
                .Include(post => post.User)
                .Where(post => post.Category.Equals(category))
                .OrderByDescending(post => post.LastActivityTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Post GetOne(Guid id) {
            Post? post = _appDbContext.Posts
                .Include(post => post.User)
                .SingleOrDefault(post => post.Id == id);
            return post ?? throw new EntityNotFoundException();
        }

        public void Add(Post post) {
            _appDbContext.Posts.Add(post);
            _appDbContext.SaveChanges();
        }

        public void Add(Post post, Comment comment) {
            _appDbContext.Posts.Add(post);
            _appDbContext.Comments.Add(comment);
            _appDbContext.SaveChanges();
        }

        public void UpdateLastActivityTime(Guid postId, DateTime lastActivityTime) {
            var post = _appDbContext.Posts.FirstOrDefault(p => p.Id == postId);
            if (post != null) {
                // Cập nhật LastActivityTime
                post.LastActivityTime = lastActivityTime;

                // Lưu thay đổi vào database
                _appDbContext.SaveChanges();
            }
            else {
                throw new EntityNotFoundException();
            }
        }

        public ICollection<Post> GetPostsByUserId(Guid userId) {
            return _appDbContext.Posts
                .Include(post => post.User)
                .Where(post => post.UserId.Equals(userId))
                .OrderByDescending(post => post.LastActivityTime)
                .ToList();
        }
    }
}
