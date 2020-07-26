using BSTServer.Data;
using BSTServer.Models;
using BSTServer.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSTServer.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<User> _users = new List<User>
        //{
        //    new User
        //    {
        //        Id = 1,
        //        Role = UserRoles.Admin,
        //        Username = "test",
        //        Password = "test"
        //    }
        //};

        public async Task<User> Authenticate(string username, string md5Pass)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(k => k.Username == username && k.Password == md5Pass);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so return user details without password
            return user.WithoutPassword();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return (await _dbContext.Users.ToListAsync()).Select(k => k.WithoutPassword());
        }
    }
}