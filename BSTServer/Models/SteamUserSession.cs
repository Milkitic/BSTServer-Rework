using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BstServer.Models
{
    public class SteamUserSession
    {
        [Key]
        public Guid SessionId { get; set; }
        public string SteamId { get; set; }
        public DateTimeOffset? ConnectTime { get; set; }

        public DateTimeOffset? DisconnectTime { get; set; }

        public List<SteamUserDamage> UserDamages { get; } = new List<SteamUserDamage>();

        [JsonIgnore]
        public SteamUser SteamUser { get; set; }
    }
}