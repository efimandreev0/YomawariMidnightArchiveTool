using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YomawariARC
{
    internal class Entry
    {
        public string name {  get; set; }
        public long offset { get; set; }
        public long length { get; set; }
        public Entry(BinaryReader reader)
        {
            name = Utils.ReadString(reader, Encoding.UTF8);
            reader.BaseStream.Position += 0x30 - name.Length - 1;
            length = reader.ReadInt64();
            offset = reader.ReadInt64();
        }
    }
}
