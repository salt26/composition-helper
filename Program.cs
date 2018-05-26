using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class MIDI
    {
        private static String NumToStr(int x, int n = -1)
        {
            String r;
            if (n == -1)
            {
                r = ((char)(x & 127)).ToString();
                while (x > 127)
                {
                    x >>= 7;
                    r = ((char)(x & 127 | 128)).ToString() + r;
                }
                return r;
            }
            r = "";
            while (n-- > 0)
            {
                r = ((char)(x & 255)).ToString() + r;
                x >>= 8;
            }
            return r;
        }

        private static byte[] StrToByte(String s)
        {
            byte[] r = new byte[s.Length];
            for (int i = 0; i < s.Length; i++) r[i] = (byte)s[i];
            return r;
        }

        public static void Export(String filename, int BPM, List<Tuple<int, int> > notes)
        {
            FileStream fo = File.Open(filename, FileMode.Create);

            int track_num = 1;

            {
                byte[] header_byte = StrToByte("MThd" + NumToStr(6, 4) + NumToStr(1, 2) + NumToStr(track_num, 2) + NumToStr(12, 2));
                fo.Write(header_byte, 0, header_byte.Length);
            }

            {
                String track = "\x00\xff\x58\x04\x04\x02\x18\x08\x00\xff\x51\x03" + NumToStr(60000000 / BPM, 3);
                for (int i = 0; i < 8; i++)
                {
                    track += "\x00" + (char)(192 | i) + "\x00";
                }

                int ch = 0;
                foreach (Tuple<int, int> note in notes)
                {
                    track += "\x00" + (char)(144 | ch) + (char)note.Item2 + "\x40";
                    track += (char)note.Item1 + ((char)(128 | ch) + ((char)note.Item2 + "\x40"));
                    ch = ch + 1 & 7;
                }

                track += "\x00\xff\x2f\x00";

                byte[] track_byte = StrToByte("MTrk" + NumToStr(track.Length, 4) + track);
                fo.Write(track_byte, 0, track_byte.Length);
            }

            fo.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<int, int> > ls = new List<Tuple<int, int> >();

            ls.Add(new Tuple<int, int>(12, 48));
            ls.Add(new Tuple<int, int>(12, 48));
            ls.Add(new Tuple<int, int>(12, 55));
            ls.Add(new Tuple<int, int>(12, 55));
            ls.Add(new Tuple<int, int>(12, 57));
            ls.Add(new Tuple<int, int>(12, 57));
            ls.Add(new Tuple<int, int>(24, 55));

            ls.Add(new Tuple<int, int>(12, 53));
            ls.Add(new Tuple<int, int>(12, 53));
            ls.Add(new Tuple<int, int>(12, 52));
            ls.Add(new Tuple<int, int>(12, 52));
            ls.Add(new Tuple<int, int>(12, 50));
            ls.Add(new Tuple<int, int>(12, 50));
            ls.Add(new Tuple<int, int>(24, 48));

            MIDI.Export("test.mid", 120, ls);
        }
    }
}
