using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace GuidEx
{
    [TestFixture]
    internal class TestSelectableEncoding
    {
        [Test]
        public void TestDifferenceLengths()
        {
            var se = new SelectableEncoding();
            foreach (
                var s in
                    new[]
                        {"a", "aa", "aaa", "aaaa", "aaaaa", "aaaaaa", "aaaaaaa", "aaaaaaaa", "aaaaaaaaa", "aaaaaaaaaa"})
            {
                var e0 = se.Encode(Encoding.UTF8.GetBytes(s));
                var d = se.Decode(e0);
                var e1 = se.Encode(d.ToArray());
                Assert.AreEqual(e0, e1);
            }
        }

        [Test]
        public void TestEmpty()
        {
            var se = new SelectableEncoding();
            var s = se.Encode(new byte[] {});
            Assert.AreEqual(s, "0");
        }

        [Test]
        public void TestExceptions()
        {
            var se = new SelectableEncoding();
            Assert.Throws(typeof (NullReferenceException), () => se.Encode(null));
        }

        [Test]
        public void TestOne()
        {
            var se = new SelectableEncoding();
            var s0 = se.Encode(new byte[] {0});
            Assert.AreEqual(s0, "00");
            var s1 = se.Encode(new byte[] {1});
            Assert.AreEqual(s1, "0g");
        }

        [Test]
        public void TestRandom()
        {
            var r = new Random(0);
            var se = new SelectableEncoding();
            for (var i = 0; i < 1024*1024; ++i)
            {
                var len = r.Next() & 63;
                var bytes = new byte[len];
                r.NextBytes(bytes);
                var e0 = se.Encode(bytes);
                var d = se.Decode(e0);
                var e1 = se.Encode(d.ToArray());
                Assert.AreEqual(e0, e1);
            }
        }
    }
}