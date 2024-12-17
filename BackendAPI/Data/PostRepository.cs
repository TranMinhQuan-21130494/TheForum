using BackendAPI.Entities;
using BackendAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data {
    public class PostRepository(AppDbContext appDbContext) {
        private readonly AppDbContext AppDbContext = appDbContext;

        public ICollection<Post> GetList(int pageSize, int pageNumber) {
            return AppDbContext.Posts
                .Include(post => post.User)
                .OrderByDescending(post => post.LastActivityTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Post GetOne(Guid id) {
            Post? post = AppDbContext.Posts
                .Include(post => post.User)
                .SingleOrDefault(post => post.Id == id);
            return post ?? throw new EntityNotFoundException();
        }

        public void Add(Post post) {
            AppDbContext.Posts.Add(post);
            AppDbContext.SaveChanges();
        }
    }
}
