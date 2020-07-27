using BSTServer.Data;
using BSTServer.Models;
using BSTServer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<User> Authenticate(string username, string password)
        {
            var md5Pass = GetSaltedPass(password);
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

        public async Task AddUser(SignUpModel signUpModel)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(k => k.Username == signUpModel.Username);
            if (user != null) throw new Exception("用户名已存在");
            if (signUpModel.Password != signUpModel.Password2)
                throw new Exception("两次密码不一致");

            #region Validate InviteCode

            var inviteCode = signUpModel.InviteCode;

            string username;
            DateTime expire;
            try
            {
                (username, expire) = EasyInviteCode.ConvertBack(inviteCode);
            }
            catch (Exception ex)
            {
                throw new Exception("邀请码无效");
            }

            string inviteCodes;
            try
            {
                var inviteUser = await _dbContext.Users.FirstOrDefaultAsync(k => k.Username == username);
                if (inviteUser == null) throw new Exception("邀请码无效");
                inviteCodes = inviteUser.InviteCodes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            var split = inviteCodes.Split('|');
            if (split.Contains(inviteCode))
            {
                if (DateTime.Now > expire)
                    throw new Exception("邀请码已过期");
            }
            else
            {
                throw new Exception("邀请码无效");
            }

            #endregion

            await _dbContext.Users.AddAsync(new User
            {
                Username = signUpModel.Username,
                Password = GetSaltedPass(signUpModel.Password),
                Role = UserRoles.User
            });
            await _dbContext.SaveChangesAsync();
        }

        public static string GetSaltedPass(string pass)
        {
            return ComputeMD5Upper32($"server_{pass}_bst");
        }

        public static string ComputeMD5Upper32(string str)
        {
            try
            {
                var stringBuilder = new StringBuilder(32);
                byte[] hash;
                using (var md5 = MD5.Create())
                {
                    hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                }

                foreach (byte num in hash)
                {
                    stringBuilder.Append(num.ToString("X2"));
                }

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MD5 32位大写加密出错：" + ex.Message);
            }
        }
    }
}