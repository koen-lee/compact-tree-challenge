using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Trie
{
    [TestFixture]
    public class TrieComparisonTests
    {
        [Test]
        public void CompareTries()
        {
            var notrie = new NoTrie();
            var realtrie = new RealTrie();
            var itemsInNoTrie = 0;
            var itemsInRealTrie = 0;
            foreach (var kv in GetTestData().Take(10000))
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
                if (notrie.TryWrite(kv.Key, kv.Value))
                    itemsInNoTrie++;
                if (realtrie.TryWrite(kv.Key, kv.Value))
                    itemsInRealTrie++;
            }

            Console.WriteLine($"Items in flat storage: {itemsInNoTrie}");
            Console.WriteLine($"Items in trie storage: {itemsInRealTrie}");
        }

        IEnumerable<KeyValuePair<string, long>> GetTestData()
        {
            return EnumerateFilesRecursively(new DirectoryInfo(@"C:\Program Files"))
                .Select(file => new KeyValuePair<string, long>(file.FullName, file.Length));
        }

        private IEnumerable<FileInfo> EnumerateFilesRecursively(DirectoryInfo directoryInfo)
        {
            return directoryInfo.EnumerateFiles()
                .Concat(directoryInfo.EnumerateDirectories()
                    .SelectMany(EnumerateFilesRecursively));
        }
    }
}