using BackendAPI.Entities;
using BackendAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data {
    public class PostRepository(AppDbContext appDbContext) {
        private readonly AppDbContext _appDbContext = appDbContext;

        public ICollection<Post> GetList(int pageSize, int pageNumber) {
            return _appDbContext.Posts
                .Include(post => post.User)
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
    }
}
