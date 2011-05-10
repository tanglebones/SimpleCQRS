using System.Collections.Generic;

namespace GuidEx
{
    public interface IEncoder
    {
        string Encode(byte[] plain);
        IEnumerable<byte> Decode(string encoded);
    }
}