namespace BackendAPI.Data {
    public class CommentRepository(AppDbContext appDbContext) {
        private readonly AppDbContext AppDbContext = appDbContext;
    }
}
