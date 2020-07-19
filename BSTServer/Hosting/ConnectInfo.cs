using System;

namespace BSTServer.Hosting
{
    public class ConnectInfo
    {
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public int EntityId { get; set; }
        public string SteamId { get; set; }
        public string PlayerStatus { get; set; }
        public string IPAddress { get; set; }
    }
    public class DisconnectInfo
    {
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public int EntityId { get; set; }
        public string SteamId { get; set; }
        public string PlayerStatus { get; set; }
        public string Reason { get; set; }
    }

    public class FriendlyFireInfo
    {
        public DateTime Timestamp { get; set; }

        public string AttackerSteamIdOrBot { get; set; }
        public string InjuredSteamIdOrBot { get; set; }

        public string Weapon { get; set; }
        public int Damage { get; set; }
    }
}