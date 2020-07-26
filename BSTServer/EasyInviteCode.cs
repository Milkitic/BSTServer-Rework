using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BSTServer
{
    public class EasyInviteCode
    {
        private static Dictionary<char, char> _mapDic;
        private static Dictionary<char, char> _mapDicBack;
        private static DateTime _dateTime = DateTime.Parse("1996/9/20");

        private static Dictionary<char, char> _mapIntDic = new Dictionary<char, char>();
        private static Dictionary<char, char> _mapIntDicBack = new Dictionary<char, char>();
        static EasyInviteCode()
        {
            var random = new Random((int)_dateTime.Ticks);
            var origin = Enumerable.Range(65, 90 - 65 + 1).Concat(Enumerable.Range(97, 122 - 97 + 1)).Select(k => (byte)k).ToList();
            var target = origin.OrderBy(k => random.Next()).ToList();
            var mapDic = new Dictionary<char, char>();
            for (var i = 0; i < origin.Count; i++)
            {
                var x = origin[i];
                var y = target[i];
                mapDic.Add((char)x, (char)y);
            }

            _mapDic = mapDic;
            _mapDicBack = mapDic.ToDictionary(k => k.Value, k => k.Key);

            var target2 = target.OrderBy(k => random.Next()).ToList();
            for (int i = 0; i < 10; i++)
            {
                var c = (char)target2[i];
                var j = char.Parse(i.ToString());
                _mapIntDic.Add(j, c);
                _mapIntDicBack.Add(c, j);
            }
        }

        public static string Generate(string user, DateTime expire)
        {
            var str = ShuffleString(user);
            var ticks = ShuffleLong((expire - _dateTime).Ticks);
            return str + "&" + ticks;
        }

        private static string ShuffleLong(in long ticks)
        {
            var str = ticks.ToString();
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                sb.Append(_mapIntDic[c]);
            }

            return sb.ToString();
        }

        private static string ShuffleString(string user)
        {
            var sb = new StringBuilder();
            foreach (var c in user)
            {
                sb.Append(_mapDic.ContainsKey(c) ? _mapDic[c] : c);
            }

            return sb.ToString();
        }

        private static string RecoverString(string code)
        {
            var sb = new StringBuilder();
            foreach (var c in code)
            {
                sb.Append(_mapDicBack.ContainsKey(c) ? _mapDicBack[c] : c);
            }

            return sb.ToString();
        }

        public static (string user, DateTime expire) ConvertBack(string str)
        {
            var g = str.Split('&');
            if (g.Length != 2) throw new Exception();
            var user = RecoverString(g[0]);
            var datetime = RecoverDateTime(g[1]);
            return (user, datetime);
        }

        private static DateTime RecoverDateTime(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                sb.Append(_mapIntDicBack[c]);
            }

            var val = long.Parse(sb.ToString());
            return new DateTime(val + _dateTime.Ticks);
        }
    }
}
