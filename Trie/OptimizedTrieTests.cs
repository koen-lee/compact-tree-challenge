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
        public void AddFiles_readback_fails()
        {
            var undertest = new OptimizedTrie();
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extensions_Msi_OrderedPackageDependencyList.xml", 68889);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\it\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\Cartridges\Sybase.xsl", 30369);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\en\Web Deploy UI.chm", 218035);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\el\WinMagicSecurityPlugin.resources.dll", 17088);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\it\WinMagicSecurityPlugin.resources.dll", 15040);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\msadc\msadcer.dll", 8192);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\TH-TH\amhelp.chm", 160246);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\sqlscm.dll", 59584);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPWbfDevice.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Travel\PassportMask_PAL.wmv", 29268);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\pl\msdeploy.resources.dll", 68240);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\401-4.htm", 1382);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\12.0\TFSFieldMapping.exe", 41152);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\System.Net.ni.dll", 869888);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\eu\mscorrc.dll", 9952);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\500-13.htm", 1195);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\pt.txt", 14007);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\pt-BR\DPTokensCard.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\IIS Express\en-us\appcmd.exe.mui", 9872);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\th\WinMagic.HP.SecurityManagerPlugin.resources.dll", 29888);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\no\DPTokensBluetooth.dll.mui", 5120);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\zh-Hans\DPTokensSpareKey.dll.mui", 10752);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\lv\HPDriveEncryption.chm", 46492);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_SQLServerSCP_Private.xml", 202);
            undertest.TryWrite(@"C:\Program Files\Microsoft Help Viewer\v1.0\dev10.mshc", 9467114);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Settings.xml", 20424);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\bg-BG\tipresx.dll.mui", 4096);
            undertest.TryWrite(@"C:\Program Files\IIS Express\license.rtf", 10658);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\SDK\Assemblies\en\Microsoft.SqlServer.ConnectionInfo.xml", 134161);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\ja\system.resources.dll", 11504);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_Slp_Private.xml", 197);
            undertest.TryWrite(@"C:\Program Files\IIS Express\WCF35Setup.js", 19478);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\uk\mscorlib.resources.dll", 12024);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPRsaTokPdr.dll", 1604944);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\lt\DPUserPolicies.resources.dll", 14672);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\HU-HU\amhelp.chm", 154395);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\14.0\Microsoft.TeamFoundation.VersionControl.Common.dll", 268096);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\th\mscorlib.resources.dll", 12536);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Full\dotslightoverlay.png", 4669);
            undertest.TryWrite(@"C:\Program Files\Microsoft Office\Office15\1043\officeinventoryagentfallback.xml", 3430);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\MsMpCom.dll", 104200);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sl\DPTokensCard.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\412.htm", 1430);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\404-7.htm", 1127);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\Drivers\Backup\NisDrv\NisDrvWFP.cat", 8354);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Settings.xml", 13180);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Stationery\Roses.htm", 233);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\ky.txt", 19726);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\BabyGirl\babypink.png", 19477);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\TortoiseOverlays.dll", 75544);
            undertest.TryWrite(@"C:\Program Files\IIS Express\dirlist.dll", 30864);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Vignette\whiteband.png", 7261);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\WF\amd64\WorkflowDebugHost.exe", 135368);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\fsdefinitions\keypad.xml", 727);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\12.0\Microsoft.TeamFoundation.Sync.ProjectServerApi.dll", 2002752);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\lt-LT\tipresx.dll.mui", 4096);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\bg\DpFillin.dll.mui", 16896);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\pl\Microsoft.Web.PlatformInstaller.UI.resources.dll", 51872);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\LogSet_KOEN_20141203_081100.cab", 58171);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VSTO\10.0\Microsoft Visual Studio 2010 Tools for Office Runtime (x64)\install.res.1053.dll", 49832);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Windows Simulator\11.0\SensorsSimulatorDriver.inf", 5998);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\SV-SE\setupres.dll.mui", 43080);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\BabyBoy\BabyBoyMainToNotesBackground.wmv", 141214);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabIpsps.dll", 40448);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\Datastore_Discovery.xml", 117027);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1028\xesqlminpkg.rll", 91328);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1040\odsole70.rll", 30400);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\it\DPFPToken.dll.mui", 10752);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\xesqlpkg.mof", 421661);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\KeyFile\1033\sql_engine_core_shared_loc_keyfile.dll", 57024);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\ET-EE\setupres.dll.mui", 45152);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\fsdefinitions\keypad\keypadbase.xml", 1118);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_ClusterIPAddresses_Private.xml", 510);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\COM\AXSCPHST.DLL", 67264);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DpOFeedb.dll", 862544);
            undertest.TryWrite(@"C:\Program Files\Intel\Media SDK\vp_w7_32.vp", 15229);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\Microsoft.SQL.Chainer.PackageData.dll", 407400);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VS7Debug\VSDebugScriptAgent110.dll", 159232);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\Cartridges\sqlpdw.xsl", 77495);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\Ole DB\en-US\oledb32r.dll.mui", 47616);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\ipssrl.xml", 2596);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\mssqlsystemresource.mdf", 41943040);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\SymSrv.yes", 1);
            undertest.TryWrite(@"C:\Program Files\IIS Express\ja-JP\appobj.dll.mui", 55440);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_SlpDumper_Private.xml", 103);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\0x0415.ini", 24262);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPPS3.dll", 212304);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_Sku_Public.xml", 143);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Detail.txt", 99189);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\rectangle_performance_Thumbnail.bmp", 5072);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\ResizingPanels\NavigationUp_ButtonGraphic.png", 4955);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\it\WinMagic.HP.SecurityManagerPlugin.resources.dll", 23744);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_PerfCounterDllTargetSystemFilePath.xml", 75);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Circle_SelectionSubpictureA.png", 3878);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.SString.dll", 44120);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_AgentScript_Private.xml", 107);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\mshwLatin.dll", 1071616);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\CommonProperties.xml", 5356);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Travel\button-highlight.png", 716);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\Resources\1033\xeclrhostpkg.rll", 18432);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\Resources\1033\dtshost.rll", 18624);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\SDPEFilter.exe", 242176);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\zh-Hant\DpMiniOnlineIds.dll.mui", 3584);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\3082\msxmlsql.rll", 66752);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP Recovery Disc Creator\HPRDC.exe", 3840824);
            undertest.TryWrite(@"C:\Program Files\Internet Explorer\SIGNUP\map_bing.xml", 1025);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1040\xeclrhostpkg.rll", 20160);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\MappingFiles\SqlClient9ToMSSql8.xml", 11324);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_AgentServiceBinaryFile.xml", 115);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\ro\WinMagicSecurityPlugin.resources.dll", 15040);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\license.rtf", 130123);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\PipelineComponents\TxSort.dll", 240832);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\zh-Hans\DpUTrain.dll.mui", 11264);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\LV-LV\eula.rtf", 162252);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Blip\LockedIcon.ico", 33385);
            undertest.TryWrite(@"C:\Program Files\Microsoft Help Viewer\v1.0\HelpLibManager.exe.config", 641);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\Microsoft.SqlServer.Configuration.Cluster.dll", 694120);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\el\DPTokens01.dll.mui", 23040);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\ko-KR\tipresx.dll.mui", 3584);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\Patch\rs2000.msp", 24064);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_SqlLegacyDiscovery_Private.xml", 130);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\SetupARP.exe.config", 331);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Pets\Pets_btn-previous-static.png", 2019);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\fsdefinitions\main\base_heb.xml", 738);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\LayeredTitles\203x8subpicture.png", 2820);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Subclipse\ConflictIcon.ico", 22486);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\DechenesVista\ConflictIcon.ico", 51083);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\1033\license_Web.rtf", 27124);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\DTSPERF.INI", 4470);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\Ole DB\en-US\sqlxmlx.rll.mui", 17920);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Subclipse\DeletedIcon.ico", 2734);
            undertest.TryWrite(@"C:\Program Files\Microsoft Help Viewer\v1.0\Microsoft Help Viewer 1.1\HELP3_VS.MSI", 519680);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\fsdefinitions\osknumpad.xml", 219);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\sqlsval.dll", 1403752);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\OfficeSoftwareProtectionPlatform\OSPPCEXT.DLL", 1833560);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\he\DpFillin.dll.mui", 13824);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\SpecialOccasion\SpecialNavigationLeft_ButtonGraphic.png", 4815);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\nl\DPoPS.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\DechenesXP\ModifiedIcon.ico", 56372);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\MpRTP.dll", 551512);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\ZH-CN\eula.rtf", 178460);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\_Extensions_Setup_DiscoveryMachineNames.xml", 151);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\Backup\amd64\epp.msi", 8982528);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\500-17.htm", 1254);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Windows Simulator\11.0\SensorsSimulatorDriver.cat", 9487);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\zh-TW\tipresx.dll.mui", 3584);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\1033\license_Dev.rtf", 25503);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\Microsoft.SQL.Chainer.Product.dll", 509800);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\ES-ES\setupres.dll.mui", 46688);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\msadc\msdaprst.dll", 389120);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20160419_132752\Datastore_LandingPage\Product.xml", 242565);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_AsInstanceId_Private.xml", 214);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\photoedge_selectionsubpicture.png", 4724);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\zh-Hant\DpFillin.dll.mui", 9728);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\DPAdminFVE64.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_Sku_Private.xml", 97);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\bg\DPoPS.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\en\Microsoft.SqlServer.WmiEnum.xml", 6363);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sk\DPTokensCard.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\es\DPTokens01.dll.mui", 20992);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Stacking\NavigationLeft_ButtonGraphic.png", 5088);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\WF\amd64\WDE.dll", 457360);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\MappingFiles\JetToSSIS.xml", 5984);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\14.0\PowerPoint\Shapes\Resources\Annotation.resources", 1693);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\hu\DPTokensSCa.dll.mui", 7680);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\msadc\msadcfr.dll", 8192);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_FailoverClusterName_Public.xml", 226);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_SSIS_Private.xml", 100);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\directshowtap.ax", 61440);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_SlpDumper_Private.xml", 103);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_Fulltext_Public.xml", 204);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1046\xeclrhostpkg.rll", 20160);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_Agent_Private.xml", 201);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\tt.txt", 18409);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\DTEParse.dll", 109760);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\DechenesVista\IgnoredIcon.ico", 49030);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\FR-FR\eula.rtf", 149712);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\MappingFiles\JetToMSSql9.xml", 5536);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\11.0\amd64\1033\TFSOfficeAdd-inUI.dll", 388184);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_AS_Private.xml", 171);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\MSEnv\OpenInVSActiveXCtl_x64.dll", 139464);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\OFFICE15\Csi.dll", 7759104);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\es\DpFillin.dll.mui", 17920);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Travel\PassportMask.wmv", 29268);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\he\WinMagic.HP.SecurityManagerPlugin.resources.dll", 24256);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Circle_SelectionSubpictureB.png", 3215);
            undertest.TryWrite(@"C:\Program Files\Microsoft Office\Office15\ONFILTER.DLL", 2165432);
            undertest.TryWrite(@"C:\Program Files\IIS Express\iisRtl.dll", 211088);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\_Extensions_Setup_DiscoveryMachineNames.xml", 151);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\en\Microsoft.SqlServer.RegSvrEnum.xml", 27353);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\CS-CZ\setupres.dll.mui", 46176);
            undertest.TryWrite(@"C:\Program Files\Microsoft Office\Office15\URLREDIR.DLL", 881880);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\lt\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\KeyFile\1033\sql_common_core_loc_keyfile.dll", 57024);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Shared\instapi10.dll", 43544);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_SqlBrowser_Private.xml", 488);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\Cartridges\db2v0801.xsl", 30084);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\es\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Performance\Title_Page.wmv", 1741316);
            undertest.TryWrite(@"C:\Program Files\Intel\Media SDK\v1_w7_32.vp", 15755);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_AgentResourceDescription.xml", 40);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\Resources\1033\dtsmsg120.rll", 522944);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Blip\ConflictIcon.ico", 32369);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\es\Microsoft.VisualBasic.resources.DLL", 11024);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\BabyBoy\BabyBoyScenesBackground.wmv", 149292);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_AtxcorePath.xml", 97);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Performance\PreviousMenuButtonIconSubpi.png", 3082);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\tr\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\fi-FI\tipresx.dll.mui", 3584);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\fi\Microsoft.VisualBasic.resources.dll", 11024);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPAuthEn.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\es\msdeploy.resources.dll", 68240);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\InputPersonalization.exe", 383488);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Include\sqlncli.h", 175473);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_Sku_Public.xml", 194);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\DPAdminFVEmsg.dll", 199680);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\eu\system.resources.dll", 11504);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Tasks\Microsoft.SqlServer.TransferDatabasesTask.dll", 58048);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\th\Microsoft.VisualBasic.resources.dll", 11024);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Full\dotsdarkoverlay.png", 4563);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\UK-UA\eula.rtf", 206134);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_LogFilePath.xml", 97);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\_Extensions_Msi_FeatureScenario.xml", 947495);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_SqlBrowserStopService_Private.xml", 226);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_SqlRSSHP_Public.xml", 103);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\fr.txt", 14652);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\License Terms\License_SqlLocalDB_1042.txt", 8712);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\es\WebpiCmd.resources.dll", 47776);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\xelocaldbpkg.mof", 8620);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sv\DpFbView.dll.mui", 13824);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\zh-CHT\WebpiCmd.resources.dll", 47776);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SqlServer.SqlEnum.dll", 1062744);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPCms.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ru\DPRsaTokPdr.dll.mui", 14336);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_AgentLocalGroupDescription.xml", 163);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\Tools\Binn\Resources\en-US\SqlLocalDB.rll.mui", 27224);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1036\xesospkg.rll", 32448);
            undertest.TryWrite(@"C:\Program Files\IIS Express\config\schema\rewrite_schema.xml", 14104);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_SniServer_Private.xml", 237);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_Slp_Private.xml", 98);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPUserPolicies.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\en-US\join.avi", 222208);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\Shared\SqlDumper.exe", 109656);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Function\ReadOnlyIcon.ico", 15086);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ar\DpUTrain.dll.mui", 16384);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\he\DPRsaTokPdr.dll.mui", 11776);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\ja\WebpiCmd.resources.dll", 51872);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\ado\msado25.tlb", 73728);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Stationery\Peacock.jpg", 5115);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\DechenesXP\ReadOnlyIcon.ico", 50051);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_ClusterIPAddresses_Private.xml", 452);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1031\XPStar.rll", 68800);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\no\DPEventMsg.dll.mui", 9568);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\UpgradeWizard.xml", 11430);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VSTO\vstoee100.tlb", 17040);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Help\help.chm", 81177);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\th\system.resources.dll", 11504);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPWinRTUtils.dll", 114000);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\Resources\1033\DTEParse.rll", 52928);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DpCardEngine.exe", 390480);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\SpecialOccasion\NavigationUp_SelectionSubpicture.png", 3081);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\CVSClassic\ReadOnlyIcon.ico", 17814);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1031\xesqlminpkg.rll", 259264);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\lt\DpUTrain.dll.mui", 17920);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\12.0\TfsNop.exe", 24216);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VSTO\10.0\Microsoft Visual Studio 2010 Tools for Office Runtime (x64)\vstor40_x64.MSI", 548352);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\404-12.htm", 1120);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\14.0\Microsoft.TeamFoundation.Diff.dll", 38648);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extensions_Msi_PackageInstallMap.xml", 617725);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\11.0\Microsoft.TeamFoundation.OfficeIntegration.Common.tlb", 9216);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\ast.txt", 10640);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_SSIS_Public.xml", 302);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\tr\DpFbView.dll.mui", 14336);
            undertest.TryWrite(@"C:\Program Files\IIS Express\iisexpress.exe.manifest", 1266);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Pets\Notes_INTRO_BG.wmv", 237226);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\Help\1033\s10ch_setup.chm", 1309625);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPBthPTok.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Microsoft Help Viewer\v1.0\sqmapi.dll", 173384);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\403-14.htm", 1167);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\sk\mscorlib.resources.dll", 12024);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\Cartridges\Informix.xsl", 31527);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\RepairWizard.xml", 4325);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\UpgradeMappings\mapping.xml.sample", 1210);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\FlipPage\NavigationUp_SelectionSubpicture.png", 3081);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Tasks\Microsoft.SqlServer.TransferJobsTask.dll", 59584);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\ku-ckb.txt", 19711);
            undertest.TryWrite(@"C:\Program Files\IIS Express\gzip.dll", 40080);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\14.0\TFSFieldMapping.exe", 40128);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VSTO\10.0\Microsoft Visual Studio 2010 Tools for Office Runtime (x64)\install.res.3082.dll", 53416);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\cs\HPDriveEncryption.chm", 47730);
            undertest.TryWrite(@"C:\Program Files\IIS Express\hwebcore.dll", 26768);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\Silverlight.ConfigurationUI.dll", 851128);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\fr\DPTokens01.dll.mui", 22528);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\0x041b.ini", 23534);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extension_Agent_SqlDataPath.xml", 80);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\SpecialOccasion\NavigationLeft_ButtonGraphic.png", 5088);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sl\DPoPS.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.Management.XEvent.dll", 162904);
            undertest.TryWrite(@"C:\Program Files\IIS Express\config\schema\WebDAV_schema.xml", 4757);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\x86\axnative.dll", 295568);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\fr\mscorrc.dll", 9952);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_Repl_Private.xml", 100);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_SqlPowershell_Public.xml", 113);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\500.htm", 1133);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_SSIS_Private.xml", 100);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_ClusterIPAddresses_Private.xml", 510);
            undertest.TryWrite(@"C:\Program Files\Common Files\SpeechEngines\Microsoft\TTS20\MSTTSCommon.dll", 41472);
            undertest.TryWrite(@"C:\Program Files\IIS Express\iis_ssi.dll", 40592);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\cs\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\Bonjour\mDNSResponder.exe", 462184);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\WinMagicSecurityPlugin.InstallState", 2597);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\hr\DPTokens01.dll.mui", 19456);
            undertest.TryWrite(@"C:\Program Files\IIS Express\loghttp.dll", 40592);
            undertest.TryWrite(@"C:\Program Files\IIS Express\WebSite1\w-brand.png", 10165);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\en-US\DpHostW.exe.mui", 4096);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\hr\Microsoft.VisualBasic.resources.dll", 11024);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Stationery\Music.emf", 26036);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\en-US\tipresx.dll.mui", 3584);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\_Extension_SlpExtension_CompleteImageShortcut_Arguments.xml", 38);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\de\DpFillin.dll.mui", 17920);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\SDK\Assemblies\en\Microsoft.SqlServer.Management.Utility.xml", 122688);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Full\NavigationUp_SelectionSubpicture.png", 3081);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\_Extension_SlpExtension_CompleteImageShortcut_Description.xml", 58);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\et\WinMagic.HP.SecurityManagerPlugin.resources.dll", 22720);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\sk.txt", 14323);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_SqlInstanceId_Private.xml", 289);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\Shared\sqldk.dll", 2004568);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\Microsoft.SqlServer.Configuration.Dmf.dll", 296808);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SqlServer.Management.Sdk.Sfc.dll", 403304);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\nl\DPFPToken.dll.mui", 10752);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\mn.txt", 14657);
            undertest.TryWrite(@"C:\Program Files\Intel\Media SDK\mfx_mft_mjpgvd_w7_32.dll", 456704);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_SqlRS_Public.xml", 196);
            undertest.TryWrite(@"C:\Program Files\IIS Express\en-us\iisexpress.exe.mui", 17552);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\xeclrhostpkg.mof", 5137);
            undertest.TryWrite(@"C:\Program Files\Microsoft Office\Office15\AppSharingChromeHook64.dll", 23200);
            undertest.TryWrite(@"C:\Program Files\Internet Explorer\pdm.dll", 542272);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extension_Agent_IniSourceFilePath.xml", 99);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1033\xeclrhostpkg.rll", 19648);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\no\system.resources.dll", 11504);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\msadc\msadce.dll", 749568);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\cs\DPTokensSpareKey.dll.mui", 22528);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\uz.txt", 13591);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\resources\ProgressWarn.ico", 1406);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\mr.txt", 17597);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_SqlPowershell_Public.xml", 212);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\PL-PL\setupres.dll.mui", 48736);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\WinMagicSecurityPlugin.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\Product.xml", 242434);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1033\xesqlminpkg.rll", 210624);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ja\DPUserPolicies.resources.dll", 15696);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1028\xplog70.rll", 20672);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\ReportViewer_Cpu32_1.log", 502508);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_ClusterGroup_Private.xml", 562);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sk\DpUTrain.dll.mui", 17408);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\0x0416.ini", 24142);
            undertest.TryWrite(@"C:\Program Files\Internet Explorer\Timeline_is.dll", 171520);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\th\DPEventMsg.dll.mui", 9568);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Filters\VISFILT.DLL", 3937528);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\0x0404.ini", 10670);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DpBranding.dll", 2801488);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\PipelineComponents\OleDbSrc.dll", 276160);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\403-17.htm", 1299);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPTokens01Lib.dll", 18256);
            undertest.TryWrite(@"C:\Program Files\Microsoft Help Viewer\v1.0\StopWords\pt-br.stp", 1516);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\eo.txt", 10637);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_AS_Public.xml", 530);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\SK-SK\setupres.dll.mui", 46176);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_SqlBrowserStopService_Private.xml", 226);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\500-18.htm", 1257);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\DTSWizard.exe.config", 1688);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\Templates\model.mdf", 2162688);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\msolap120.dll", 8568000);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.PolicyEnum.dll", 52312);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\FlipPage\pagecurl.png", 24520);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPBthMSImpl.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\pt\Microsoft.VisualBasic.resources.dll", 11024);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPCrStor.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\PipelineComponents\Resources\1033\dtspipeline.rll", 116416);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\de\mscorrc.dll", 10464);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sv\DPTokensBluetooth.dll.mui", 5120);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\System.Net.dll", 227992);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sr\DPUserPolicies.resources.dll", 14672);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\HR-HR\setupres.dll.mui", 46688);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\zh-Hant\DPUserPolicies.resources.dll", 14160);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VSTO\10.0\Microsoft Visual Studio 2010 Tools for Office Runtime (x64)\install.res.1044.dll", 49832);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ko\DPOnlineIDs.dll.mui", 13312);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\fi\DPTokensCard.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\14.0\PowerPoint\Shapes\Resources\Windows Desktop.resources", 6479);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\lt\DPTokensBluetooth.dll.mui", 5120);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\VSSMO_Cpu32_1.log", 1679256);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Stationery\grid_(cm).wmf", 2920);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\pt-BR\DPTokensSpareKey.dll.mui", 23040);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.WmiEnum.dll", 56408);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Rectangles\NavigationLeft_SelectionSubpicture.png", 3130);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.SmoExtended.dll", 232960);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\en-US\DpMiniOnlineIds.dll.mui", 4096);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Straight\AddedIcon.ico", 22486);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ru\DpHostW.exe.mui", 4096);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\resources\ProgressSuccess.ico", 1406);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPCmsGPOClient.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\Silverlight.Configuration.exe", 304824);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_ManagementToolsAdvanced_Private.xml", 138);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\en-US\InkObj.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\pl\mscorrc.dll", 9952);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\System.Windows.RuntimeHost.ni.dll", 76288);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\HU-HU\eula.rtf", 171690);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\pidgen.dll", 38936);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extension_Agent_AgentSysadminSID.xml", 79);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Straight\ModifiedIcon.ico", 22486);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\SDK\Assemblies\en\Microsoft.SqlServer.Management.Sdk.Sfc.xml", 804548);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Stacking\NavigationUp_SelectionSubpicture.png", 3081);
            undertest.TryWrite(@"C:\Program Files\Internet Explorer\DiagnosticsHub.DataWarehouse.dll", 666624);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\VSPrerequisites_Cpu64_1.log", 766066);
            undertest.TryWrite(@"C:\Program Files\IIS Express\ru-RU\appobj.dll.mui", 91280);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\Runtime_ActionData.xml", 29517);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_SniServer_Public.xml", 241);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Stacking\720x480icongraphic.png", 5620);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\sqmapi.dll", 284312);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\sk\HPDriveEncryption.chm", 47258);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\DechenesVista\DeletedIcon.ico", 55499);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\ProductSettings_SqlEngine_Public.xml", 1271);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\DechenesVista\UnversionedIcon.ico", 63990);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\pl\system.resources.dll", 11504);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\EppManifest.dll", 187200);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\zh-Hans\mscorrc.dll", 9440);
            undertest.TryWrite(@"C:\Program Files\IIS Express\ko-KR\appcmd.exe.mui", 9872);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\sr\DPTokensSpareKey.dll.mui", 22016);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\nl\DPTokensBluetooth.dll.mui", 5632);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Shatter\NavigationLeft_SelectionSubpicture.png", 3130);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\KeyFile\1033\rsfx_keyfile.dll", 57024);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1041\xepackage0.rll", 31424);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\OldAge\decorative_rule.png", 6203);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\Backup\amd64\setup.exe", 1112064);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\ZH-CN\setupres.dll.mui", 24008);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1042\odsole70.rll", 25280);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\el\DPEventMsg.dll.mui", 10080);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\KeyFile\1033\sqlsqm_keyfile.dll", 57024);
            undertest.TryWrite(@"C:\Program Files\IIS Express\pt\IisExpressadminCmd.resources.dll", 12432);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\401-1.htm", 1222);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\ru\msdeploy.resources.dll", 84624);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\SpecialOccasion\SpecialNavigationUp_ButtonGraphic.png", 4866);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPCardHID.dll", 324944);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_Agent_Private.xml", 102);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\da\HPDriveEncryption.chm", 45984);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_ClusterState.xml", 67);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Stationery\Desktop.ini", 645);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\SDK\Assemblies\Microsoft.SqlServer.SqlEnum.dll", 1355456);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\Shared\xe.dll", 476144);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\Microsoft.Xna.Framework.Graphics.dll", 61640);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\Datastore\Package.xml", 110243);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\BabyBoy\BabyBoyMainToNotesBackground_PAL.wmv", 157214);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\Microsoft.Web.PlatformInstaller.UI.dll", 638624);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPTokensBluetooth.dll.hpsign", 256);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Team Foundation Server\14.0\PowerPoint\Shapes\Windows Apps.sbsx", 323345);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\zh-CHS\Microsoft.Web.PlatformInstaller.UI.resources.dll", 47776);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\hkcompile.dll", 751296);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\setup100.exe", 59240);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\en\msdeploy.resources.dll", 64144);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\no\mscorrc.dll", 9952);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\_Extension_Agent_AgentServiceAccount.xml", 44);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\de\WinMagic.HP.SecurityManagerPlugin.resources.dll", 24256);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\KeyFile\1033\sqlsysclrtypes_keyfile.dll", 14176);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\Stationery\Garden.htm", 231);
            undertest.TryWrite(@"C:\Program Files\Microsoft\Web Platform Installer\pl\WebpiCmd.resources.dll", 51872);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\SK-SK\amhelp.chm", 150947);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\Resources\1033\odsole70.rll", 27224);
            undertest.TryWrite(@"C:\Program Files\IIS Express\authmap.dll", 50320);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\VSTO\10.0\Microsoft Visual Studio 2010 Tools for Office Runtime (x64)\eula.1042.txt", 5848);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ro\DPFPToken.dll.mui", 10240);
            undertest.TryWrite(@"C:\Program Files\Microsoft Help Viewer\v1.0\Microsoft Help Viewer 1.1\install.res.2052.dll", 31584);
            undertest.TryWrite(@"C:\Program Files\Internet Explorer\nl-NL\jsprofilerui.dll.mui", 8704);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\Microsoft.SqlServer.DtsMsg.dll", 156352);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\Newtonsoft.Json.dll", 399552);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Pets\Pets_frame-shadow.png", 25662);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\ProductSettings_ClusterGroup_Private.xml", 562);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\ES-ES\eula.rtf", 125752);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Memories\16_9-frame-overlay.png", 35858);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extension_Agent_LocalMachineName.xml", 151);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Release\x64\1033\license_Std.rtf", 41746);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\BabyGirl\16_9-frame-image-mask.png", 1551);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\Microsoft.Web.Deployment.Tracing.dll", 39568);
            undertest.TryWrite(@"C:\Program Files\IIS Express\ru-RU\iisexpresstray.exe.mui", 9360);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_Sku_Private.xml", 196);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\sr-Cyrl-CS\mscorrc.dll", 9952);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\xmsrv.dll", 19106496);
            undertest.TryWrite(@"C:\Program Files\Microsoft Security Client\Drivers\mpfilter\mpfilter.cat", 8359);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\110\LocalDB\Binn\sqlscm.dll", 59480);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\Product.xml", 242565);
            undertest.TryWrite(@"C:\Program Files\IIS Express\config\schema\IIS_schema.xml", 91090);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\BabyGirl\Bear_Formatted_RGB6_PAL.wmv", 269322);
            undertest.TryWrite(@"C:\Program Files\Microsoft Analysis Services\AS OLEDB\120\SQLDumper.exe", 120000);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_AsDumper_Private.xml", 101);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\RemoveBL.DOS", 25007);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\403-7.htm", 1335);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1046\xepackage0.rll", 45248);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\msadc\en-US\msadcor.dll.mui", 5632);
            undertest.TryWrite(@"C:\Program Files\IIS\Microsoft Web Deploy V3\it\Web Deploy UI.chm", 222535);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\ProductSettings_ClusterDisk_Private.xml", 411);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\XPStar.dll", 419008);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\ja\DPOnlineIDs.dll.mui", 14336);
            undertest.TryWrite(@"C:\Program Files\Intel\Intel(R) Management Engine Components\IPT\iptWys64.dll", 254912);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\ru\sqlaccess.resources.dll", 37056);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Tasks\en\Microsoft.SqlServer.WMIEWTask.xml", 22807);
            undertest.TryWrite(@"C:\Program Files\IIS Express\pl\iisexpresstray.resources.dll", 51856);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\id.txt", 13337);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Vignette\vignettemask25.png", 56543);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\DPDeviceValidity301.dll", 496976);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\no\DPTokensCard.dll.mui", 4608);
            undertest.TryWrite(@"C:\Program Files\Intel\Intel(R) Management Engine Components\IPT\UpdateServiceCProxy64.dll", 211208);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\hr\DPFPToken.dll.mui", 9728);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\nl\DPTokensSCa.dll.mui", 7168);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\Drive Encryption\h2.bin", 19968);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\16to9Squareframe_VideoInset.png", 3316);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\ProductSettings_Fulltext_Private.xml", 108);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\Vignette\NavigationLeft_ButtonGraphic.png", 5088);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\uk.txt", 19729);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\system.dll", 239248);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\Filters.xml", 14239);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\lv\DPTokensSpareKey.dll.mui", 22528);
            undertest.TryWrite(@"C:\Program Files\IIS Express\custerr\en-us\500-15.htm", 1210);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\SDK\Assemblies\en\Microsoft.SqlServer.RegSvrEnum.xml", 27353);
            undertest.TryWrite(@"C:\Program Files\DVD Maker\Shared\DvdStyles\LayeredTitles\layers.png", 24557);
            undertest.TryWrite(@"C:\Program Files\IIS Express\IisExpressAdminCmd.exe", 47760);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\Binn\Resources\1033\dtspipeline.rll", 116416);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_084111\Datastore\_Extension_Agent_AgentServiceAccountSID.xml", 25);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\DTS\MappingFiles\JetToIBMDB2.XML", 5471);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\InputSettings_ChainerSettings_SlpSettings.xml", 1453);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\Modern\ConflictIcon.ico", 20270);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\LocalDB\Binn\Resources\1031\odsole70.rll", 30400);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\MSSQLSERVER\Datastore\InputSettings_RsDumper_Private.xml", 101);
            undertest.TryWrite(@"C:\Program Files\Common Files\TortoiseOverlays\icons\XPStyle\ModifiedIcon.ico", 40903);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\it\DPUserPolicies.resources.dll", 14672);
            undertest.TryWrite(@"C:\Program Files\Common Files\System\msadc\msdfmap.dll", 57344);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_081100\resources\ProgressSuccess.ico", 1406);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20141203_082121\Datastore\_Extensions_Msi_InitialPackageInstallList.xml", 352277);
            undertest.TryWrite(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe", 224768);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\ru\mscorrc.dll", 9952);
            undertest.TryWrite(@"C:\Program Files\Hewlett-Packard\HP ProtectTools Security Manager\Bin\pt-PT\DPRsaTokPdr.dll.mui", 15360);
            undertest.TryWrite(@"C:\Program Files\Intel\Media SDK\he_w7_64.vp", 4016);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\KeyFile\1033\sql_tools_loc_keyfile.dll", 57024);
            undertest.TryWrite(@"C:\Program Files\Microsoft SQL Server\120\Setup Bootstrap\Log\20150715_030834\Datastore\ProductSettings_SqlLegacyDiscovery_Public.xml", 128);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\eu.txt", 12799);
            undertest.TryWrite(@"C:\Program Files\IIS Express\urlauthz.dll", 31376);
            undertest.TryWrite(@"C:\Program Files\7-Zip\Lang\el.txt", 21536);
            undertest.TryWrite(@"C:\Program Files\Microsoft Silverlight\5.1.41212.0\System.ServiceModel.Web.ni.dll", 174080);
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