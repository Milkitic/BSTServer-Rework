using BstServer.Models;
using BSTServer.Models;
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

        public async Task InsertUserAsync(User user)
        {
            await Users.AddAsync(user);
            await SaveChangesAsync();
        }

        public async Task<SteamUser> GetSteamUserBySteamId(string steamId)
        {
            var user = await SteamUsers.FirstOrDefaultAsync(k => k.SteamId == steamId);
            return user;
        }

        public async Task<SteamUserSession> GetSteamUserSessionById(string steamId, Guid sessionId)
        {
            var user = (await SteamUsers.FirstOrDefaultAsync(k => k.SteamId == steamId)).UserSessions
                .FirstOrDefault(k => k.SessionId == sessionId);
            return user;
        }

        public async Task<SteamUserSession> GetSessionByTime(string steamId, DateTime time)
        {
            var user = (await SteamUsers.FirstOrDefaultAsync(k => k.SteamId == steamId)).UserSessions
                .FirstOrDefault(k => k.ConnectTime <= time && k.DisconnectTime >= time);
            return user;
        }

        public async Task<SteamUserSession> GetSteamUserCurrentSession(string steamId)
        {
            var now = DateTime.Now;
            var user = (await SteamUsers.FirstOrDefaultAsync(k => k.SteamId == steamId)).UserSessions
                .FirstOrDefault(k => k.ConnectTime <= now && k.DisconnectTime >= now);
            return user;
        }
    }
}
