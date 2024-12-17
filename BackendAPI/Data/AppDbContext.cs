using BackendAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options) {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReaction> CommentsReaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            new UserTableConfig().Configure(modelBuilder.Entity<User>());
            new PostTableConfig().Configure(modelBuilder.Entity<Post>());
            new CommentTableConfig().Configure(modelBuilder.Entity<Comment>());
            new CommentReactionTableConfig().Configure(modelBuilder.Entity<CommentReaction>().HasNoKey());
        }
    }
}
