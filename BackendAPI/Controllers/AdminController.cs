using BackendAPI.Data;
using BackendAPI.Entities;
using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers {
    [Route("api/admin")]
    [ApiController]
    public class AdminController(UserService userService, CommentRepository commentRepository) : Controller {
        private readonly UserService _userService = userService;
        private readonly CommentRepository _commentRepository = commentRepository;

        [HttpPatch("users/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeUserStatus(Guid id, string status) {
            UserUpdateDTO userUpdate = new UserUpdateDTO { Id = id, Status = status };
            _userService.Update(userUpdate);
            return Ok();
        }

        [HttpPatch("comments/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeCommentStatus(Guid id, string status) {
            _commentRepository.UpdateStatus(id, status);
            return Ok();
        }
    }
}
