using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BstServer.Models
{
    public class SteamUser
    {
        [Key]
        public string SteamId { get; set; }

        public string NickName { get; set; }

        [NotMapped]
        public bool IsOnline { get; set; } = false;

        public List<SteamUserSession> UserSessions { get; } = new List<SteamUserSession>();
    }
}
