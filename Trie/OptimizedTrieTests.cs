using System.Linq;
using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class OptimizedTrieTests
    {
        [Test]
        public void WriteLastItemWorks()
        {
            var storage = new byte[13];
            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("test", 123);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void WriteLargeItemFails()
        {
            var storage = new byte[13];
            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("test2", 123); //1+5+8 bytes
            Assert.IsFalse(result, "expected failure");
            CollectionAssert.AreEqual(new byte[13], storage);
        }


        [Test]
        public void WriteFirstItemWorks()
        {
            var storage = new byte[18];
            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("test", 123);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
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
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,
                0,
            };
            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("Work", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
                0,0,
                0,
            }, storage);
        }

        [Test]
        public void DeleteLastItemWorks()
        {
            var storage = new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0, 0,
                0, 0, 0, 0,
                0, 0,
                0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            };
            var undertest = new OptimizedTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEqual(new byte[]
            {
                0,
                0, 0, 0, 0,
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
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0,0,0,0
            };
            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("testing", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4|1<<7|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                12,0,

                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,
                0,0,0,
            }, storage);
        }


        [Test]
        public void Delete_last_child_works()
        {
            var storage = new byte[]
            {
                4|1<<7|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                12,0,

                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,
                0,0,0,
            };
            var undertest = new OptimizedTrie(storage);
            undertest.Delete("testing");
            CollectionAssert.AreEqual(new byte[]
            {

                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                0,0,

                0,
                0,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,
            }, storage);
        }

        [Test]
        public void Delete_parent_works()
        {
            var storage = new byte[]
            {
                4|1<<7|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                12,0,

                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,
                0,0,0,
            };
            var undertest = new OptimizedTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEqual(new byte[]
            {
                7|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t', (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,

                0,0,
                0,0,0,0,
                0,0,
                0,
                0,0,0,0,
                0
            }, storage);
        }


        [Test]
        public void WriteSecondItemWithSamePrefixWorks()
        {
            var storage = new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0, 0,
                0, 0, 0, 0,
                0, 0,
                0,
                0, 0, 0,
                0, 0
            };

            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("team", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                2|1<<6,
                (byte)'t',(byte)'e',
                22, 0,
                2|1<<7,
                (byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                2|1<<7,
                (byte)'a',(byte)'m',
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }


        [Test]
        public void WriteSecondItemWithJustSamePrefixWorks()
        {
            var storage = new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                0, 0,
                0, 0, 0, 0,
                0, 0,
                0,
                0, 0, 0
            };

            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("te", 99);
            Assert.That(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                2|1<<6|1<<7,
                (byte)'t',(byte)'e',
                99,0,0,0,
                0,0,0,0,
                11, 0,
                2|1<<7,
                (byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                0,
            }, storage);
        }

        [Test]
        public void ReadFirstItemWorks()
        {
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<7,
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
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                7|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',(byte)'i',(byte)'n',(byte)'g',
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
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                7|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',(byte)'i',(byte)'n',(byte)'g',
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
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<7|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                12,0,

                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,
                0,0,0,
            });

            long value;
            var result = undertest.TryRead("test", out value);
            Assert.That(result, "expected success");
            Assert.That(value, Is.EqualTo(123));
        }

        [Test]
        public void ParseWithSubItemsWorks()
        {
            var undertest = new byte[]
            {
                4 | 1 << 7 | 1 << 6,
                (byte) 't', (byte) 'e', (byte) 's', (byte) 't',
                123, 0, 0, 0,
                0, 0, 0, 0,
                12, 0,

                3 | 1 << 7,
                (byte) 'i', (byte) 'n', (byte) 'g',
                99, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0,
            };
            int i = 0;
            var result = OptimizedTrie.TrieItem.Read(undertest, ref i);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Key.ToStringUtf8(), Is.EqualTo("test"));
            Assert.That(result.HasChildren);
            Assert.That(result.HasValue);
            Assert.That(result.Value, Is.EqualTo(123));
            Assert.That(result.PayloadSize, Is.EqualTo(12));
            Assert.That(result.ItemSize, Is.EqualTo(13+2+12));

            var child = result.Children.Single();

            Assert.That(child, Is.Not.Null);
            Assert.That(child.Key.ToStringUtf8(), Is.EqualTo("ing"));
            Assert.That(child.HasChildren, Is.False);
            Assert.That(child.HasValue);
            Assert.That(child.Value, Is.EqualTo(99));
            Assert.That(child.PayloadSize, Is.EqualTo(0));
        }

        [Test]
        public void ParseWithValuelessParentItemsWorks()
        {
            var undertest = new byte[]
            {
                4|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                1+3+8,0,
                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,
            };
            int i = 0;
            var result = OptimizedTrie.TrieItem.Read(undertest, ref i);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Key.ToStringUtf8(), Is.EqualTo("test"));
            Assert.That(result.HasChildren);
            Assert.That(result.HasValue, Is.False);
            Assert.That(result.PayloadSize, Is.EqualTo(12));
            Assert.That(result.ItemSize, Is.EqualTo(1 + 4 + 2 + 12));

            var child = result.Children.Single();

            Assert.That(child, Is.Not.Null);
            Assert.That(child.Key.ToStringUtf8(), Is.EqualTo("ing"));
            Assert.That(child.HasChildren, Is.False);
            Assert.That(child.HasValue);
            Assert.That(child.Value, Is.EqualTo(99));
            Assert.That(child.PayloadSize, Is.EqualTo(0));
        }

        [Test]
        public void ReadSecondOfSubItemsWorks()
        {
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<6|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,
                1 +3+8,0,
                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
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
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                1+3+8+1+2+8,0,
                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,

                2|1<<7,
                (byte)'e',(byte)'r',
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
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                1+3+8+1+2+8,0,
                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,
                2|1<<7,
                (byte)'e',(byte)'r',
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
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                12,0,
                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,

                2|1<<7,
                (byte)'e',(byte)'r',
                66,0,0,0,
                0,0,0,0,
            });
            long value;
            var result = undertest.TryRead("does not exist", out value);
            Assert.IsFalse(result, "expected failure");
        }

        [Test]
        public void ReadNonExistingKey_but_existing_prefix_does_Fails()
        {
            var undertest = new OptimizedTrie(new byte[]
            {
                4|1<<6,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                12,0,
                3|1<<7,
                (byte)'i',(byte)'n',(byte)'g',
                99,0,0,0,
                0,0,0,0,

                2|1<<7,
                (byte)'e',(byte)'r',
                66,0,0,0,
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
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new OptimizedTrie(storage);
            var result = undertest.TryWrite("Work", 0xabcdef01);
            Assert.IsTrue(result, "expected success");
            CollectionAssert.AreEqual(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
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
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new OptimizedTrie(storage);
            undertest.Delete("Work");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
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
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new OptimizedTrie(storage);
            undertest.Delete("test");
            CollectionAssert.AreEquivalent(new byte[]
            {
                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
                
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
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            };
            var undertest = new OptimizedTrie(storage);
            undertest.Delete("DoesNotExist");
            CollectionAssert.AreEquivalent(new byte[]
             {
                4|1<<7,
                (byte)'t',(byte)'e',(byte)'s',(byte)'t',
                123,0,0,0,
                0,0,0,0,

                4|1<<7,
                (byte)'W',(byte)'o',(byte)'r',(byte)'k',
                99,0,0,0,
                0,0,0,0,
            }, storage);
        }

        [Test]
        public void AddFiles()
        {
            var undertest = new OptimizedTrie();

            WriteAndReadBack(undertest, @"A-zip.ch", 92392);
            WriteAndReadBack(undertest, @"A-zip.dl", 88064);
            WriteAndReadBack(undertest, @"A-zip3", 56320);
            WriteAndReadBack(undertest, @"Az", 1478656);
        }

        [Test]
        public void AddFiles_readback_works()
        {
            var trie = new OptimizedTrie();
            
            trie.TryWrite(@"dll.hpsign", 256);
            trie.TryWrite(@"dll", 99);
            long value;
            Assert.That(trie.TryRead(@"dll", out value));
        }


        [Test]
        //[Timeout(200)]
        public void AddFiles_readback_fails()
        {
            var undertest = new OptimizedTrie();
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\el\WinMagicSecurityPlugin.resources.dll", 17088);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\lv\HPDriveEncryption.chm", 46492);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\es\DPTokens01.dll.mui", 20992);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\he\WinMagic.HP.SecurityManagerPlugin.resources.dll", 24256);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\lt\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\KeyFile\1033\sql_common_core_loc_keyfile.dll", 57024);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\et\WinMagic.HP.SecurityManagerPlugin.resources.dll", 22720);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\h2.bin", 19968);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\es\HPDriveEncryption.chm", 46804);
        }



        private static void WriteAndReadBack(ITrie undertest, string key, int v)
        {
            undertest.TryWrite(key, v);
            long value;
            Assert.That(undertest.TryRead(key, out value));
            Assert.That(value, Is.EqualTo(v));
        }
    }
}