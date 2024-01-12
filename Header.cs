using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YomawariARC
{
    internal class Header
    {
        public string magic { get; set; }
        public long count { get; set; }
        public Header(BinaryReader reader)
        {
            magic = Encoding.UTF8.GetString(reader.ReadBytes(8));
            count = reader.ReadInt64();
        }
    }
}
