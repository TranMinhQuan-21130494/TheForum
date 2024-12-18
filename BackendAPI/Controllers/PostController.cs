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
    public class PostController(PostService postService, CommentService commentService) : ControllerBase {
        private readonly PostService _postService = postService;
        private readonly CommentService _commentService = commentService;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PostResponse> GetPost(Guid id) {
            try {
                return Ok(_postService.ToPostResponse(_postService.GetOneById(id)));
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
            CommentAddDTO commentAddDTO = new() {
                Content = request.Comment,
                UserId = userId,
                PostId = Guid.Empty,
            };

            _postService.Add(postAddDTO, commentAddDTO);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ICollection<PostResponse>> GetPosts(int pageSize, int pageNumber) {
            try {
                ICollection<PostDTO> posts = _postService.GetList(pageSize, pageNumber);
                ICollection<PostResponse> postResponses = 
                    posts.Select(post => _postService.ToPostResponse(post)).ToList();
                return Ok(postResponses);
            }
            catch (IllegalParameterException) {
                return BadRequest("Page size and page number should begin at 1");
            }
        }
    } 
}
