using System.Collections.Generic;

namespace GuidEx
{
    public interface IEncoder
    {
        string Encode(IEnumerable<byte> plain);
        IEnumerable<byte> Decode(string encoded);
    }
}