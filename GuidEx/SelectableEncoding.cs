using System;
using System.Collections.Generic;
using System.Linq;

namespace GuidEx
{
    public class SelectableEncoding : IEncoder
    {
        public string Encode(IEnumerable<byte> bytes)
        {
            return Convert.ToBase64String(bytes.ToArray())
                .Replace("Z", "Za")
                .Replace("+", "Zb")
                .Replace("/", "Zc")
                .Replace("=", "Zd");
        }

        public IEnumerable<byte> Decode(string encoded)
        {
            return Convert.FromBase64String(
                encoded
                    .Replace("Zd", "=")
                    .Replace("Zc", "/")
                    .Replace("Zb", "+")
                    .Replace("Za", "Z"));
        }
    }
}