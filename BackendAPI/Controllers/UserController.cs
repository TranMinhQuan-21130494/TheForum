using BackendAPI.Entities;
using BackendAPI.Exceptions;
using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackendAPI.Controllers {
    [Route("api/users")]
    [ApiController]
    public class UserController(UserService userService, PostService postService) : ControllerBase {
        private readonly UserService _userService = userService;
        private readonly PostService _postService = postService;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserResponse> GetUser(Guid id) {
            try {
                return Ok(_userService.ToUserResponse(_userService.GetOneById(id)));
            }
            catch (EntityNotFoundException) {
                return NotFound();
            }
        }

        [HttpGet("{id}/posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ICollection<PostResponse>> GetUserPosts(Guid id) {
            try {
                ICollection<PostDTO> posts = _postService.GetPostsByUserId(id);
                ICollection<PostResponse> postResponses =
                    posts.Select(post => _postService.ToPostResponse(post)).ToList();
                return Ok(postResponses);
            }
            catch (EntityNotFoundException) {
                return NotFound();
            }
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult AddUser(UserAddRequest request) {
            UserAddDTO userAddDTO = new() {
                Name = request.Name,
                Email = request.Email,
                RawPassword = request.Password,
                AvatarImage = request.AvatarImage,
                Role = UserEnum.ROLE_USER,
                Status = UserEnum.STATUS_ACTIVE
            };
            try {
                _userService.Add(userAddDTO);
                return Ok();
            }
            catch (EntityUniqueCollisionException) {
                return Conflict();
            }
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserResponse> GetCurrentUser() {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try {
                return Ok(_userService.ToUserResponse(_userService.GetOneById(userId)));
            }
            catch (EntityNotFoundException) {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPost("me")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult UpdateUser(UserUpdateRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            UserUpdateDTO userUpdateDTO = new() {
                Id = userId,
                Name = request.Name,
                RawPassword = request.Password,
                AvatarImage = request.AvatarImage,
            };
            _userService.Update(userUpdateDTO);
            return Ok();
        }

        [Authorize]
        [HttpGet("me/posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult GetCurrentUserPosts() {
            return Ok(); //TODO
        }

        [Authorize]
        [HttpPost("me/changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult ChangePassword(UserChangePasswordRequest changePasswordRequest) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isTruePassword = _userService.CheckPasswordById(userId, changePasswordRequest.OldPassword);
            if (!isTruePassword) {
                return BadRequest(new {
                    message = "Mật khẩu cũ không đúng",
                });
            }
            if (!changePasswordRequest.NewPassword.Equals(changePasswordRequest.RecheckPassword)) {
                return BadRequest(new {
                    message = "Mật khẩu mới nhập lại không trùng khớp",
                });
            }
            UserUpdateDTO userUpdateDTO = new() {
                Id = userId,
                RawPassword = changePasswordRequest.NewPassword,
            };
            _userService.Update(userUpdateDTO);
            return Ok();
        }
    }
}
