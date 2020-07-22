using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace BstServer.Models
{
    public class SessionDamage
    {
        [Key]
        public Guid SessionDamageId { get; set; }
        public Guid SessionId { get; set; }
        public string SteamUserId { get; set; }
        public DateTimeOffset DamageTime { get; set; }
        public bool IsHurt { get; set; }
        public int Damage { get; set; }
        public string Weapon { get; set; }

        [JsonIgnore]
        public SteamUser SteamUser { get; set; }

        [JsonIgnore]
        public Session Session { get; set; }
    }
}