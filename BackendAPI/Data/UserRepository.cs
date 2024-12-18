using BackendAPI.Entities;
using BackendAPI.Exceptions;

namespace BackendAPI.Data {
    public class UserRepository(AppDbContext appDbContext) {
        private readonly AppDbContext _appDbContext = appDbContext;
        public ICollection<User> GetList(int pageSize, int pageNumber) {
            return _appDbContext.Users
                .OrderByDescending(user => user.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public User GetOneById(Guid id) {
            User? user = _appDbContext.Users.SingleOrDefault(user => user.Id == id);
            return user ?? throw new EntityNotFoundException();
        }

        public  User GetOneByEmail(string email) {
            User? user = _appDbContext.Users.SingleOrDefault(user => user.Email.Equals(email));
            return user ?? throw new EntityNotFoundException();
        }

        public void Add(User user) { 
            _appDbContext.Users.Add(user);
            _appDbContext.SaveChanges();
        }

        public void Update(User user) {
            _appDbContext.Users.Update(user);
            _appDbContext.SaveChanges();
        }
    }
}
