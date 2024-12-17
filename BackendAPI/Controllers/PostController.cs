using Microsoft.AspNetCore.Mvc;
using BackendAPI.Entities;
using BackendAPI.Services;
using BackendAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackendAPI.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController(PostService postService, IConfiguration configuration) : ControllerBase {
        private readonly PostService _postService = postService;
        private readonly string _apiBaseURL = configuration["URL:APIBaseURL"]!;
        private readonly string _imageBaseURL = configuration["URL:ImageBaseURL"]!;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PostResponse> GetPost(Guid id) {
            try {
                return Ok(PostResponse.FromDTO(_postService.GetOneById(id), _apiBaseURL, _imageBaseURL));
            }
            catch (EntityNotFoundException) {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult AddPost(PostAddRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            PostAddDTO postAddDTO = new() {
                Title = request.Title,
                Status = PostEnum.STATUS_PUBLISHED,
                UserId = userId
            };

            _postService.Add(postAddDTO);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ICollection<PostResponse>> GetPosts(int pageSize, int pageNumber) {
            try {
                ICollection<PostDTO> posts = _postService.GetList(pageSize, pageNumber);
                ICollection<PostResponse> postResponses = 
                    posts.Select(post => PostResponse.FromDTO(post, _apiBaseURL, _imageBaseURL)).ToList();
                return Ok(postResponses);
            }
            catch (IllegalParameterException) {
                return BadRequest("Page size and page number should begin at 1");
            }
        }
    } 
}
