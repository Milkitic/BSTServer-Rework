using BSTServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSTServer.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
        Task AddUser(SignUpModel signUpModel);
    }
}
