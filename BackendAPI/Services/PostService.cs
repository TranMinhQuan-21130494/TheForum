using BackendAPI.Data;
using BackendAPI.Entities;
using BackendAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services {
    public class PostService(PostRepository repository) {
        private readonly PostRepository PostRepository = repository;

        /// <summary>
        /// Lấy danh sách các Post (có phân trang), danh sách sắp xếp thời gian mới nhất đến cũ nhất
        /// </summary>
        /// <param name="pageSize">Số lượng bản ghi trong một trang</param>
        /// <param name="pageNumber">Vị trí trang, bắt đầu từ 1</param>
        /// <exception cref="IllegalParameterException">Ném ra khi pageSize hoặc pageNumber nhỏ hơn hoặc bằng 0</exception>
        public ICollection<PostDTO> GetList(int pageSize, int pageNumber) {
            if (pageNumber <= 0 || pageSize <= 0) {
                throw new IllegalParameterException();
            }

            ICollection<Post> posts = PostRepository.GetList(pageSize, pageNumber);
            ICollection<PostDTO> result = [];
            foreach (Post post in posts) {
                result.Add(PostDTO.FromEntity(post));
            }
            return result;
        }

        /// <summary>
        /// Trả về một Post gồm các thông tin cơ bản
        /// </summary>
        /// <param name="id">ID của Post</param>
        /// <exception cref="EntityNotFoundException">Ném ra khi không có Post với ID đã cho.</exception>
        public PostDTO GetOne(Guid id) {
            Post post = PostRepository.GetOne(id);
            return PostDTO.FromEntity(post);
        }

        public void Add(PostAddDTO postAddDTO) {
            Post post = new() {
                Id = Guid.NewGuid(),
                Title = postAddDTO.Title,
                Status = postAddDTO.Status,
                CreatedTime = DateTime.Now,
                UserId = postAddDTO.UserId,
            };
            PostRepository.Add(post);
        }
    }
}
