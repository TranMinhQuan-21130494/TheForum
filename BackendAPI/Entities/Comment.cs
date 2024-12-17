using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Entities {
    public class Comment {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public required Guid PostId { get; set; }
        public required Guid UserId { get; set; }
        public Post Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    public class CommentTableConfig : IEntityTypeConfiguration<Comment> {
        public void Configure(EntityTypeBuilder<Comment> builder) {
            builder.ToTable("comment");
            builder.Property(comment => comment.Content).HasColumnType("varchar(2048)").IsRequired();
            builder.Property(comment => comment.Status).HasColumnType("varchar(32)").IsRequired();
        }
    }

    // Dùng trong mọi trường hợp, vì thông tin Comment không có quá nhiều
    public class CommentResponse {

    }
}
