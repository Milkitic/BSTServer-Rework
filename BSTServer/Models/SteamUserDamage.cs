using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace BstServer.Models
{
    public class SteamUserDamage
    {
        [Key]
        public Guid Id { get; set; }
        public string SteamId { get; set; }
        public Guid SessionId { get; set; }
        public DateTimeOffset DamageTime { get; set; }
        public bool IsHurt { get; set; }

        [JsonIgnore]
        public SteamUser SteamUser { get; set; }

        [JsonIgnore]
        public SteamUserSession SteamUserSession { get; set; }
    }
}