using BSTServer.Data;
using BSTServer.Hosting.HostingBase;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BstServer.Models;

namespace BSTServer.Hosting
{
    public class L4D2AppHost : AppHost
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ConfigFilePath = Path.Combine(CurrentPath, "user.json");

        private Task _scanTask;
        private CancellationTokenSource _cts;
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        private readonly List<SteamUserSession> _users = new List<SteamUserSession>();
        
        public L4D2AppHost(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _cts = new CancellationTokenSource();
        }

        protected override async Task OnDataReceived(object sender, OutputReceivedEventArgs e)
        {
            _queue.Enqueue(e.Data);
        }

        protected override async Task OnStarted()
        {
            _scanTask = Task.Factory.StartNew(ScanTask, TaskCreationOptions.LongRunning);
        }

        protected override async Task OnExited()
        {
            _cts.Cancel();
            _queue.Clear();
            await _scanTask;
        }

        private void ScanTask()
        {
            while (!_cts.IsCancellationRequested)
            {
                while (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out var current))
                    {
                        Parse(current);
                    }
                }

                Thread.Sleep(10);
            }
        }

        private void Parse(string source)
        {
            if (source == null) return;
            try
            {
                if (DetectConnect(source)) return;
                if (DetectDisconnect(source)) return;
                if (DetectFriendlyFire(source)) return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool DetectFriendlyFire(string source)
        {
            const string blackKey1 = ": [l4d_ff_tracker.smx] STEAM_";
            const string blackKey2 = " => ";

            var blackI1 = source.IndexOf(blackKey1, StringComparison.Ordinal);
            if (blackI1 == -1) return false;
            var blackI2 = source.IndexOf(blackKey2, StringComparison.Ordinal);
            if (blackI2 == -1) return false;
            var steamUid = source.Substring(blackKey1.Length + blackI1 - 6, blackI2 - (blackKey1.Length + blackI1 - 6));
            var user = UserConfig.SteamUsers.FirstOrDefault(k => k.SteamUid == steamUid && k.IsOnline);
            bool isValid = false;
            if (user == null) return false;

            var info = source.Substring(blackI2 + 4);
            var infoArray = info.Split(" [").Select(k => k.Trim(']')).ToArray();
            string botOrUid = infoArray[0];
            string weapon = infoArray[1];
            int damage = int.Parse(infoArray[2].Split(' ')[0]);
            bool isBot = botOrUid == "BOT";
            SteamUser user2 = null;
            if (!isBot)
            {
                user2 = UserConfig.SteamUsers.FirstOrDefault(k => k.SteamUid == botOrUid && k.IsOnline);
                if (user2 != null) isValid = true;
            }
            else
            {
                Console.WriteLine($"FF: {user.CurrentName} =({weapon})> BOT [{damage} HP]");
            }

            if (!isValid) return false;
            var nowCustomDate = new CustomDate(DateTime.Now);
            DailySteamUserInfo user1Today = GetTodayInfo(user, nowCustomDate); //user1 today info (create if not exist)
            DailySteamUserInfo user2Today = GetTodayInfo(user2, nowCustomDate); //user2 today info

            GetWeapon(weapon, user1Today.WeaponInfos).Damage += damage; //1 damage (create if not exist)
            GetWeapon(weapon, user2Today.WeaponInfos).Hurt += damage; //2 hurt

            GetWeapon(weapon, user1Today.WeaponInfos).DamageTimes += 1; //1 damage count
            GetWeapon(weapon, user2Today.WeaponInfos).HurtTimes += 1; //2 hurt count

            Console.WriteLine($"FF: {user.CurrentName} =({weapon})> {user2.CurrentName} [{damage} HP]");

            return true;
        }

        private bool DetectConnect(string source)
        {
            var obj = new ConnectRegexObj();
            obj.Match(source);
            if (obj.Success)
            {
                
            }

            return obj.Success;
        }

        private bool DetectDisconnect(string source)
        {
            var obj = new DisconnectRegexObj();
            obj.Match(source);
            if (obj.Success)
            {

            }

            return obj.Success;
        }

        private static WeaponCell GetWeapon(string weapon, ICollection<WeaponCell> dmg)
        {
            WeaponCell weaponDmg = dmg.FirstOrDefault(k => k.WeaponName == weapon);
            if (weaponDmg != null) return weaponDmg;

            dmg.Add(new WeaponCell(weapon));
            weaponDmg = dmg.First(k => k.WeaponName == weapon);
            return weaponDmg;
        }

        private static DailySteamUserInfo GetTodayInfo(SteamUser user, CustomDate nowCustomDate)
        {
            DailySteamUserInfo today =
                user.DailyInfos.FirstOrDefault(k => k.CustomDate == nowCustomDate);
            if (today != null) return today;

            user.DailyInfos.Add(new DailySteamUserInfo(nowCustomDate));
            today = user.DailyInfos.First(k => k.CustomDate == nowCustomDate);
            return today;
        }


        private void RunMissingDayFix()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Random rnd = new Random();
                    foreach (var user in UserConfig.SteamUsers)
                    {
                        var date = new CustomDate(DateTime.Now);
                        var startDate = CustomDate.Parse(UserConfig.StartTime);
                        //var startDate = new CustomDate(DateTime.Now.AddDays(-2));
                        var y = startDate.Year;
                        var m = startDate.Month;
                        var d = startDate.Day;
                        while (y < date.Year ||
                               y == date.Year && m < date.Month ||
                               y == date.Year && m == date.Month && d <= date.Day)
                        {
                            _ = GetTodayInfo(user, new CustomDate(y, m, d));
                            //var sb = GetTodayInfo(user, new CustomDate(y, m, d));
                            //sb.HurtList.Add(new WeaponCell("Chrome Shotgun", rnd.Next(1, 101)));
                            //sb.HurtTimesList.Add(new WeaponCell("Chrome Shotgun", rnd.Next(1, 30)));
                            //sb.DamageList.Add(new WeaponCell("Chrome Shotgun", rnd.Next(1, 101)));
                            //sb.DamageTimesList.Add(new WeaponCell("Chrome Shotgun", rnd.Next(1, 30)));
                            //sb.OnlineTime = rnd.NextDouble() * 100;
                            //sb.SupportedYuan = rnd.Next(10);

                            var dt = new DateTime(y, m, d).AddDays(1);
                            y = dt.Year;
                            m = dt.Month;
                            d = dt.Day;
                        }
                    }

                    SaveConfig();
                    Thread.Sleep((int)TimeSpan.FromDays(1).TotalMilliseconds);
                }

            });

        }

        //private void SetRate()
        //{
        //    string currentDate = GetCustomDate(DateTime.Now);
        //    var nowDateInfo = GetDate(currentDate);
        //    var dateInfo = GetDate(UserConfig.StartTime);
        //    var offsetY = dateInfo.Year;
        //    var offsetM = dateInfo.Month;
        //    while (offsetY < nowDateInfo.Year ||
        //           offsetY == nowDateInfo.Year && offsetM <= nowDateInfo.Month)
        //    {
        //        string key = GetCustomDate(new DateInfo(offsetY, offsetM));

        //        double total = UserConfig.SteamUsers.Select(k =>
        //            (double)k.DailyInfos[key].SupportedYuan / k.DailyInfos[key].OnlineTime).Sum();
        //        foreach (var user in UserConfig.SteamUsers)
        //        {
        //            user.DailyInfos[key].Rate =
        //                ((double)user.DailyInfos[key].SupportedYuan / user.DailyInfos[key].OnlineTime) / total;
        //        }

        //        offsetM++;
        //        if (offsetM == 13)
        //        {
        //            offsetM = 1;
        //            offsetY += 1;
        //        }
        //    }
        //}

        private static void UpdateDisconnectInfo(SteamUser user)
        {
            user.LastDisconnect = DateTime.Now;
            user.IsOnline = false;
            var date = new CustomDate(DateTime.Now);
            var time = user.LastDisconnect - user.LastConnect;
            if (time != null)
            {
                DailySteamUserInfo todayinfo = GetTodayInfo(user, date);
                todayinfo.OnlineTime += time.Value.TotalMinutes;
            }

            Console.WriteLine($"OFFLINE: {user.CurrentName} ({user.SteamUid}) {user.LastDisconnect}");
        }

        private void Clear()
        {
            foreach (var user in UserConfig.SteamUsers.Where(k => k.IsOnline))
            {
                UpdateDisconnectInfo(user);
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            ConcurrentFile.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(UserConfig, Formatting.Indented));
            Console.WriteLine("Config saved.");
        }

        protected override Task<bool> CloseGracefully()
        {
            throw new NotImplementedException();
        }
    }
}
