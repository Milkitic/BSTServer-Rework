using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BstServer.Models
{
    public class SteamUser
    {
        [Key]
        public string SteamId { get; set; }

        public string NickName { get; set; }

        public bool IsOnline { get; set; }

        public List<SteamUserSession> UserSessions { get; } = new List<SteamUserSession>();
    }
}
