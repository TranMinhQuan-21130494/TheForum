using BackendAPI.Data;
using BackendAPI.Entities;
using BackendAPI.Exceptions;
using BackendAPI.Utils;

namespace BackendAPI.Services {
    public class UserService(
        UserRepository repository, 
        ImageService imageService,
        IConfiguration configuration
        ) {
        private readonly UserRepository _userRepository = repository;
        private readonly ImageService _imageService = imageService;
        private readonly string _imageBaseURI = configuration["URL:ImageBaseURI"]!;

        public UserDTO GetOneById(Guid id) {
            User user = _userRepository.GetOneById(id);
            return UserDTO.FromEntity(user);
        }

        public UserDTO GetOneByEmail(string email) {
            User user = _userRepository.GetOneByEmail(email);
            return UserDTO.FromEntity(user);
        }

        public void Add(UserAddDTO userAddDTO) {
            try {
                User userInDatabase = _userRepository.GetOneByEmail(userAddDTO.Email);
                // Trong trường hợp đã có email trong DB rồi thì không được phép add
                throw new EntityUniqueCollisionException("Email existed in another user, cannot add");
            }
            catch (EntityNotFoundException) {
                // Thêm ảnh vào nơi lưu trữ
                string? imageFileName = null;
                if(userAddDTO.AvatarImage != null) {
                    imageFileName = _imageService.Add(userAddDTO.AvatarImage);
                }

                // Nếu chưa có User với email trùng thì được phép thêm vào
                User user = new() {
                    Id = Guid.NewGuid(),
                    Name = userAddDTO.Name,
                    Email = userAddDTO.Email,
                    HashedPassword = HashUtil.Hash(userAddDTO.RawPassword), // Hash pwd before add to DB'
                    AvatarImageName = imageFileName,
                    Role = userAddDTO.Role,
                    Status = userAddDTO.Status,
                    CreatedTime = DateTime.Now,
                    LastUpdatedTime = null
                };
                _userRepository.Add(user);
            }
        }

        public void Update(UserUpdateDTO userUpdateDTO) {
            User user = _userRepository.GetOneById(userUpdateDTO.Id);

            if (userUpdateDTO.AvatarImage != null) user.AvatarImageName = _imageService.Add(userUpdateDTO.AvatarImage);
            if (userUpdateDTO.Name != null) user.Name = userUpdateDTO.Name;
            if (userUpdateDTO.RawPassword != null) user.HashedPassword = HashUtil.Hash(userUpdateDTO.RawPassword);
            if (userUpdateDTO.Status != null) user.Status = userUpdateDTO.Status;
            if (userUpdateDTO.Role != null) user.Role = userUpdateDTO.Role;

            user.LastUpdatedTime = DateTime.Now;
            _userRepository.Update(user);
        }

        public UserDTO GetOneUsingAuthentication(UserAuthenticationDTO userAuthenticationDTO) {
            User user = _userRepository.GetOneByEmail(userAuthenticationDTO.Email);
            if(!HashUtil.Verify(userAuthenticationDTO.RawPassword, user.HashedPassword)) {
                throw new UnauthorizedException();
            }
            return UserDTO.FromEntity(user);
        }

        public bool CheckPasswordById(Guid userId, string rawPassword) {
            User user = _userRepository.GetOneById(userId);
            if (!HashUtil.Verify(rawPassword, user.HashedPassword)) {
                return false;
            }
            return true;
        }

        // Response generator
        public UserResponse ToUserResponse(UserDTO userDTO) {
            string avatarImageName = userDTO.AvatarImageName ?? "defaultAvatar.jpg";
            return new() {
                Id = userDTO.Id,
                Name = userDTO.Name,
                AvatarImageURI = $"{_imageBaseURI}/{avatarImageName}",
                Role = userDTO.Role,
                Status = userDTO.Status,
                CreatedTime = userDTO.CreatedTime
            };
        }
    }
}
