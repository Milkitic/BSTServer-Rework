using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BstServer.Models
{
    public class SteamUser
    {
        [Key]
        public string SteamUserId { get; set; }

        public string Nickname { get; set; }

        public bool IsOnline { get; set; }

        public List<Session> Sessions { get; } = new List<Session>();
    }
}
