using System.Linq;
using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class RealTrieTests
    {
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
                9,0,
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
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,0,0,
                0
            }, storage);
        }
        
        [Test]
        public void WriteSecondItemWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0,0,0,0,
            };
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("Work", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void DeleteLastItemWorks()
        {
            var storage = new byte[]
            {
                4, 0,
                (byte) 't', (byte) 'e', (byte) 's', (byte) 't',
                9, 0,
                1,
                123, 0, 0, 0,
                0, 0, 0, 0,

                0, 0,
                0, 0, 0, 0,
                0, 0,
                0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            };
            var undertest = new RealTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEqual(new byte[]
            {
                0, 0,
                0, 0, 0, 0,
                0, 0,
                0,
                0, 0, 0, 0,
                0, 0, 0, 0,

                0, 0,
                0, 0, 0, 0,
                0, 0,
                0,
                0, 0, 0, 0,
                0, 0, 0, 0,

            }, storage);
        }
        
        [Test]
        public void WriteSecondItemWithSameSubstringWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0,0,0,0,
            };
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("testing", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                25,0,
                3,
                123,0,0,0,
                0,0,0,0,
                3,0,
                (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
                0
            }, storage);
        }


        [Test]
        public void Delete_last_child_works()
        {
            var storage = new byte[]
            {
                4, 0,
                (byte) 't', (byte) 'e', (byte) 's', (byte) 't',
                25, 0,
                3,
                123, 0, 0, 0,
                0, 0, 0, 0,
                3, 0,
                (byte) 'i', (byte) 'n', (byte) 'g',
                9, 0,
                1,
                99, 0, 0, 0,
                0, 0, 0, 0,
                0
            };
            var undertest = new RealTrie(storage);
            undertest.Delete("testing");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void Delete_parent_works()
        {
            var storage = new byte[]
            {
                4, 0,
                (byte) 't', (byte) 'e', (byte) 's', (byte) 't',
                25, 0,
                3,
                123, 0, 0, 0,
                0, 0, 0, 0,
                3, 0,
                (byte) 'i', (byte) 'n', (byte) 'g',
                9, 0,
                1,
                99, 0, 0, 0,
                0, 0, 0, 0,
                0
            };
            var undertest = new RealTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEqual(new byte[]
            {
                7,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t', (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0,
            }, storage);
        }


        [Test]
        public void WriteSecondItemWithSamePrefixWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0,
                0,0,0,0,
                0,0,
            };
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("team", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                2,0,
                (byte)'t',(byte)'e',
                31,0,
                2,
                2,0,
                (byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,
                2,0,
                (byte)'a',(byte)'m',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }

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
        public void ParseWithSubItemsWorks()
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
            int i = 0;
            var result = RealTrie.TrieItem.Read(undertest, ref i);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Key, Is.EqualTo("test"));
            Assert.That(result.HasChildren);
            Assert.That(result.HasValue);
            Assert.That(result.Value, Is.EqualTo(123));
            Assert.That(result.PayloadSize, Is.EqualTo(9+16));
            Assert.That(result.ItemSize, Is.EqualTo(2+4+2+9+16));

            var child = result.Children.Single();

            Assert.That(child, Is.Not.Null);
            Assert.That(child.Key, Is.EqualTo("ing"));
            Assert.That(child.HasChildren, Is.False);
            Assert.That(child.HasValue);
            Assert.That(child.Value, Is.EqualTo(99));
            Assert.That(child.PayloadSize, Is.EqualTo(9));
        }

        [Test]
        public void ParseWithValuelessParentItemsWorks()
        {
            var undertest = new RealTrie(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                1+16,0,
                2,

                3,0,
                (byte)'i',(byte)'n',(byte)'g',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            });
            int i = 0;
            var result = RealTrie.TrieItem.Read(undertest, ref i);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Key, Is.EqualTo("test"));
            Assert.That(result.HasChildren);
            Assert.That(result.HasValue, Is.False);
            Assert.That(result.PayloadSize, Is.EqualTo(1 + 16));
            Assert.That(result.ItemSize, Is.EqualTo(2 + 4 + 2 + 1 + 16));

            var child = result.Children.Single();

            Assert.That(child, Is.Not.Null);
            Assert.That(child.Key, Is.EqualTo("ing"));
            Assert.That(child.HasChildren, Is.False);
            Assert.That(child.HasValue);
            Assert.That(child.Value, Is.EqualTo(99));
            Assert.That(child.PayloadSize, Is.EqualTo(9));
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
        
        [Test]
        public void OverwriteKeyWorks()
        {
            var storage = new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new RealTrie(storage);
            var result = undertest.TryWrite("Work", 0xabcdef01);
            Assert.IsTrue(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
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
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new RealTrie(storage);
            undertest.Delete("Work");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                0,0,0,0,
                0,
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
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new RealTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,

                0,0,
                0,0,
                0,
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
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new RealTrie(storage);
            undertest.Delete("DoesNotExist");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4,0,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                9,0,
                1,
                123,0,0,0,
                0,0,0,0,

                4,0,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                9,0,
                1,
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void AddFiles()
        { 
            var undertest = new RealTrie();
            
            WriteAndReadBack(undertest, @"A-zip.ch", 92392);
            WriteAndReadBack(undertest, @"A-zip.dl", 88064);
            WriteAndReadBack(undertest, @"A-zip3", 56320);
            WriteAndReadBack(undertest, @"Az", 1478656);
        }

        private static void WriteAndReadBack(RealTrie undertest, string key, int v)
        {
            undertest.TryWrite(key, v);
            long value;
            Assert.That(undertest.TryRead(key, out value));
            Assert.That(value, Is.EqualTo(v));
        }
    }
}