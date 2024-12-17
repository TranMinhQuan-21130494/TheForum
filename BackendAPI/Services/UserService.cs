using BackendAPI.Data;
using BackendAPI.Entities;
using BackendAPI.Exceptions;
using BackendAPI.Utils;

namespace BackendAPI.Services {
    public class UserService(UserRepository repository, ImageService imageService) {
        private readonly UserRepository UserRepository = repository;
        private readonly ImageService ImageService = imageService;

        public UserDTO GetOneById(Guid id) {
            User user = UserRepository.GetOneById(id);
            return UserDTO.FromEntity(user);
        }

        public UserDTO GetOneByEmail(string email) {
            User user = UserRepository.GetOneByEmail(email);
            return UserDTO.FromEntity(user);
        }

        public void Add(UserAddDTO userAddDTO) {
            try {
                User userInDatabase = UserRepository.GetOneByEmail(userAddDTO.Email);
                // Trong trường hợp đã có email trong DB rồi thì không được phép add
                throw new EntityUniqueCollisionException("Email existed in another user, cannot add");
            }
            catch (EntityNotFoundException) {
                // Thêm ảnh vào nơi lưu trữ
                string? imageFileName = null;
                if(userAddDTO.AvatarImage != null) {
                    imageFileName = ImageService.Add(userAddDTO.AvatarImage);
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
                UserRepository.Add(user);
            }
        }

        public void Update(UserUpdateDTO userUpdateDTO) {
            User user = UserRepository.GetOneById(userUpdateDTO.Id);

            if (userUpdateDTO.AvatarImage != null) user.AvatarImageName = ImageService.Add(userUpdateDTO.AvatarImage);
            if (userUpdateDTO.Name != null) user.Name = userUpdateDTO.Name;
            if (userUpdateDTO.RawPassword != null) user.HashedPassword = HashUtil.Hash(userUpdateDTO.RawPassword);
            if (userUpdateDTO.Status != null) user.Status = userUpdateDTO.Status;
            if (userUpdateDTO.Role != null) user.Role = userUpdateDTO.Role;

            user.LastUpdatedTime = DateTime.Now;
            UserRepository.Update(user);
        }

        public UserDTO GetOneUsingAuthentication(UserAuthenticationDTO userAuthenticationDTO) {
            User user = UserRepository.GetOneByEmail(userAuthenticationDTO.Email);
            if(!HashUtil.Verify(userAuthenticationDTO.RawPassword, user.HashedPassword)) {
                throw new UnauthorizedException();
            }
            return UserDTO.FromEntity(user);
        }
    }
}
