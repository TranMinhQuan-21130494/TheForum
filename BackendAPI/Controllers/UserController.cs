using BackendAPI.Entities;
using BackendAPI.Exceptions;
using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackendAPI.Controllers {
    [Route("api/users")]
    [ApiController]
    public class UserController(UserService userService, IConfiguration configuration) : ControllerBase {
        private readonly UserService _userService = userService;
        private readonly string _apiBaseURL = configuration["URL:APIBaseURL"]!;
        private readonly string _imageBaseURL = configuration["URL:ImageBaseURL"]!;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserResponse> GetUser(Guid id) {
            try {
                return Ok(UserResponse.FromDTO(_userService.GetOneById(id), _apiBaseURL, _imageBaseURL));
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
        public ActionResult<UserDTO> GetCurrentUser() {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try {
                return Ok(UserResponse.FromDTO(_userService.GetOneById(userId), _apiBaseURL, _imageBaseURL));
            }
            catch (EntityNotFoundException) {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPatch("me")]
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
    }
}
