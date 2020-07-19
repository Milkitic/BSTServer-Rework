using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BstServer.Models
{
    public class SteamUserSession
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string SteamId { get; set; }
        public DateTimeOffset? ConnectTime { get; set; }

        public DateTimeOffset? DisconnectTime { get; set; }

        public List<SteamUserDamage> UserDamages { get; } = new List<SteamUserDamage>();

        [JsonIgnore]
        public SteamUser SteamUser { get; set; }
    }
}