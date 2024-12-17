﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendAPI.Entities {
    public class UserEnum {
        public static readonly string ROLE_USER = "USER";
        public static readonly string ROLE_MODERATOR = "MODERATOR";
        public static readonly string ROLE_ADMIN = "ADMIN";
        public static readonly string STATUS_ACTIVE = "ACTIVE";
        public static readonly string STATUS_BANNED = "BANNED";
    }

    public class User {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public string? AvatarImageName { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
    }

    public class UserTableConfig : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            // Khai báo kiểu dữ liệu và ràng buộc
            builder.ToTable("user");
            builder.Property(user => user.Name).HasColumnType("varchar(64)").IsRequired();
            builder.Property(user => user.Email).HasColumnType("varchar(256)").IsRequired();
            builder.Property(user => user.HashedPassword).HasColumnType("varchar(256)").IsRequired();
            builder.Property(user => user.AvatarImageName).HasColumnType("varchar(256)");
            builder.Property(user => user.Role).HasColumnType("varchar(32)").IsRequired();
            builder.Property(user => user.Status).HasColumnType("varchar(32)").IsRequired();
            builder.Property(user => user.CreatedTime).IsRequired();
        }
    }

    // DTO for display
    public class UserDTO {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? AvatarImageName { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }

        public static UserDTO FromEntity(User user) {
            return new UserDTO {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarImageName = user.AvatarImageName,
                Role = user.Role,
                Status = user.Status,
                CreatedTime = user.CreatedTime,
                LastUpdatedTime = user.LastUpdatedTime,
            };
        }
    }

    // DTO ADD: Avatar có thể 
    public class UserAddDTO {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string RawPassword { get; set; }
        public IFormFile? AvatarImage { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
    }

    // DTO UPDATE: Dữ liệu nào không cần cập nhật thì để null
    public class UserUpdateDTO {
        public required Guid Id { get; set; }
        public string? Name { get; set; }
        public string? RawPassword { get; set; }
        public IFormFile? AvatarImage { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }

    public class UserAuthenticationDTO {
        public required string Email { get; set; }
        public required string RawPassword { get; set; }
    }

    // Network
    public class UserAddRequest {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public IFormFile? AvatarImage {  get; set; }
    }

    public class UserUpdateRequest {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public IFormFile? AvatarImage { get; set; }
    }

    public class UserAuthenticationRequest {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    // Response thông tin chi tiết của User, nên sử dụng khi xem chi tiết 1 User
    public class UserResponse {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string AvatarImageURL { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedTime { get; set; }
        public required UserLinks _links { get; set; }

        public class UserLinks {
            public required string Self { get; set; }
            public required string UserPosts { get; set; }
        }

        public static UserResponse FromDTO(UserDTO userDTO, string apiBaseURL, string imageBaseURL) {
            string avatarImageName = userDTO.AvatarImageName ?? "defaultAvatar.jpg";
            return new() {
                Id = userDTO.Id,
                Name = userDTO.Name,
                AvatarImageURL = $"{imageBaseURL}/{avatarImageName}",
                Role = userDTO.Role,
                Status = userDTO.Status,
                CreatedTime = userDTO.CreatedTime,
                _links = new() {
                    Self = $"{apiBaseURL}/users/{userDTO.Id}",
                    UserPosts = $"{apiBaseURL}/users/{userDTO.Id}/posts"
                }
            };
        }
    }

    // Thông tin cơ bản của User kèm theo link đến thông tin chi tiết
    // Nên dùng class này khi muốn lấy thông tin user từ các nguồn khác, nơi mà User không phải là đối tượng chính
    // Ví dụ Post, Comment, danh sách các user trong trang admin, ...
    public class UserBasicInfoResponse {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string AvatarImageURL { get; set; }
        public required string Role { get; set; }
        public required UserLinks2 _links { get; set; }

        public class UserLinks2 {
            public required string Self { get; set; }
        }

        public static UserBasicInfoResponse FromDTO(UserDTO userDTO, string apiBaseURL, string imageBaseURL) {
            string avatarImageName = userDTO.AvatarImageName ?? "defaultAvatar.jpg";
            return new() {
                Id = userDTO.Id,
                Name = userDTO.Name,
                AvatarImageURL = $"{imageBaseURL}/{avatarImageName}",
                Role = userDTO.Role,
                _links = new() {
                    Self = $"{apiBaseURL}/users/{userDTO.Id}",
                },
            };
        }
    }
}
