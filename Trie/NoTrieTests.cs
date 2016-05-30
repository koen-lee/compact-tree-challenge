using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class NoTrieTests
    {
        [Test]
        public void WriteLastItemWorks()
        {
            var storage = new byte[14];
            var undertest = new NoTrie(storage);
            var result = undertest.TryWrite("test", 123);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0
            }, storage);
        }

        [Test]
        public void WriteLargeItemFails()
        {
            var storage = new byte[14];
            var undertest = new NoTrie(storage);
            var result = undertest.TryWrite("test2", 123);
            Assert.IsFalse(result, "expected failure");
            CollectionAssert.AreEqual(new byte[]
            {
                0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0
            }, storage);
        }

        [Test]
        public void WriteFirstItemWorks()
        {
            var storage = new byte[22];
            var undertest = new NoTrie(storage);
            var result = undertest.TryWrite("test", 123);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                
                0,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void WriteSecondItemWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
            };
            var undertest = new NoTrie(storage);
            var result = undertest.TryWrite("Work", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }


        [Test]
        public void ReadFirstItemWorks()
        { 
            var undertest = new NoTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("test", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(123));
        }

        [Test]
        public void ReadSecondItemWorks()
        {
            var undertest = new NoTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("Work", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(99));
        }

        [Test]
        public void ReadFirstOfTwoItemsWorks()
        {
            var undertest = new NoTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,0,0,
                0,0,0,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("test", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(123));
        }

        [Test]
        public void ReadNonExistingKeyFails()
        {
            var undertest = new NoTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("does not exist", out value);
            Assert.IsFalse(result, "expected failure");
        }


        [Test]
        public void OverwriteKeyWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new NoTrie(storage);
            long value;
            var result = undertest.TryWrite("Work", 0xabcdef01);
            Assert.IsTrue(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                0x01,0xef,0xcd,0xab,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void DeleteSecondKeyWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new NoTrie(storage);
            undertest.Delete("Work");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void DeleteFirstKeyWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new NoTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
            }, storage);
        }


        [Test]
        public void DeleteNonExistingKeySilentlyFails()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new NoTrie(storage);
            undertest.Delete("DoesNotExist");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }
    }
}