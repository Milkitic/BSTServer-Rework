using System;
using System.Text.RegularExpressions;

namespace BSTServer.Hosting
{
    public class ConnectRegexObj : IRegexObj<ConnectInfo>
    {
        private static readonly Regex Regex =
            new Regex(@"L (.+) - (.+): ""(.+)<(.+)><(.+)><(.*)>"" connected, address ""(.+)""");
        public ConnectInfo Result { get; private set; }
        public bool Success { get; private set; }
        public void Match(string line)
        {
            var result = Regex.Match(line);
            if (result.Success)
            {
                Result = new ConnectInfo()
                {
                    Timestamp = DateTime.Parse($"{result.Groups[1].Value} {result.Groups[2].Value}"),
                    Username = result.Groups[3].Value,
                    EntityId = int.Parse(result.Groups[4].Value),
                    SteamId = result.Groups[5].Value,
                    PlayerStatus = result.Groups[6].Value,
                    IPAddress = result.Groups[7].Value
                };
            }

            Success = result.Success;
        }
    }

    public class DisconnectRegexObj : IRegexObj<DisconnectInfo>
    {
        private static readonly Regex Regex =
            new Regex(@"L (.+) - (.+): ""(.+)<(.+)><(.+)><(.*)>"" disconnected \(reason ""(.+)""\)");
        public DisconnectInfo Result { get; private set; }
        public bool Success { get; private set; }
        public void Match(string line)
        {
            var result = Regex.Match(line);
            if (result.Success)
            {
                Result = new DisconnectInfo()
                {
                    Timestamp = DateTime.Parse($"{result.Groups[1].Value} {result.Groups[2].Value}"),
                    Username = result.Groups[3].Value,
                    EntityId = int.Parse(result.Groups[4].Value),
                    SteamId = result.Groups[5].Value,
                    PlayerStatus = result.Groups[6].Value,
                    Reason = result.Groups[7].Value
                };
            }

            Success = result.Success;
        }
    }

    public class FriendlyFireRegexObj : IRegexObj<FriendlyFireInfo>
    {
        private static readonly Regex Regex =
            new Regex(@"L (.+) - (.+): \[l4d_ff_tracker.smx\] (.+) => (.+) \[(.+)\] \[(.+) DMG\]");
        public FriendlyFireInfo Result { get; private set; }
        public bool Success { get; private set; }
        public void Match(string line)
        {
            var result = Regex.Match(line);
            if (result.Success)
            {
                Result = new FriendlyFireInfo()
                {
                    Timestamp = DateTime.Parse($"{result.Groups[1].Value} {result.Groups[2].Value}"),
                    AttackerSteamIdOrBot = result.Groups[3].Value,
                    InjuredSteamIdOrBot = result.Groups[4].Value,
                    Weapon = result.Groups[5].Value,
                    Damage = int.Parse(result.Groups[6].Value),
                };
            }

            Success = result.Success;
        }
    }
}