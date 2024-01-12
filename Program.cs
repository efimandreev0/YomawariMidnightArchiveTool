using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace YomawariARC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileAttributes attr = File.GetAttributes(args[0]);
            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Rebuild(args[0]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string arc)
        {
            var reader = new BinaryReader(File.OpenRead(arc));
            Header header = new Header(reader);
            Entry[] entries = new Entry[header.count];
            string dir = Path.GetFileNameWithoutExtension(arc) + "//";
            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(arc));
            for (int i = 0; i < header.count; i++)
            {
                entries[i] = new Entry(reader);
                var pos = reader.BaseStream.Position;
                reader.BaseStream.Position = entries[i].offset;
                byte[] bytes = reader.ReadBytes((int)entries[i].length);
                reader.BaseStream.Position = pos;
                File.WriteAllBytes(dir + entries[i].name, bytes);
            }
        }
        public static void Rebuild(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            long[] offsets = new long[files.Length];
            string[] names = new string[files.Length];
            long[] length = new long[files.Length];
            using (BinaryWriter writer = new BinaryWriter(File.Create(dir + ".arc")))
            {
                writer.Write(Encoding.UTF8.GetBytes("PS_FS_V1"));
                writer.Write((long)files.Length);
                writer.Write(new byte[files.Length * 0x40 + 0x230]);
                for (int i = 0; i < files.Length; i++)
                {
                    offsets[i] = writer.BaseStream.Position;
                    names[i] = Path.GetFileName(files[i]);
                    byte[] bytes = File.ReadAllBytes(files[i]);
                    length[i] = bytes.Length;
                    writer.Write(bytes);
                    writer.Write(new byte[0x148]);
                }
                writer.BaseStream.Position = 0x10;
                for (int i = 0; i < files.Length; i++)
                {
                    writer.Write(Encoding.UTF8.GetBytes(names[i]));
                    writer.BaseStream.Position += (0x30 - names[i].Length);
                    writer.Write(length[i]);
                    writer.Write(offsets[i]);
                }
            }
        }
    }
}
