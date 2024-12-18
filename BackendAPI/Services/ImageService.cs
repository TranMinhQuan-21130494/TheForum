namespace BackendAPI.Services{
    public class ImageService {
        public string Add(IFormFile file) {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            string imageFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, imageFileName);
            using (var stream = new FileStream(filePath, FileMode.Create)) {
                file.CopyTo(stream);
            }
            return imageFileName;
        }

        public void Delete(string fileName) {
            // TODO
        }
    }
}
