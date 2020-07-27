using BstServer.Models;
using BSTServer.Data;
using BSTServer.Hosting.HostingBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BSTServer.Hosting
{
    public class L4D2AppHost : AppHost
    {
        private readonly ApplicationDbContext _dbContext;

        private Task _scanTask;
        private CancellationTokenSource _cts;
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        private readonly Dictionary<string, Session> _users = new Dictionary<string, Session>();

        public L4D2AppHost(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _cts = new CancellationTokenSource();
        }

        protected override Task OnDataReceived(object sender, OutputReceivedEventArgs e)
        {
            _queue.Enqueue(e.Data);
            return Task.CompletedTask;
        }

        protected override Task OnStarted()
        {
            _scanTask = Task.Factory.StartNew(ScanTask, TaskCreationOptions.LongRunning);
            return Task.CompletedTask;
        }

        protected override async Task OnExited()
        {
            _cts.Cancel();
            _queue.Clear();
            await _scanTask;
        }

        private async ValueTask ScanTask()
        {
            while (!_cts.IsCancellationRequested)
            {
                while (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out var current))
                    {
                        await Parse(current);
                    }
                }

                Thread.Sleep(10);
            }
        }

        private async ValueTask Parse(string source)
        {
            if (source == null) return;
            try
            {
                if (await DetectConnect(source)) return;
                if (await DetectDisconnect(source)) return;
                if (await DetectFriendlyFire(source)) return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async ValueTask<bool> DetectFriendlyFire(string source)
        {
            var obj = new FriendlyFireRegexObj();
            obj.Match(source);
            if (!obj.Success) return false;
            //var attacker = await _dbContext.GetSteamUserBySteamId(obj.Result.AttackerSteamIdOrBot);
            //var injurer = await _dbContext.GetSteamUserBySteamId(obj.Result.InjuredSteamIdOrBot);
            var result = obj.Result;
            var attackerSession = await _dbContext.GetSteamUserCurrentSession(result.AttackerSteamIdOrBot);
            var injurerSession = await _dbContext.GetSteamUserCurrentSession(result.InjuredSteamIdOrBot);
            attackerSession?.UserDamages?.Add(new SessionDamage
            {
                DamageTime = DateTimeOffset.Now,
                SessionDamageId = Guid.NewGuid(),
                IsHurt = false,
                SessionId = attackerSession.SessionId,
                SteamUserId = attackerSession.SteamUserId,
                Weapon = result.Weapon,
                Damage = result.Damage
            });

            injurerSession?.UserDamages?.Add(new SessionDamage
            {
                DamageTime = DateTimeOffset.Now,
                SessionDamageId = Guid.NewGuid(),
                IsHurt = true,
                SessionId = injurerSession.SessionId,
                SteamUserId = injurerSession.SteamUserId,
                Weapon = result.Weapon,
                Damage = result.Damage
            });

            Console.WriteLine($"FF: {attackerSession?.SteamUser?.Nickname} " +
                              $"=({result.Weapon})> " +
                              $"{injurerSession?.SteamUser?.Nickname} [{result.Damage} HP]");
            return true;
        }

        private async ValueTask<bool> DetectConnect(string source)
        {
            var obj = new ConnectRegexObj();
            obj.Match(source);
            if (!obj.Success) return false;
            var result = obj.Result;
            var exists = _dbContext.SteamUsers.AsNoTracking().Any(k => k.SteamUserId == result.SteamId);
            var steamUserSession = new Session()
            {
                SessionId = Guid.NewGuid(),
                ConnectTime = result.Timestamp,
                SteamUserId = result.SteamId
            };
            if (exists)
            {
                var steamUser = await _dbContext.SteamUsers.FirstAsync(k => k.SteamUserId == result.SteamId);
                steamUser.Nickname = result.Nickname;
                steamUser.IsOnline = true;
                steamUser.Sessions.Add(steamUserSession);
            }
            else
            {
                await _dbContext.SteamUsers.AddAsync(new SteamUser
                {
                    Sessions = { steamUserSession },
                    SteamUserId = result.SteamId,
                    IsOnline = true,
                    Nickname = result.Nickname
                });
            }

            await _dbContext.SaveChangesAsync();
            _users.Add(result.SteamId, steamUserSession);

            Console.WriteLine($"OFFLINE: {result.Nickname} ({result.SteamId}) {result.Timestamp}");
            return true;
        }

        private async ValueTask<bool> DetectDisconnect(string source)
        {
            var obj = new DisconnectRegexObj();
            obj.Match(source);
            if (!obj.Success) return false;
            var result = obj.Result;
            if (_users.ContainsKey(result.SteamId))
            {
                var session = _users[result.SteamId];
                var steamUser = await _dbContext.GetSteamUserBySteamId(result.SteamId);
                var dbSession = await _dbContext.GetSteamUserSessionById(result.SteamId, session.SessionId);
                dbSession.DisconnectTime = result.Timestamp;
                steamUser.IsOnline = false;
                await _dbContext.SaveChangesAsync();
                _users.Remove(result.SteamId);

                Console.WriteLine($"OFFLINE: {result.Nickname} ({result.SteamId}) {result.Timestamp}");
            }

            return true;
        }

        protected override Task<bool> CloseGracefully()
        {
            if (HostSettings.InputEnabled)
            {
                //ProcessExited += OnProcessExited;
                SendMessage($"quit{Environment.NewLine}");
                return Task.FromResult(true);
                //bool quit = false;
                //Stopwatch sw = Stopwatch.StartNew();
                //while (sw.ElapsedMilliseconds < 8000)
                //{
                //    if (quit) break;
                //    await Task.Delay(10);
                //}

                //return quit;

                //void OnProcessExited()
                //{
                //    quit = true;
                //    ProcessExited -= OnProcessExited;
                //}
            }

            return Task.FromResult(false);
        }
    }
}
