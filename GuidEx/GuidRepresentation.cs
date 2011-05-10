using System;
using System.Linq;

namespace GuidEx
{
    /*
     * The Goal: Build a Guid representation for System.Guid that:
     *   [1] is smaller (usually) than .ToString("N") would be (so <= 32 characters)
     *   [2] use extra bits to guards against typos (checksum)
     *   [3] is completely selected in windows via a dbl-clicking (so no puncuation allowed)
     * 
     * To do this I'm using a variable length encoding based on Base64. Since two values
     * in Base64 are typically puncuation we have to use multi-byte values to keep everything
     * in the [A-Za-z0-9] character set of 62 values. Hence 'Z','+','/' become "Za","Zb","Zc"
     * 
     * This typically results in 22-24 character guids. The worse case is 43 characters, but
     * it would be very rare. In 1024*1024 random guids typically you get some at 30 characters
     * 
     * The check sum is fairly simple, and has a 1:16 chance of detecting a typo.
     */

    public static class GuidRepresentation
    {
        private static readonly SelectableEncoding SelectableEncoding = new SelectableEncoding();

        public static Guid FromCompactString(string compactGuidString)
        {
            const int tokCount = 22;
            if (compactGuidString.Length < tokCount)
                throw new ArgumentException("compactGuidString to short to be valid");

            return new Guid(SelectableEncoding.Decode(compactGuidString).ToArray());
        }

        public static string ToCompactString(Guid g)
        {
            return SelectableEncoding.Encode(g.ToByteArray());
        }
    }
}