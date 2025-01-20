using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Entities {
    public class Comment {
        public required Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public string? ImageName { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public required Guid PostId { get; set; }
        public required Guid UserId { get; set; }
        public Post? Post { get; set; }
        public User? User { get; set; }
    }

    public class CommentTableConfig : IEntityTypeConfiguration<Comment> {
        public void Configure(EntityTypeBuilder<Comment> builder) {
            builder.ToTable("comment");
            builder.Property(comment => comment.Content).HasColumnType("varchar(2048)").IsRequired();
        }
    }

    public class CommentDTO {
        public required Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public string? ImageName { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public required UserDTO User { get; set; }

        public static CommentDTO FromEntity(Comment comment) {
            return new() { 
                Id = comment.Id,
                Content = comment.Content,
                Status = comment.Status,
                ImageName = comment.ImageName,
                CreatedTime = comment.CreatedTime, 
                UpdatedTime = comment.UpdatedTime, 
                User = UserDTO.FromEntity(comment.User!)
            };
        }
    }

    public class CommentAddDTO {
        public required string Content { get; set; }
        public string? ImageName { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CommentRequest {
        public required string Content { get; set; }
        public string? ImageName { get; set; }
    }

    // Dùng trong mọi trường hợp, vì thông tin Comment không có quá nhiều
    public class CommentResponse {
        public required Guid Id { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public string? ImageName { get; set; }
        public required int ReactionCount { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public required UserResponse User { get; set; }
    }
}
