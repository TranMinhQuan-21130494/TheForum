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
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required Guid UserId { get; set; }
        public User? User { get; set; }
    }

    public class PostTableConfig : IEntityTypeConfiguration<Post> {
        public void Configure(EntityTypeBuilder<Post> builder) {
            // Khai báo kiểu dữ liệu và ràng buộc
            builder.ToTable("post");
            builder.Property(post => post.Title).HasColumnType("varchar(256)").IsRequired();
            builder.Property(post => post.Status).HasColumnType("varchar(32)").IsRequired();
            builder.Property(post => post.CreatedTime).IsRequired();
            builder.Property(post => post.UserId).IsRequired();
        }
    }

    // DTOs
    public class PostDTO {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required Guid UserId { get; set; }

        public static PostDTO FromEntity(Post post) {
            return new PostDTO {
                Id = post.Id,
                Title = post.Title,
                Status = post.Status,
                CreatedTime = post.CreatedTime,
                UserId = post.UserId,
            };
        }
    }

    public class PostAddDTO {
        public required string Title { get; set; }
        public required string Status { get; set; }
        public required Guid UserId { get; set; }
    }

    // Network
    public class PostAddRequest {
        public required string Title { get; set; }
        public required string Comment { get; set; }
    }

    public class PostResponse {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required UserBasicInfoResponse User { get; set; }
        public required List<CommentResponse> Comments { get; set; }
        public required Links _links { get; set; }

        public class Links {
            public required string Self { get; set; }
        }
    }

    // Thông tin cơ bản của một Post, thường dùng khi hiển thị danh sách các Post
    public class PostBasicInfoResponse {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required UserBasicInfoResponse User { get; set; }
        public required Links _links { get; set; }

        public class Links {
            public required string Self { get; set; }
        }
    }
}