using BackendAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers {
    [Route("api/images")]
    [ApiController]
    public class ImageController(ImageService imageService) : Controller {
        private readonly ImageService _imageService = imageService;

        [HttpPost]
        [Authorize]
        public ActionResult UploadImage(IFormFile file) {
            string imgName = _imageService.Add(file);
            return Ok(new {
                imageName = imgName
            });
        }
    }
}
