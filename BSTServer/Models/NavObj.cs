using System.Collections.Generic;
using System.Linq;

namespace BSTServer.Models
{
    public class NavObj
    {
        public List<SectionObj> Sections { get; set; } = new List<SectionObj>();
    }
    public class SectionObj
    {
        public static SectionObj CreateGeneral(params ItemObj[] items) => new SectionObj
        {
            Id = 1,
            IconString = '\uEE6F',
            Name = "总览",
            Tag = "sectionGeneral",
            Items = items.ToList()
        };

        public static SectionObj CreateManagement(params ItemObj[] items) => new SectionObj
        {
            Id = 2,
            IconString = '\uE912',
            Name = "管理",
            Tag = "sectionManagement",
            Items = items.ToList()
        };

        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public char IconString { get; set; }
        public List<ItemObj> Items { get; set; } = new List<ItemObj>();
    }

    public class ItemObj
    {
        public static ItemObj Dashboard => new ItemObj
        { Id = 1, Name = "仪表盘", IconString = '\uF246'/*'\uEC48'*/, Tag = "dashboard" };
        public static ItemObj Statistics => new ItemObj
        { Id = 2, Name = "数据", IconString = '\uED5E', Tag = "statistics" };

        public static ItemObj Server => new ItemObj
            { Id = 3, Name = "服务器", IconString = '\uE83B', Tag = "server" };
        public static ItemObj Files => new ItemObj
            { Id = 4, Name = "文件", IconString = '\uEC50', Tag = "files" };
        public static ItemObj Users => new ItemObj
            { Id = 5, Name = "用户", IconString = '\uE7EE', Tag = "users" };

        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public char IconString { get; set; }
    }
}
