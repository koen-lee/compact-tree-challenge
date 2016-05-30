using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class RealTrieTests
    {/*
        [Test]
        public void WriteLastItemWorks()
        {
            var storage = new byte[17];
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("test", 123);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                5,0,
                1,
                123,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void WriteLargeItemFails()
        {
            var storage = new byte[17];
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("test2", 123);
            Assert.IsFalse(result, "expected failure");
            CollectionAssert.AreEqual(new byte[17], storage);
        }

        [Test]
        public void WriteFirstItemWorks()
        {
            var storage = new byte[22];
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("test", 123);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                5,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,0,0,
                0
            }, storage);
        }
        /*
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

        */
        [Test]
        public void ReadFirstItemWorks()
        { 
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                5,0,
                1,
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
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                7,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',(byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("Working", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(99));
        }
        
        [Test]
        public void ReadFirstOfTwoItemsWorks()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                7,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',(byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("test", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(123));
        }


        [Test]
        public void ReadFirstOfSubItemsWorks()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9+16,0,
                3,
                123,0,0,0,
                0,0,0,0,

                3,0,
                (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("test", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(123));
        }

        [Test]
        public void ReadSecondOfSubItemsWorks()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9+16,0,
                3,
                123,0,0,0,
                0,0,0,0,

                3,0,
                (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("testing", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(99));
        }
        
        [Test]
        public void ReadSecondOfSubItems_with_empty_parentWorks()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                1+16+15,0,
                2,

                3,0,
                (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,

                2,0,
                (byte)'e',(byte)'r',
                9,0,
                1,
                66,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("tester", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(66));
        }

        [Test]
        public void Read_item_that_only_exists_as_subitem_fails()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                1+16+15,0,
                2,

                3,0,
                (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,

                2,0,
                (byte)'e',(byte)'r',
                9,0,
                1,
                66,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("ing", out value);
            Assert.IsFalse(result, "expected failure");
        }
        [Test]
        public void ReadNonExistingKeyFails()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                7,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',(byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("does not exist", out value);
            Assert.IsFalse(result, "expected failure");
        }
        
        [Test]
        public void ReadNonExistingKey_but_existing_prefix_does_Fails()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                7,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',(byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("test does not exist", out value);
            Assert.IsFalse(result, "expected failure");
        }
        /*
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
        }*/
    }
}