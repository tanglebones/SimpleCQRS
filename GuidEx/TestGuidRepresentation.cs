using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace GuidEx
{
    [TestFixture]
    internal class TestGuidRepresentation
    {
        [Test]
        public void TestEmpty()
        {
            var g = Guid.Empty;
            var s = GuidRepresentation.ToCompactString(g);
            Assert.IsTrue(s.ToCharArray().All(c => c == '0'));
            var g1 = GuidRepresentation.FromCompactString(s);
            Assert.AreEqual(g, g1);
        }

        [Test]
        public void TestExceptions()
        {
            Assert.Throws(typeof (ArgumentException),
                          () => GuidRepresentation.FromCompactString("ThisHasAnInvalidChecksum"));
            Assert.Throws(typeof (NullReferenceException), () => GuidRepresentation.FromCompactString(null));
            Assert.Throws(typeof (ArgumentException), () => GuidRepresentation.FromCompactString("toshort"));
        }

        [Test]
        public void TestLargest()
        {
            var g = new Guid(new byte[] {255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255});
            var s = GuidRepresentation.ToCompactString(g);
            var g1 = GuidRepresentation.FromCompactString(s);
            Assert.AreEqual(g, g1);
        }

        [Test]
        public void TestOne()
        {
            var g = new Guid(new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            var s = GuidRepresentation.ToCompactString(g);
            var g1 = GuidRepresentation.FromCompactString(s);
            Assert.AreEqual(g, g1);
        }

        [Test]
        public void TestRandom()
        {
            const int count = 1024*1024;
            var total = 0;
            var max = 0;
            for (var i = 0; i < count; ++i)
            {
                var g = Guid.NewGuid();
                var s = GuidRepresentation.ToCompactString(g);
                if (i < 10)
                    Debug.WriteLine("Sample: " + s);
                var length = s.Length;
                total += length;
                if (length > max) max = length;
                var g1 = GuidRepresentation.FromCompactString(s);
                Assert.AreEqual(g, g1);
            }
            var avg = ((double) total)/count;
            Debug.WriteLine("avg length: " + avg);
            Debug.WriteLine("max length: " + max);
        }
    }
}