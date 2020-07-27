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
            IconString = "SAFD",
            Name = "总览",
            Tag = "sectionGeneral",
            Items = items.ToList()
        };

        public static SectionObj CreateManagement(params ItemObj[] items) => new SectionObj
        {
            Id = 2,
            IconString = "SAFD",
            Name = "管理",
            Tag = "sectionManagement",
            Items = items.ToList()
        };

        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string IconString { get; set; }
        public List<ItemObj> Items { get; set; } = new List<ItemObj>();
    }

    public class ItemObj
    {
        public static ItemObj Dashboard => new ItemObj
        { Id = 1, Name = "仪表盘", IconString = "1234", Tag = "dashboard" };
        public static ItemObj Statistics => new ItemObj
        { Id = 2, Name = "数据", IconString = "1234", Tag = "statistics" };

        public static ItemObj Server => new ItemObj
            { Id = 3, Name = "服务器", IconString = "1234", Tag = "server" };
        public static ItemObj Files => new ItemObj
            { Id = 4, Name = "文件", IconString = "1234", Tag = "files" };
        public static ItemObj Users => new ItemObj
            { Id = 5, Name = "用户", IconString = "1234", Tag = "users" };

        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string IconString { get; set; }
    }
}
