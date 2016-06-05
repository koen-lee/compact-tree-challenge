using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class BufferpartTests
    {
        [Test]
        public void SubstringWorks()
        {
            var buffer = new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            };
            var undertest = new Bufferpart(buffer, 1, 7);
            var result = undertest.Substring(3);
            var bytes = result.GetBytes();
            CollectionAssert.AreEqual(new byte[]
            {
                4, 5, 6, 7
            }, bytes);
        }

        [Test]
        public void OperatorEqualsWorks()
        {
            var undertest1 = new Bufferpart(new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            }, 1, 7);

            var unequal1 = new Bufferpart(new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            }, 2, 7);

            var equal1 = new Bufferpart(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7
            });

#pragma warning disable CS1718 // Comparison made to same variable
            // ReSharper disable once EqualExpressionComparison
            Assert.That(undertest1 == undertest1);
#pragma warning restore CS1718 // Comparison made to same variable

            Assert.That(undertest1 == equal1);
            Assert.That(undertest1 != unequal1);
        }

        [Test]
        public void SubstringWithLengthWorks()
        {
            var buffer = new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            };
            var undertest = new Bufferpart(buffer, 1, 7);
            var result = undertest.Substring(3, 2);
            var bytes = result.GetBytes();
            CollectionAssert.AreEqual(new byte[]
            {
                4, 5
            }, bytes);
        }

        [Test]
        public void ToBytesWorks()
        {
            var buffer = new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            };
            var undertest = new Bufferpart(buffer, 1, 7);
            var bytes = undertest.GetBytes();
            CollectionAssert.AreEqual(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7
            }, bytes);
        }

        [Test]
        public void AddWorks()
        {
            var left = new Bufferpart(new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            }, 1, 7);
            var right = new Bufferpart(new byte[]
            {
                99, 11, 8, 9, 14, 15, 16, 17, 88
            }, 2, 2);

            var bytes = (left + right).GetBytes();
            CollectionAssert.AreEqual(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9
            }, bytes);
        }

        [Test]
        public void StartsWithWorks()
        {
            var buffer_1234567 = new Bufferpart(new byte[]
            {
                99, 1, 2, 3, 4, 5, 6, 7, 88
            }, 1, 7);

            var buffer_123 = new Bufferpart(new byte[]
            {1, 2, 3});

            Assert.That(buffer_1234567.StartsWith(buffer_123));
            Assert.That(buffer_1234567.StartsWith(buffer_1234567));
            Assert.IsFalse(buffer_123.StartsWith(buffer_1234567));
        }
    }
}