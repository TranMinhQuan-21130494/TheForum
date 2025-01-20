using Microsoft.AspNetCore.Mvc;
using BackendAPI.Entities;
using BackendAPI.Services;
using BackendAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BackendAPI.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController(UserService userService, PostService postService, CommentService commentService) : ControllerBase {
        private readonly PostService _postService = postService;
        private readonly CommentService _commentService = commentService;
        private readonly UserService _userService = userService;

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
            UserDTO user = _userService.GetOneById(userId);
            if (user.Status.Equals("BANNED")) {
                return NotFound(new {
                    message = "Tài khoản đã bị khóa, không thể thêm bài viết"
                });
            }

            PostAddDTO postAddDTO = new() {
                Title = request.Title,
                Category = request.Category,
                Status = PostEnum.STATUS_PUBLISHED,
                UserId = userId
            };
            CommentAddDTO commentAddDTO = new() {
                Content = request.Comment,
                ImageName = request.ImageName,
                UserId = userId,
                PostId = Guid.Empty,
            };

            _postService.Add(postAddDTO, commentAddDTO);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ICollection<PostResponse>> GetPosts(string category, int pageSize, int pageNumber) {
            try {
                ICollection<PostDTO> posts = _postService.GetList(category, pageSize, pageNumber);
                ICollection<PostResponse> postResponses = 
                    posts.Select(post => _postService.ToPostResponse(post)).ToList();
                return Ok(postResponses);
            }
            catch (IllegalParameterException) {
                return BadRequest("Page size and page number should begin at 1");
            }
        }

        [HttpGet("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ICollection<CommentResponse>> GetPostComments(Guid id, int pageSize, int pageNumber) {
            _commentService.GetListByPostId(id, pageSize, pageNumber);
            try {
                ICollection<CommentDTO> comments = _commentService.GetListByPostId(id, pageSize, pageNumber);
                ICollection<CommentResponse> commentResponses =
                    comments.Select(comment => _commentService.ToCommentResponse(comment)).ToList();
                return Ok(commentResponses);
            }
            catch (IllegalParameterException) {
                return BadRequest("Page size and page number should begin at 1");
            }
        }

        [Authorize]
        [HttpPost("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ICollection<CommentResponse>> AddPostComments(Guid id, CommentRequest commentRequest) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            CommentAddDTO commentAddDTO = new CommentAddDTO {
                Content = commentRequest.Content,
                ImageName = commentRequest.ImageName,
                PostId = id,
                UserId = userId,

            };
            _commentService.Add(commentAddDTO);
            _postService.UpdateLastActivityTime(id);
            return Ok();
        }
    } 
}
