using BSTServer;
using System;
using System.Text.RegularExpressions;

namespace RegexTest
{
    class Program
    {
        private static readonly Regex _disconnectRegex =
          new Regex(@"L (.+) - (.+): ""(.+)<(.+)><(.+)><(.*)>"" disconnected \(reason ""(.+)""\)");
        private static readonly Regex _ffRegex =
            new Regex(@"L (.+) - (.+): \[l4d_ff_tracker.smx\] (.+) => (.+) \[(.+)\] \[(.+) DMG\]");
        static void Main(string[] args)
        {
            var dateTime = DateTime.Now;
            var str = EasyInviteCode.Generate("milkitic", dateTime);
            var success = EasyInviteCode.ConvertBack(str);

            var connectRegex = new ConnectRegexObj();
            connectRegex.Match(
                "L 07/19/2020 - 17:48:37: \"CPU 0<1774><STEAM_1:0:138017978><>\" connected, address \"112.64.0.74:6785\"");
        }

    }

    class ConnectRegexObj : IRegexObj<ConnectInfo>
    {
        private static readonly Regex _connectRegex =
            new Regex(@"L (.+) - (.+): ""(.+)<(.+)><(.+)><(.*)>"" connected, address ""(.+)""");
        public ConnectInfo Result { get; private set; }
        public bool Success { get; private set; }
        public void Match(string line)
        {
            var result = _connectRegex.Match(line);
            if (result.Success)
            {
                Result = new ConnectInfo()
                {
                    Timestamp = DateTime.Parse($"{result.Groups[1].Value} {result.Groups[2].Value}"),
                    Username = result.Groups[3].Value,
                    EntityId = int.Parse(result.Groups[4].Value),
                    SteamId = result.Groups[5].Value,
                    IPAddress = result.Groups[6].Value
                };
            }

            Success = result.Success;
        }
    }

    internal interface IRegexObj<T>
    {
        T Result { get; }
        bool Success { get; }
        void Match(string line);
    }

    internal class ConnectInfo
    {
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public int EntityId { get; set; }
        public string SteamId { get; set; }
        public string IPAddress { get; set; }
    }
}
