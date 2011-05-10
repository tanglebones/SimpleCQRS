using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuidEx
{
    public class SelectableEncoding : IEncoder
    {
        private static readonly IDictionary<string, byte> Decode6 = new Dictionary<string, byte>();

        private static readonly string[] Encode6 =
            new[]
                {
                    "0", "1", "2", "3", "4", "5", "6", "7",
                    "8", "9", "a", "b", "c", "d", "e", "f",
                    "g", "h", "i", "j", "k", "l", "m", "n",
                    "o", "p", "q", "r", "s", "t", "u", "v",
                    "w", "x", "y", "z", "A", "B", "C", "D",
                    "E", "F", "G", "H", "I", "J", "K", "L",
                    "M", "N", "O", "P", "Q", "R", "S", "T",
                    "U", "V", "W", "X", "Y", "Za", "Zb", "Zc"
                };

        static SelectableEncoding()
        {
            var i = 0;
            foreach (var c in Encode6)
            {
                Decode6[c] = (byte) i;
                ++i;
            }
        }

        public string Encode(byte[] raw)
        {
            var rawLen = raw.Length;
            var encLen = (rawLen + 2)/3*4;

            var checksum = raw.Aggregate(0, (current, t) => current ^ ((current*5) ^ t));

            var sb = new StringBuilder(encLen);

            var i = 0;
            var rawLen3 = (rawLen/3)*3;
            while (i < rawLen3)
            {
                // 11111100 00000000 00000000
                var a = (raw[i] & 0xFC) >> 2;
                // 00000011 11110000 00000000
                var i1 = i + 1;
                var b = ((raw[i] & 0x03) << 4) | ((raw[i1] & 0xF0) >> 4);
                // 00000000 00001111 11000000
                var i2 = i + 2;
                var c = ((raw[i1] & 0x0F) << 2) | ((raw[i2] & 0xC0) >> 6);
                // 00000000 00000000 00111111
                var d = raw[i2] & 0x3F;

                sb.Append(Encode6[a] + Encode6[b] + Encode6[c] + Encode6[d]);
                i += 3;
            }

            switch (rawLen - rawLen3)
            {
                case 0:
                    {
                        var a = ((checksum & 0xF0) >> 2);
                        sb.Append(Encode6[a]);
                    }
                    break;
                case 1:
                    {
                        // 11111100 00000000 00000000
                        var a = (raw[i] & 0xFC) >> 2;
                        // 00000011 CCCC0000 00000000
                        var b = ((raw[i] & 0x03) << 4) | ((checksum & 0xF0) >> 4);
                        sb.Append(Encode6[a] + Encode6[b]);
                    }
                    break;
                case 2:
                    {
                        // 11111100 00000000 00000000
                        var a = (raw[i] & 0xFC) >> 2;
                        // 00000011 11110000 00000000
                        var i1 = i + 1;
                        var b = ((raw[i] & 0x03) << 4) | ((raw[i1] & 0xF0) >> 4);
                        // 00000000 00001111 CC000000
                        var c = ((raw[i1] & 0x0F) << 2) | ((checksum & 0xC0) >> 6);
                        // 00000000 00000000 00CC0000
                        var d = checksum & 0x30;
                        sb.Append(Encode6[a] + Encode6[b] + Encode6[c] + Encode6[d]);
                    }
                    break;
                default:
                    throw new Exception("Should never happen");
            }

            return sb.ToString();
        }

        public IEnumerable<byte> Decode(string encoded)
        {
            var ba = new List<byte>();
            if (encoded.Length == 0) return ba;

            var ca = encoded.ToCharArray();
            var tokCount = ca.Length;
            byte checksum;
            var ta = new int[tokCount + 3];
            {
                var i = 0;
                var j = 0;
                do
                {
                    var tok = ca[j].ToString();
                    if (ca[j] == 'Z')
                    {
                        --tokCount;
                        tok = tok + ca[j + 1];
                        ++j;
                    }
                    ta[i] = Decode6[tok];
                    ++i;
                    ++j;
                } while (i < tokCount);
            }

            for (var i = tokCount; i < ta.Length; ++i)
            {
                ta[i] = 0;
            }

            var tokCount4 = (tokCount - 1)/4*4;
            for (var i = 0; i < tokCount4; i += 4)
            {
                // xx111111 xx110000 xx000000 xx000000
                var i1 = i + 1;
                ba.Add((byte) ((ta[i] << 2) | ((ta[i1] & 0x30) >> 4)));
                // xx000000 xx001111 xx111100 xx000000
                var i2 = i + 2;
                ba.Add((byte) (((ta[i1] & 0xF) << 4) | ((ta[i2] & 0x3C) >> 2)));
                // xx000000 xx000000 xx000011 xx111111
                ba.Add((byte) (((ta[i2] & 0x03) << 6) | (ta[i + 3] & 0x3F)));
            }

            switch (tokCount - tokCount4)
            {
                case 1:
                    {
                        checksum = (byte) ((ta[tokCount4] << 2) & 0xF0);
                    }
                    break;
                case 2:
                    {
                        var i1 = tokCount4 + 1;
                        ba.Add((byte) ((ta[tokCount4] << 2) | ((ta[i1] & 0x30) >> 4)));
                        checksum = (byte) (((ta[i1] & 0xF) << 4));
                    }
                    break;
                case 3:
                    {
                        var i1 = tokCount4 + 1;
                        ba.Add((byte) ((ta[tokCount4] << 2) | ((ta[i1] & 0x30) >> 4)));
                        var i2 = tokCount4 + 2;
                        ba.Add((byte) (((ta[i1] & 0xF) << 4) | ((ta[i2] & 0x3C) >> 2)));
                        checksum = (byte) (((ta[i2] & 0x03) << 6) + (ta[tokCount4 + 3] & 0x30));
                    }
                    break;
                case 4:
                    {
                        var i1 = tokCount4 + 1;
                        ba.Add((byte) ((ta[tokCount4] << 2) | ((ta[i1] & 0x30) >> 4)));
                        var i2 = tokCount4 + 2;
                        ba.Add((byte) (((ta[i1] & 0xF) << 4) | ((ta[i2] & 0x3C) >> 2)));
                        checksum = (byte) (((ta[i2] & 0x03) << 6) | (ta[tokCount4 + 3] & 0x30));
                    }
                    break;
                default:
                    throw new Exception("Should never happen");
            }

            var t = ba.Aggregate(0, (current, t2) => current ^ ((current*5) ^ t2));

            var t1 = (t & 0xf0);
            if (t1 != checksum)
                throw new ArgumentException("compactGuidString checksum failed");

            return ba;
        }
    }
}