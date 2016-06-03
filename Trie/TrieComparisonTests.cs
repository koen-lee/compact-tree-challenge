using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var testdata = GetTestData().Take(32 * 1024 / 8).ToArray();
            
            TestTrie(new NoTrie(), testdata);
            TestTrie(new RealTrie(), testdata);
            TestTrie(new OptimizedTrie(), testdata);
        }

        private static void TestTrie(ITrie trie, KeyValuePair<string, long>[] testdata)
        {
            var items = 0;
            Console.WriteLine($"Testing {trie.GetType()}");
            var stopwatch = Stopwatch.StartNew();
            foreach (var kv in testdata)
            {
                if (!trie.TryWrite(kv.Key, kv.Value))
                    break;
                items++;
            }
            Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine($"Items in {trie.GetType()}: {items}");
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