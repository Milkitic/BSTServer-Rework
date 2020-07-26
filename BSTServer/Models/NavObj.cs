using System.Collections.Generic;

namespace BSTServer.Models
{
    public class NavObj
    {
        public List<SectionObj> Sections { get; set; }
    }
    public class SectionObj
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string IconString { get; set; }
        public List<ItemObj> Items { get; set; }
    }

    public class ItemObj
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string IconString { get; set; }
    }
}
