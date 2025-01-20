using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Entities {
    public class PostEnum {
        public static readonly string STATUS_PUBLISHED = "PUBLISHED";
        public static readonly string STATUS_PENDING = "PENDING";
        public static readonly string STATUS_LOCKED = "LOCKED";
        public static readonly string STATUS_DELETED = "DELETED";
    }

    public class Post {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Category { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required DateTime LastActivityTime { get; set; }
        public required Guid UserId { get; set; }
        public User? User { get; set; }
    }

    public class PostTableConfig : IEntityTypeConfiguration<Post> {
        public void Configure(EntityTypeBuilder<Post> builder) {
            // Khai báo kiểu dữ liệu và ràng buộc
            builder.ToTable("post");
            builder.Property(post => post.Title).HasColumnType("varchar(256)").IsRequired();
            builder.Property(post => post.Status).HasColumnType("varchar(32)").IsRequired();
            builder.Property(post => post.Category).HasColumnType("varchar(32)").IsRequired();
            builder.Property(post => post.CreatedTime).IsRequired();
            builder.Property(post => post.UserId).IsRequired();
        }
    }

    // DTOs
    public class PostDTO {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Category { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required DateTime LastActivityTime { get; set; }
        public required UserDTO User { get; set; }

        public static PostDTO FromEntity(Post post) {
            return new PostDTO {
                Id = post.Id,
                Title = post.Title,
                Category = post.Category,
                Status = post.Status,
                CreatedTime = post.CreatedTime,
                LastActivityTime = post.LastActivityTime,
                User = UserDTO.FromEntity(post.User!)
            };
        }
    }

    public class PostAddDTO {
        public required string Title { get; set; }
        public required string Category { get; set; }
        public required string Status { get; set; }
        public required Guid UserId { get; set; }
    }

    // Network
    public class PostAddRequest {
        public required string Title { get; set; }
        public required string Comment { get; set; }
        public string? ImageName { get; set; }
        public required string Category { get; set; }
    }

    // Thông tin chi tiết của một Post, có cả danh sách các Comment, có thể sử dụng với phân trang
    public class PostResponse {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Category { get; set; }
        public required string Status { get; set; }
        public required int CommentCount { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required DateTime LastActivityTime { get; set; }
        public required UserResponse User { get; set; }
    }
}