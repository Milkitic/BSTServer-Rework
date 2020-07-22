using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BstServer.Models
{
    public class Session
    {
        [Key]
        public Guid SessionId { get; set; }
        public string SteamUserId { get; set; }
        public DateTimeOffset? ConnectTime { get; set; }

        public DateTimeOffset? DisconnectTime { get; set; }

        public List<SessionDamage> UserDamages { get; } = new List<SessionDamage>();

        [JsonIgnore]
        public SteamUser SteamUser { get; set; }
    }
}