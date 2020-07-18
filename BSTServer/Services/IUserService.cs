using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BSTServer.Models;

namespace BSTServer.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
    }
}
