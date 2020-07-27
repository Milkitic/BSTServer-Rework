using BstServer.Models;
using BSTServer.Models;
using BSTServer.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BSTServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SteamUser> SteamUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Password = UserService.GetSaltedPass("r00t@bst123"),
                Username = "root",
                Role = UserRoles.Root,
                InviteCodes = string.Join('|', EasyInviteCode.Generate("root", DateTime.Now.AddDays(7)))
            });
        }

        public async Task InsertUserAsync(User user)
        {
            await Users.AddAsync(user);
            await SaveChangesAsync();
        }

        public async Task<SteamUser> GetSteamUserBySteamId(string steamId)
        {
            var user = await SteamUsers.FirstOrDefaultAsync(k => k.SteamUserId == steamId);
            return user;
        }

        public async Task<Session> GetSteamUserSessionById(string steamId, Guid sessionId)
        {
            var user = (await SteamUsers.FirstOrDefaultAsync(k => k.SteamUserId == steamId)).Sessions
                .FirstOrDefault(k => k.SessionId == sessionId);
            return user;
        }

        public async Task<Session> GetSessionByTime(string steamId, DateTime time)
        {
            var user = (await SteamUsers.FirstOrDefaultAsync(k => k.SteamUserId == steamId)).Sessions
                .FirstOrDefault(k => k.ConnectTime <= time && k.DisconnectTime >= time);
            return user;
        }

        public async Task<Session> GetSteamUserCurrentSession(string steamId)
        {
            var now = DateTime.Now;
            var user = (await SteamUsers.FirstOrDefaultAsync(k => k.SteamUserId == steamId)).Sessions
                .FirstOrDefault(k => k.ConnectTime <= now && k.DisconnectTime >= now);
            return user;
        }
    }
}
