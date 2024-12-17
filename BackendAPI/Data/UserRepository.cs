using BackendAPI.Entities;
using BackendAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Data {
    public class UserRepository(AppDbContext appDbContext) {
        private readonly AppDbContext AppDbContext = appDbContext;
        public ICollection<User> GetList(int pageSize, int pageNumber) {
            return AppDbContext.Users
                .OrderByDescending(user => user.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public User GetOneById(Guid id) {
            User? user = AppDbContext.Users.SingleOrDefault(user => user.Id == id);
            return user ?? throw new EntityNotFoundException();
        }

        public  User GetOneByEmail(string email) {
            User? user = AppDbContext.Users.SingleOrDefault(user => user.Email.Equals(email));
            return user ?? throw new EntityNotFoundException();
        }

        public void Add(User user) { 
            AppDbContext.Users.Add(user);
            AppDbContext.SaveChanges();
        }

        public void Update(User user) {
            AppDbContext.Users.Update(user);
            AppDbContext.SaveChanges();
        }
    }
}
